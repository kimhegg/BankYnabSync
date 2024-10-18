using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BankYnabSync.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using Moq.Protected;
using Xunit;

namespace BankYnabSyncUnitTests
{
    public class BankNorwegianTests
    {
        [Fact]
        public async Task GetTransactions_ShouldRefreshTokenAndRetry_WhenInitialRequestUnauthorized()
        {
            // Arrange
            var configDictionary = new Dictionary<string, string>
            {
                {"BankNorwegian:BaseUrl", "https://api.example.com/"},
                {"BankNorwegian:AccountPath", "accounts/transactions"},
                {"BankNorwegian:Access", "initial_access_token"},
                {"BankNorwegian:Refresh", "initial_refresh_token"}
            };

            var configuration = new TestConfiguration(configDictionary);

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.Unauthorized))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(@"{""access"":""new_access_token"",""refresh"":""new_refresh_token""}")
                })
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(@"{""transactions"":{""booked"":[]}}")
                });

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);

            var bankNorwegian = new BankNorwegian(configuration, httpClient);

            // Act
            var transactions = await bankNorwegian.GetTransactions();

            // Assert
            httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(3),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );

            Assert.Empty(transactions);
            Assert.Equal("new_access_token", configuration["BankNorwegian:Access"]);
            Assert.Equal("new_refresh_token", configuration["BankNorwegian:Refresh"]);
        }
    }

    public class TestConfiguration : IConfigurationRoot
    {
        private readonly Dictionary<string, string> _data;

        public TestConfiguration(Dictionary<string, string> initialData)
        {
            _data = new Dictionary<string, string>(initialData);
        }

        public string this[string key]
        {
            get => _data.TryGetValue(key, out var value) ? value : null;
            set => _data[key] = value;
        }

        public IEnumerable<IConfigurationSection> GetChildren() =>
            _data.Keys.Select(k => k.Split(':')[0]).Distinct().Select(k => GetSection(k));

        public IChangeToken GetReloadToken() => new ConfigurationReloadToken();

        public IConfigurationSection GetSection(string key) => new TestConfigurationSection(this, key);

        public void Reload() { }

        public IConfigurationProvider GetProvider() => new TestConfigurationProvider(_data);

        public IEnumerable<IConfigurationProvider> Providers => new[] { GetProvider() };
    }

    public class TestConfigurationSection : IConfigurationSection
    {
        private readonly IConfiguration _configuration;
        private readonly string _path;

        public TestConfigurationSection(IConfiguration configuration, string path)
        {
            _configuration = configuration;
            _path = path;
        }

        public string this[string key]
        {
            get => _configuration[$"{_path}:{key}"];
            set => _configuration[$"{_path}:{key}"] = value;
        }

        public string Key => _path.Split(':').Last();
        public string Path => _path;
        public string Value
        {
            get => _configuration[_path];
            set => _configuration[_path] = value;
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return _configuration.GetChildren()
                .Where(c => c.Path.StartsWith($"{_path}:"))
                .Select(c => new TestConfigurationSection(_configuration, c.Path));
        }

        public IChangeToken GetReloadToken() => new ConfigurationReloadToken();

        public IConfigurationSection GetSection(string key) =>
            new TestConfigurationSection(_configuration, $"{_path}:{key}");
    }

    public class TestConfigurationProvider : IConfigurationProvider
    {
        private readonly Dictionary<string, string> _data;

        public TestConfigurationProvider(Dictionary<string, string> data)
        {
            _data = data;
        }

        public bool TryGet(string key, out string value) => _data.TryGetValue(key, out value);

        public void Set(string key, string value) => _data[key] = value;

        public IChangeToken GetReloadToken() => new ConfigurationReloadToken();

        public void Load() { }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            var prefix = parentPath == null ? string.Empty : parentPath + ":";
            return _data.Keys
                .Where(k => k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(k => k.Substring(prefix.Length))
                .Concat(earlierKeys)
                .OrderBy(k => k, StringComparer.OrdinalIgnoreCase)
                .Distinct();
        }
    }
}

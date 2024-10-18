using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using BankYnabSync.Models;
using Microsoft.Extensions.Configuration;

namespace BankYnabSync.Services;

public class BankNorwegian : IBank
{
    public string Name => "Bank Norwegian";
    private readonly string _baseUrl;
    private string _accessToken;
    private string _refreshToken;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IConfiguration _configuration;
    private readonly string _accountPath;

    public BankNorwegian(IConfiguration configuration)
    {
        _baseUrl = configuration["BankNorwegian:BaseUrl"];
        _accountPath = configuration["BankNorwegian:AccountPath"];
        _accessToken = configuration["BankNorwegian:Access"];
        _refreshToken = configuration["BankNorwegian:Refresh"];

        if (string.IsNullOrEmpty(_baseUrl) || string.IsNullOrEmpty(_accessToken) || string.IsNullOrEmpty(_refreshToken))
            throw new InvalidOperationException("Bank Norwegian URL or API Key is missing from configuration.");


        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true};
        _jsonOptions.Converters.Add(new DecimalConverter());
        _configuration = configuration;
    }

    public async Task<List<Transaction>> GetTransactions()
    {
        try
        {
            var response = await SendRequestWithTokenRefresh();

            LogRateLimitInfo(response);
            response.EnsureSuccessStatusCode();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(await response.Content.ReadAsStringAsync(), _jsonOptions);
            //var apiResponse = JsonSerializer.Deserialize<ApiResponse>(LoadResultFile(), _jsonOptions);

            if (apiResponse != null)
                return ConvertToTransactions(apiResponse);
            
            Console.WriteLine("Failed to deserialize the API response.");
            return [];

        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error fetching transactions: {e.Message}");
            return [];
        }
        catch (JsonException e)
        {
            Console.WriteLine($"Error deserializing JSON: {e.Message}");
            return [];
        }
    }

    private string LoadResultFile()
    {
        return File.ReadAllText("result.json");
    }

    private void LogRateLimitInfo(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("HTTP_X_RATELIMIT_ACCOUNT_SUCCESS_LIMIT", out var limitValues))
            Console.WriteLine($"Rate Limit: {limitValues.FirstOrDefault()}");

        if (response.Headers.TryGetValues("HTTP_X_RATELIMIT_ACCOUNT_SUCCESS_REMAINING", out var remainingValues))
            Console.WriteLine($"Rate Limit Remaining: {remainingValues.FirstOrDefault()}");

        if (response.Headers.TryGetValues("HTTP_X_RATELIMIT_ACCOUNT_SUCCESS_RESET", out var resetValues))
            Console.WriteLine($"Rate Limit Reset: {resetValues.FirstOrDefault()}s");
    }

    private List<Transaction> ConvertToTransactions(ApiResponse apiResponse)
    {
        return apiResponse.Transactions.Booked.Select(item => new Transaction
        {
            Id = item.TransactionId,
            Date = DateTime.Parse(item.BookingDate),
            Amount = item.TransactionAmount.Amount,
            Payee = item.CreditorName,
            Category = item.AdditionalInformation
        })
            .ToList();
    }
    
    private async Task<HttpResponseMessage> SendRequestWithTokenRefresh()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}{_accountPath}");

        if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized) 
            return response;
        
        Console.WriteLine("Access token expired. Refreshing token...");
        await RefreshToken();
        response = await _httpClient.GetAsync(_baseUrl);

        return response;
    }

    private async Task RefreshToken()
    {
        var refreshTokenUrl = $"{_baseUrl}api/v2/token/refresh/";

        var request = new HttpRequestMessage(HttpMethod.Post, refreshTokenUrl);
        request.Headers.Add("Authorization", $"Bearer {_refreshToken}");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadAsStringAsync();
        var newAccessToken = JsonSerializer.Deserialize<Dictionary<string, string>>(tokenResponse)?["access"] ?? throw new InvalidOperationException("Access token not found in response");;
        _accessToken = newAccessToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        UpdateAccessTokenInConfiguration(newAccessToken);
    }
    

    private void UpdateAccessTokenInConfiguration(string newAccessToken)
    {
        var configRoot = (IConfigurationRoot)_configuration;
        configRoot["BankNorwegian:ApiKey"] = newAccessToken;
        var filePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        File.WriteAllText(filePath, JsonSerializer.Serialize(configRoot.GetSection("BankNorwegian").Get<Dictionary<string, string>>(), new JsonSerializerOptions { WriteIndented = true }));
    }

}


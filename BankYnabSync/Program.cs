
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BankYnabSync.Models.Bank;
using BankYnabSync.Models.Repositories;
using BankYnabSync.Models.Services;
using BankYnabSync.Repository;
using BankYnabSync.Services;
using BankYnabSync.Services.Tools;

namespace BankYnabSync;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var syncService = services.GetRequiredService<SyncService>();
                await syncService.SyncTransactions();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IBank, BankService>();
                    services.AddTransient<IYnabRepository, YnabRepository>();
                    services.AddTransient<IBankRepository, BankRepository>();
                    services.AddTransient<IYnabService, YnabService>();
                    services.AddTransient<ISecretService, SecretService>();
                    services.AddTransient<SyncService>();
                });
    }

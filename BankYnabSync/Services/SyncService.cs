
using BankYnabSync.Models;
using Microsoft.Extensions.Configuration;

namespace BankYnabSync.Services
{
    public class SyncService
    {
        private readonly List<IBank> _banks;
        private readonly YnabClient _ynabClient;

        public SyncService(IConfiguration configuration)
        {
            _banks = new List<IBank> { new BankNorwegian(configuration) };
            _ynabClient = new YnabClient(configuration["Ynab:ApiKey"]);
        }

        public async Task SyncTransactions()
        {
            foreach (var bank in _banks)
            {
                var transactions = await bank.GetTransactions();
                
               //ø https://developer.gocardless.com/bank-account-data/endpoints
                await _ynabClient.AddTransactions(transactions);
                
                Console.WriteLine($"Synced {transactions.Count} transactions from {bank.Name}");
            }
        }
    }
}
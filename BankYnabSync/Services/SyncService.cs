
using BankYnabSync.Models;
using BankYnabSync.Models.Bank;
using BankYnabSync.Models.Services;
using Microsoft.Extensions.Configuration;

namespace BankYnabSync.Services
{
    public class SyncService(IConfiguration configuration, IYnabService ynabService)
    {
        public async Task SyncTransactions()
        {
            var banks = new List<IBank> { new BankNorwegian(configuration) };
            foreach (var bank in banks)
            {
                try
                {
                    await ynabService.InsertTransactionsFromBank(bank);
                    Console.WriteLine($"Synced transactions from {bank.Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error syncing transactions from {bank.Name}: {ex.Message}");
                }
            }
        }
    }
}

using BankYnabSync.Models.Bank;
using BankYnabSync.Models.Repositories;
using BankYnabSync.Models.Services;


namespace BankYnabSync.Services
{
    public class SyncService(IYnabService ynabService, IBankRepository bankRepository)
    {
        public async Task SyncTransactions()
        {
            var banks = new List<IBank> { new BankNorwegian(bankRepository) };
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
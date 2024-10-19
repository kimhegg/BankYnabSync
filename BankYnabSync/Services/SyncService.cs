
using BankYnabSync.Models.Bank;
using BankYnabSync.Models.Repositories;
using BankYnabSync.Models.Services;


namespace BankYnabSync.Services
{
    public class SyncService(IYnabService ynabService, IBankRepository bankRepository)
    {
        public async Task SyncTransactions()
        {
            var banks = new List<IBank> { new BankService(bankRepository) };
            foreach (var bank in banks)
            {
                try
                {
                    await ynabService.InsertTransactionsFromBanks(bank);
                    Console.WriteLine($"Synced transactions ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error syncing transactions: {ex.Message}");
                }
            }
        }
    }
}
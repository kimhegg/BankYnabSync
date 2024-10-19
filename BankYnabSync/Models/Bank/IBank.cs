namespace BankYnabSync.Models.Bank;

public interface IBank
{

    Task<List<Transaction>> GetTransactions(string bankAccountPath);
    Task<List<BankInfo>> GetBanksAsync();
}
namespace BankYnabSync.Models.Bank;

public interface IBank
{
    string Name { get; }
    Task<List<Transaction>> GetTransactions();
}
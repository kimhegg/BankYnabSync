namespace BankYnabSync.Models;

public interface IBank
{
    string Name { get; }
    Task<List<Transaction>> GetTransactions();
}
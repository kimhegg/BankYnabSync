using BankYnabSync.Models.Bank;

namespace BankYnabSync.Models.Repositories;

public interface IBankRepository
{
    Task<List<Transaction>> GetTransactions();
}
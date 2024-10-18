using BankYnabSync.Models.Bank;

namespace BankYnabSync.Models.Services;

public interface IYnabService
{
    Task InsertTransactionsFromBank(IBank bank);
}
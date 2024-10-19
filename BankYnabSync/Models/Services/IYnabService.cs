using BankYnabSync.Models.Bank;

namespace BankYnabSync.Models.Services;

public interface IYnabService
{
    Task InsertTransactionsFromBanks(IBank bank);
}
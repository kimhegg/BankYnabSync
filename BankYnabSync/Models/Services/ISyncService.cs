namespace BankYnabSync.Models.Services;

public interface ISyncService
{
    Task SyncTransactions();
}
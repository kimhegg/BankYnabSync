namespace BankYnabSync.Models.Services;

public interface ISecretService
{
    string GetAccessToken();
    string GetAccountPath();
    string GetRefreshToken();
    
}
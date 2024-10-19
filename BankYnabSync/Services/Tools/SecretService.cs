using BankYnabSync.Models.Services;
using Microsoft.Extensions.Configuration;

namespace BankYnabSync.Services.Tools;

public class SecretService(IConfiguration configuration) : ISecretService
{
    private readonly string _accountPath = configuration["BankNorwegian:AccountPath"];
    private readonly string _accessToken = configuration["BankNorwegian:Access"];
    private readonly string _refreshToken = configuration["BankNorwegian:Refresh"];
    
    
    public string GetAccountPath()
    {
        return _accountPath;
    }
    
    public string GetAccessToken()
    {
        return _accessToken;
    }
    
    public string GetRefreshToken()
    {
        return _refreshToken;
    }
}
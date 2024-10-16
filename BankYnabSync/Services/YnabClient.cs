using BankYnabSync.Models;

namespace BankYnabSync.Services;

public class YnabClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public YnabClient(string apiKey)
    {
        _httpClient = new HttpClient();
        _apiKey = apiKey;
        _httpClient.BaseAddress = new Uri("https://api.youneedabudget.com/v1/");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task AddTransactions(List<Transaction> transactions)
    {
        // Implement YNAB API call to add transactions
        throw new NotImplementedException();
    }
}
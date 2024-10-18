using System.Text.Json;
using System.Net.Http.Headers;
using BankYnabSync.Models.Repositories;
using BankYnabSync.Models.Ynab.Account;
using BankYnabSync.Models.Ynab.Budget;
using BankYnabSync.Models.Ynab.Category;
using Microsoft.Extensions.Configuration;

namespace BankYnabSync.Repository;

public class YnabRepository : IYnabRepository
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    public YnabRepository(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        var apiKey = configuration["Ynab:ApiKey"];
        _httpClient.BaseAddress = new Uri("https://api.youneedabudget.com/v1/");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task<T> GetAndDeserialize<T>(string endpoint)
    {
        var response = (await _httpClient.GetAsync(endpoint)).EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(), _options);

        if (result == null)
            throw new Exception($"Failed to deserialize the API response for endpoint: {endpoint}");

        return result;
    }

    public async Task<CategoryResponse> GetCategories(string budgetId)
    {
        return await GetAndDeserialize<CategoryResponse>($"budgets/{budgetId}/categories");
    }

    public async Task<IEnumerable<Account>> ListAccounts(string budgetId)
    {
        var response = await GetAndDeserialize<YnabAccountsResponse>($"budgets/{budgetId}/accounts");
        return response.Data.Accounts;
    }

    public async Task<IEnumerable<Budget>> ListBudgets()
    {
        var response = await GetAndDeserialize<YnabBudgetResponse>("budgets?include_accounts=false");
        return response.Data.Budgets;
    }

    public async Task InsertTransactions(string budgetId, object transactions)
    {
        var response = await _httpClient.PostAsync($"budgets/{budgetId}/transactions", new StringContent(JsonSerializer.Serialize(transactions, _options), System.Text.Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to insert transactions: {response.StatusCode}, {errorContent}");
        }
    }
}

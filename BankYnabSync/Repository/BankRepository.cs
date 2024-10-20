using System.Net.Http.Headers;
using System.Text.Json;
using BankYnabSync.Models;
using BankYnabSync.Models.Bank;
using BankYnabSync.Models.Repositories;
using BankYnabSync.Services.Tools;
using Microsoft.Extensions.Configuration;

namespace BankYnabSync.Repository;

public class BankRepository: IBankRepository
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly JsonSerializerOptions _jsonOptions;

    public BankRepository(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _configuration = configuration;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _jsonOptions.Converters.Add(new DecimalConverter());
    }
    
   
    
    public async Task<List<Transaction>> GetTransactions(string bankAccountPath)
    {
        var url = $"{_configuration["Bank:BaseUrl"]}{bankAccountPath}";
        var result = await FetchAndDeserialize<BankTransactionResponse>(url);
        return ConvertToTransactions(result);
    }

    public async Task<List<BankInfo>> GetBanks()
    {
        var url = "https://bankaccountdata.gocardless.com/api/v2/institutions/?country=no"; 
        var result = await FetchAndDeserialize<List<BankInfo>>(url);
        return result;
    }
    
    private async Task<T> FetchAndDeserialize<T>(string url) where T : class
    {
        try
        {
            var response = await SendRequestWithTokenRefresh(_httpClient, url);
            LogRateLimitInfo(response);
            response.EnsureSuccessStatusCode();
    
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(content, _jsonOptions);

            if (result != null)
                return result;
            
    
            Console.WriteLine($"Failed to deserialize the API response for {typeof(T).Name}.");
            return default;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error fetching data: {e.Message}");
            return default;
        }
        catch (JsonException e)
        {
            Console.WriteLine($"Error deserializing JSON: {e.Message}");
            return default;
        }
    }

    
    public async Task<List<Transaction>> GetFakeTransactionsAsync()
    {
        var result = LoadResultFile();
        return ConvertToTransactions(JsonSerializer.Deserialize<BankTransactionResponse>(result, _jsonOptions));
    }
    private string LoadResultFile()
    {
        return File.ReadAllText("result.json");
    }
    
    private List<Transaction> ConvertToTransactions(BankTransactionResponse bankTransactionResponse)
    {
        var bookedTransactions = bankTransactionResponse.Transactions.Booked.Select(ConvertBookedTransaction).ToList();
        var pendingTransactions = bankTransactionResponse.Transactions.Pending.Select(ConvertPendingTransaction).ToList();
    
        return bookedTransactions.Concat(pendingTransactions).ToList();
    }
 
    private Transaction ConvertBookedTransaction(BookedTransaction item)
    {
        return new Transaction
        {
            Id = item.TransactionId,
            Date = DateTime.Parse(item.BookingDate),
            Amount = item.TransactionAmount.Amount,
            Payee = item.CreditorName,
            Category = item.AdditionalInformation
        };
    }
    
    private Transaction ConvertPendingTransaction(PendingTransaction item)
    {
        return new Transaction
        {
            Id = item.TransactionId,
            Date = DateTime.Parse(item.ValueDate),
            Amount = item.TransactionAmount.Amount,
            Payee = item.RemittanceInformationUnstructured,
            Category = null, 
        };
    }

    private void LogRateLimitInfo(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("HTTP_X_RATELIMIT_ACCOUNT_SUCCESS_LIMIT", out var limitValues))
            Console.WriteLine($"Rate Limit: {limitValues.FirstOrDefault()}");

        if (response.Headers.TryGetValues("HTTP_X_RATELIMIT_ACCOUNT_SUCCESS_REMAINING", out var remainingValues))
            Console.WriteLine($"Rate Limit Remaining: {remainingValues.FirstOrDefault()}");

        if (response.Headers.TryGetValues("HTTP_X_RATELIMIT_ACCOUNT_SUCCESS_RESET", out var resetValues))
            Console.WriteLine($"Rate Limit Reset: {resetValues.FirstOrDefault()}s");
    }

  

    private async Task<HttpResponseMessage> SendRequestWithTokenRefresh(HttpClient httpClient, string url)
    {
        var response = await httpClient.GetAsync(url);

        if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
            return response;

        Console.WriteLine("Access token expired. Refreshing token...");
        await RefreshToken(httpClient);
        response = await httpClient.GetAsync(url);

        return response;
    }

    private async Task RefreshToken(HttpClient httpClient)
    {
        var refreshTokenUrl = $"{_configuration["Bank:BaseUrl"]}token/refresh/";

        var request = new HttpRequestMessage(HttpMethod.Post, refreshTokenUrl);
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("refresh", _configuration["Bank:Refresh"])
        });

        request.Content = content;

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var tokenResponseJson = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(tokenResponseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            throw new InvalidOperationException("Failed to deserialize token response or access token is missing");
        

        var accessToken = tokenResponse.AccessToken;
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        Console.WriteLine($"Token refreshed. New access token expires in {tokenResponse.AccessExpires} seconds.");
    }
    
}
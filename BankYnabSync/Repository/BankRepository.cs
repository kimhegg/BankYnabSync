using System.Net.Http.Headers;
using System.Text.Json;
using BankYnabSync.Models;
using BankYnabSync.Models.Bank;
using BankYnabSync.Models.Repositories;
using BankYnabSync.Services.Tools;
using Microsoft.Extensions.Configuration;

namespace BankYnabSync.Repository;

public class BankRepository(IConfiguration configuration) : IBankRepository
{
    
    public async Task<List<Transaction>> GetTransactions()
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration["BankNorwegian:Access"]);

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        jsonOptions.Converters.Add(new DecimalConverter());
        try
        {
            var url = $"{configuration["BankNorwegian:BaseUrl"]}{configuration["BankNorwegian:AccountPath"]}";
            var response = await SendRequestWithTokenRefresh(httpClient, url);
            LogRateLimitInfo(response);
            response.EnsureSuccessStatusCode();
            var apiResponse = JsonSerializer.Deserialize<BankTransactionResponse>(await response.Content.ReadAsStringAsync(), jsonOptions);
            //var apiResponse = JsonSerializer.Deserialize<BankTransactionResponse>(LoadResultFile(), _jsonOptions);

            if (apiResponse != null)
                return ConvertToTransactions(apiResponse);

            Console.WriteLine("Failed to deserialize the API response.");
            return default;

        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Error fetching transactions: {e.Message}");
            return default;
        }
        catch (JsonException e)
        {
            Console.WriteLine($"Error deserializing JSON: {e.Message}");
            return default;
        }finally
        {
            httpClient.Dispose();
        }
    }
    
    private string LoadResultFile()
    {
        return File.ReadAllText("result.json");
    }
    private List<Transaction> ConvertToTransactions(BankTransactionResponse bankTransactionResponse)
    {
        return bankTransactionResponse.Transactions.Booked.Select(item => new Transaction
            {
                Id = item.TransactionId,
                Date = DateTime.Parse(item.BookingDate),
                Amount = item.TransactionAmount.Amount,
                Payee = item.CreditorName,
                Category = item.AdditionalInformation
            })
            .ToList();
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
        var refreshTokenUrl = $"{configuration["BankNorwegian:BaseUrl"]}token/refresh/";

        var request = new HttpRequestMessage(HttpMethod.Post, refreshTokenUrl);
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("refresh", configuration["BankNorwegian:Refresh"])
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
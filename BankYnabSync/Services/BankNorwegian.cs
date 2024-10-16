using System.Net.Http.Headers;
using System.Text.Json;
using BankYnabSync.Models;
using Microsoft.Extensions.Configuration;

namespace BankYnabSync.Services;

    public class BankNorwegian: IBank
    {
      public string Name => "Bank Norwegian";
        private readonly string _url;
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public BankNorwegian(IConfiguration configuration)
        {
            _url = configuration["BankNorwegian:Url"];
            _apiKey = configuration["BankNorwegian:ApiKey"];

            if (string.IsNullOrEmpty(_url) || string.IsNullOrEmpty(_apiKey))
                throw new InvalidOperationException("Bank Norwegian URL or API Key is missing from configuration.");
            
            
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Transaction>> GetTransactions()
        {
            try
            {
                var response = await _httpClient.GetAsync(_url);
                LogRateLimitInfo(response);
                response.EnsureSuccessStatusCode();
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(await response.Content.ReadAsStringAsync(), _jsonOptions);

                if (apiResponse != null)
                    return ConvertToTransactions(apiResponse);
                
                Console.WriteLine("Failed to deserialize the API response.");
                return [];

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error fetching transactions: {e.Message}");
                return [];
            }
            catch (JsonException e)
            {
                Console.WriteLine($"Error deserializing JSON: {e.Message}");
                return [];
            }
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

        private List<Transaction> ConvertToTransactions(ApiResponse apiResponse)
        {
            return apiResponse.Transactions.Booked.Select(item => new Transaction
                {
                    Id = item.TransactionId,
                    Date = DateTime.Parse(item.BookingDate),
                    Amount = decimal.Parse(item.TransactionAmount.Amount),
                    Payee = item.CreditorName,
                    Category = item.AdditionalInformation
                })
                .ToList();
        }
    }
    

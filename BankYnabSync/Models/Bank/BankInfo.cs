
using System.Text.Json.Serialization;

namespace BankYnabSync.Models.Bank;

public class BankInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("bic")]
    public string Bic { get; set; }

    [JsonPropertyName("transaction_total_days")]
    public string TransactionTotalDays { get; set; }

    [JsonPropertyName("countries")]
    public List<string> Countries { get; set; }

    [JsonPropertyName("logo")]
    public string Logo { get; set; }
}
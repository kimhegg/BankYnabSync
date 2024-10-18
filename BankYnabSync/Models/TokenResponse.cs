using System.Text.Json.Serialization;

namespace BankYnabSync.Models;

public class TokenResponse
{
    [JsonPropertyName("access")]
    public string AccessToken { get; set; }

    [JsonPropertyName("access_expires")]
    public int AccessExpires { get; set; }
}
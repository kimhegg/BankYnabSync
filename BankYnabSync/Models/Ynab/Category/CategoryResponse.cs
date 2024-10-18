using System.Text.Json.Serialization;

namespace BankYnabSync.Models.Ynab.Category;

public class CategoryResponse
{
    [JsonPropertyName("data")]
    public CategoryData Data { get; set; }
}
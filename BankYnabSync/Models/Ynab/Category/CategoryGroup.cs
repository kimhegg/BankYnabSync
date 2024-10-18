using System.Text.Json.Serialization;

namespace BankYnabSync.Models.Ynab.Category;

public class CategoryGroup
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("hidden")]
    public bool Hidden { get; set; }

    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }

    [JsonPropertyName("categories")]
    public List<Category> Categories { get; set; }
}
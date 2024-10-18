using System.Text.Json.Serialization;

namespace BankYnabSync.Models.Ynab.Category;


    public class CategoryData
    {
        [JsonPropertyName("category_groups")]
        public List<CategoryGroup> CategoryGroups { get; set; }

        [JsonPropertyName("server_knowledge")]
        public int ServerKnowledge { get; set; }
    }

using System.Text.Json.Serialization;

namespace BankYnabSync.Models.Ynab.Category;

public class Category
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("category_group_id")]
    public string CategoryGroupId { get; set; }

    [JsonPropertyName("category_group_name")]
    public string CategoryGroupName { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("hidden")]
    public bool Hidden { get; set; }

    [JsonPropertyName("original_category_group_id")]
    public string OriginalCategoryGroupId { get; set; }

    [JsonPropertyName("note")]
    public string Note { get; set; }

    [JsonPropertyName("budgeted")]
    public long Budgeted { get; set; }

    [JsonPropertyName("activity")]
    public long Activity { get; set; }

    [JsonPropertyName("balance")]
    public long Balance { get; set; }

    [JsonPropertyName("goal_type")]
    public string GoalType { get; set; }

    [JsonPropertyName("goal_needs_whole_amount")]
    public bool? GoalNeedsWholeAmount { get; set; }

    [JsonPropertyName("goal_day")]
    public int? GoalDay { get; set; }

    [JsonPropertyName("goal_cadence")]
    public int? GoalCadence { get; set; }

    [JsonPropertyName("goal_cadence_frequency")]
    public int? GoalCadenceFrequency { get; set; }

    [JsonPropertyName("goal_creation_month")]
    public string GoalCreationMonth { get; set; }

    [JsonPropertyName("goal_target")]
    public long GoalTarget { get; set; }

    [JsonPropertyName("goal_target_month")]
    public string GoalTargetMonth { get; set; }

    [JsonPropertyName("goal_percentage_complete")]
    public int? GoalPercentageComplete { get; set; }

    [JsonPropertyName("goal_months_to_budget")]
    public int? GoalMonthsToBudget { get; set; }

    [JsonPropertyName("goal_under_funded")]
    public long? GoalUnderFunded { get; set; }

    [JsonPropertyName("goal_overall_funded")]
    public long? GoalOverallFunded { get; set; }

    [JsonPropertyName("goal_overall_left")]
    public long? GoalOverallLeft { get; set; }

    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }
}
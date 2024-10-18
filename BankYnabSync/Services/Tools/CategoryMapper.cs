namespace BankYnabSync.Services.Tools;

public static class CategoryMapper
{
    private static readonly Dictionary<string, string?> CategoryMappings = new()
    {
        { "Grocery Stores, Supermarkets", "\ud83c\udf54 Dagligvarer" },
        { "GROCERY STORES/SUPERMARKETS", "\ud83c\udf54 Dagligvarer" },
        { "FABRIC STORES", "Klær" },
        { "FAMILY CLOTHING STORES", "Klær" },
        { "HOME SUPPLY WAREHOUSE STORES", "Hus og hjem" },
        { "LUMBER/BUILD. SUPPLY STORES", "Hus og hjem" },
        { "RESTAURANTS", "\ud83c\udf7d\ufe0f Spise ute" },
        
        // Add more mappings as needed
    };

    private const string? DefaultCategory = "";

    public static string? MapCategory(string bankCategory)
    {
        var normalizedCategory = bankCategory.Trim();
        
        if (CategoryMappings.TryGetValue(normalizedCategory, out var mappedCategory))
            return mappedCategory;
        
        foreach (var mapping in CategoryMappings.Where(mapping => normalizedCategory.Contains(mapping.Key)))
            return mapping.Value;
        
        return DefaultCategory;
    }
}
using System.Text.Json;
using BankYnabSync.Models.Bank;
using BankYnabSync.Models.Repositories;
using BankYnabSync.Models.Services;
using BankYnabSync.Models.Ynab.Category;
using BankYnabSync.Services.Tools;
using Microsoft.Extensions.Configuration;

namespace BankYnabSync.Services;

public class YnabService(IConfiguration configuration, IYnabRepository repository) : IYnabService
{
    private readonly string _ynabTestBudget = configuration["Ynab:TestBudgetId"];
    
    public async Task InsertTransactionsFromBank(IBank bank)
    {
        var budgets = await repository.ListBudgets();
        var budget = budgets.FirstOrDefault();
        if (budget == null)
        {
            Console.WriteLine("No budgets found in YNAB.");
            return;
        }

        var accounts = await repository.ListAccounts(_ynabTestBudget);
        var account = accounts.FirstOrDefault();
        if (account == null)
        {
            Console.WriteLine("No accounts found in the selected budget.");
            return;
        }
        // Fetch transactions from the bank
        var bankTransactions = await bank.GetTransactions();

        // Fetch categories from YNAB
        var categoryResponse = await repository.GetCategories(_ynabTestBudget);
        var categoryNameToIdMap = BuildCategoryNameToIdMap(categoryResponse);

        // Convert bank transactions to YNAB format
        var ynabTransactions = bankTransactions.Select(t =>
        {
            var mappedCategory = t.Category != null ? CategoryMapper.MapCategory(t.Category) : "";
            string categoryId = null;
            if (categoryNameToIdMap.TryGetValue(mappedCategory, out var id))
                categoryId = id;
            else
                Console.WriteLine($"Warning: No matching category found for '{mappedCategory}'");


            return new
            {
                account_id = account.Id,
                date = t.Date.ToString("yyyy-MM-dd"),
                amount = (long)(t.Amount * 1000), // YNAB uses milliunits
                payee_name = t.Payee,
                category_id = categoryId,
                memo = t.Category, // Keep the original category in the memo field
                cleared = "cleared",
                approved = false,
                import_id = $"YNAB:{t.Id}" // Use a unique import_id to prevent duplicates
            };
        }).ToList();

        await repository.InsertTransactions(_ynabTestBudget, new { transactions = ynabTransactions });

        Console.WriteLine($"Successfully inserted {ynabTransactions.Count} transactions into YNAB.");
    }
    
    private Dictionary<string, string> BuildCategoryNameToIdMap(CategoryResponse categoryResponse)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (categoryResponse?.Data?.CategoryGroups == null)
            return map;


        foreach (var group in categoryResponse.Data.CategoryGroups)
        {
            if (group?.Categories == null) 
                continue;

            foreach (var category in group.Categories.OfType<Category>().Where(category => !string.IsNullOrWhiteSpace(category.Name) && !string.IsNullOrWhiteSpace(category.Id)))
                map[category.Name] = category.Id;

        }
        return map;
    }
}

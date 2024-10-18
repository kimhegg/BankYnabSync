namespace BankYnabSync.Models.Ynab.Budget;

public class Data
{
    public List<Budget> Budgets { get; set; }
    public object DefaultBudget { get; set; } // This is null in the example, so we'll use object
}
namespace BankYnabSync.Models.Ynab.Budget;

public class Budget
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime LastModifiedOn { get; set; }
    public string FirstMonth { get; set; }
    public string LastMonth { get; set; }
    public DateFormat DateFormat { get; set; }
    public CurrencyFormat CurrencyFormat { get; set; }
}
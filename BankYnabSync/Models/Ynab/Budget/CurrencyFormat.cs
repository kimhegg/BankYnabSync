namespace BankYnabSync.Models.Ynab.Budget;

public class CurrencyFormat
{
    public string IsoCode { get; set; }
    public string ExampleFormat { get; set; }
    public int DecimalDigits { get; set; }
    public string DecimalSeparator { get; set; }
    public bool SymbolFirst { get; set; }
    public string GroupSeparator { get; set; }
    public string CurrencySymbol { get; set; }
    public bool DisplaySymbol { get; set; }
}
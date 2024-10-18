namespace BankYnabSync.Models.Ynab.Account;

public class Account
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public bool OnBudget { get; set; }
    public bool Closed { get; set; }
    public string Note { get; set; }
    public long Balance { get; set; }
    public long ClearedBalance { get; set; }
    public long UnclearedBalance { get; set; }
    public string TransferPayeeId { get; set; }
    public bool DirectImportLinked { get; set; }
    public bool DirectImportInError { get; set; }
    public DateTime? LastReconciledAt { get; set; }
    public long? DebtOriginalBalance { get; set; }
    public Dictionary<string, int> DebtInterestRates { get; set; }
    public Dictionary<string, long> DebtMinimumPayments { get; set; }
    public Dictionary<string, long> DebtEscrowAmounts { get; set; }
    public bool Deleted { get; set; }
}
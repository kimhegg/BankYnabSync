namespace BankYnabSync.Models.Bank;

public class Transaction
{
    public required string Id { get; set; }
    public required DateTime Date { get; set; }
    public required decimal Amount { get; set; }
    public required string Payee { get; set; }
    public required string? Category { get; set; }
}
namespace BankYnabSync.Models;

public class BookedTransaction
{
    public required string TransactionId { get; set; }
    public required string BookingDate { get; set; }
    public required TransactionAmount TransactionAmount { get; set; }
    public string? CreditorName { get; set; }
    public  string? AdditionalInformation { get; set; }
}
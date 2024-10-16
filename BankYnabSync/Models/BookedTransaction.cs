namespace BankYnabSync.Models;

public class BookedTransaction
{
    public required string TransactionId { get; set; }
    public required string BookingDate { get; set; }
    public required TransactionAmount TransactionAmount { get; set; }
    public required string CreditorName { get; set; }
    public required string AdditionalInformation { get; set; }
}
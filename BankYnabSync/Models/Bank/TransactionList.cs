namespace BankYnabSync.Models.Bank;

public class TransactionList
{
    public required List<BookedTransaction> Booked { get; set; }
    public required List<PendingTransaction> Pending { get; set; }
}

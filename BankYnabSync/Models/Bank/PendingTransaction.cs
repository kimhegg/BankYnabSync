namespace BankYnabSync.Models.Bank;

public class PendingTransaction
{
    public required string TransactionId { get; set; }
    public required string ValueDate { get; set; }
    public required TransactionAmount TransactionAmount { get; set; }
    public required string RemittanceInformationUnstructured { get; set; }
    
}
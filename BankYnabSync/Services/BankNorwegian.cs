
using BankYnabSync.Models.Bank;
using BankYnabSync.Models.Repositories;


namespace BankYnabSync.Services;

public class BankNorwegian(IBankRepository bankRepository) : IBank
{
    public string Name => "Bank Norwegian";
    
    public async Task<List<Transaction>> GetTransactions()
    {
        return await bankRepository.GetTransactions();
    }
    
}


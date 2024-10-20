
using BankYnabSync.Models.Bank;
using BankYnabSync.Models.Repositories;


namespace BankYnabSync.Services;

public class BankService(IBankRepository bankRepository) : IBank
{
    
    public async Task<List<Transaction>> GetTransactions(string bankAccountPath)
    {
        return await bankRepository.GetTransactions(bankAccountPath);
    }
    
    public async Task<List<Transaction>> GetFakeTransactionsAsync()
    {
        return await bankRepository.GetFakeTransactionsAsync();
    }
    
    public async Task<List<BankInfo>> GetBanksAsync()
    {

        return await bankRepository.GetBanks();
      
    }
    
}


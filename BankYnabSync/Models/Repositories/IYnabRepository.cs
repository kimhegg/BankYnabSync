using BankYnabSync.Models.Ynab.Account;
using BankYnabSync.Models.Ynab.Budget;
using BankYnabSync.Models.Ynab.Category;

namespace BankYnabSync.Models.Repositories;

public interface IYnabRepository
{
    Task<CategoryResponse> GetCategories(string budgetId);
    Task<IEnumerable<Account>> ListAccounts(string budgetId);
    Task<IEnumerable<Budget>> ListBudgets();
    Task InsertTransactions(string budgetId, object transactions);
}
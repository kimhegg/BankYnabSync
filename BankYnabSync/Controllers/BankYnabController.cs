

using BankYnabSync.Models.Bank;
using BankYnabSync.Models.Repositories;
using BankYnabSync.Models.Services;
using BankYnabSync.Models.Ynab.Account;
using BankYnabSync.Models.Ynab.Budget;
using BankYnabSync.Models.Ynab.Category;
using Microsoft.AspNetCore.Mvc;

namespace BankYnabSync.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BankYnabController : ControllerBase
{
    private readonly IBankRepository _bankRepository;
    private readonly IYnabRepository _ynabRepository;
    private readonly ISyncService _syncService;

    public BankYnabController(IBankRepository bankRepository, IYnabRepository ynabRepository, ISyncService syncService)
    {
        _bankRepository = bankRepository;
        _ynabRepository = ynabRepository;
        _syncService = syncService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Sync()
    {
        try
        {
            await _syncService.SyncTransactions();
            return Ok("Sync completed successfully");
        }
        catch (Exception ex)
        {
            return Problem($"An error occurred: {ex.Message}");
        }
    }

    [HttpGet("banks")]
    public async Task<ActionResult<List<BankInfo>>> GetBanks()
    {
        return await _bankRepository.GetBanks();
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<List<Transaction>>> GetTransactions(string bankAccountPath)
    {
        return await _bankRepository.GetTransactions(bankAccountPath);
    }

    [HttpGet("ynab/budgets")]
    public async Task<ActionResult<IEnumerable<Budget>>> GetYnabBudgets()
    {
        return Ok(await _ynabRepository.ListBudgets());
    }

    [HttpGet("ynab/accounts")]
    public async Task<ActionResult<IEnumerable<Account>>> GetYnabAccounts(string budgetId)
    {
        return Ok(await _ynabRepository.ListAccounts(budgetId));
    }

    [HttpGet("ynab/categories")]
    public async Task<ActionResult<CategoryResponse>> GetYnabCategories(string budgetId)
    {
        return await _ynabRepository.GetCategories(budgetId);
    }

    [HttpPost("ynab/transactions")]
    public async Task<IActionResult> InsertYnabTransactions(string budgetId, [FromBody] object transactions)
    {
        await _ynabRepository.InsertTransactions(budgetId, transactions);
        return Ok();
    }
}
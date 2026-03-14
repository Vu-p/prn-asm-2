using models;
using repositories;

namespace services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repo;

    public AccountService(IAccountRepository repo) => _repo = repo;

    public Task<List<Account>> GetAllAsync(string? search) => _repo.GetAllAsync(search);
    public Task<Account?> GetByIdAsync(short id) => _repo.GetByIdAsync(id);
    public async Task<Account?> GetByEmailAsync(string email)
    {
        Console.WriteLine($"DEBUG: [AccountService] GetByEmailAsync called for {email}");
        var result = await _repo.GetByEmailAsync(email);
        Console.WriteLine($"DEBUG: [AccountService] GetByEmailAsync result: {result?.AccountEmail ?? "NULL"}");
        return result;
    }
    public Task AddAsync(Account account) => _repo.AddAsync(account);
    public async Task UpdateAsync(Account account) 
    {
        var existing = await _repo.GetByIdAsync(account.AccountId);
        if (existing == null) throw new InvalidOperationException("Account not found.");

        // Preserve password if not provided (e.g., from a partial update form)
        if (string.IsNullOrWhiteSpace(account.AccountPassword))
        {
            account.AccountPassword = existing.AccountPassword;
        }

        await _repo.UpdateAsync(account);
    }

    public Task DeleteAsync(short id) => _repo.DeleteAsync(id);

    public async Task<Account?> AuthenticateAsync(string email, string password)
    {
        var account = await _repo.GetByEmailAsync(email);
        if (account != null && account.AccountPassword == password)
        {
            return account;
        }
        return null;
    }
}

using models;
using repositories;

namespace services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repo;

    public AccountService(IAccountRepository repo) => _repo = repo;

    public Task<List<Account>> GetAllAsync(string? search) => _repo.GetAllAsync(search);
    public Task<Account?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
    public Task<Account?> GetByEmailAsync(string email) => _repo.GetByEmailAsync(email);
    public Task AddAsync(Account account) => _repo.AddAsync(account);
    public Task UpdateAsync(Account account) => _repo.UpdateAsync(account);
    public Task DeleteAsync(int id) => _repo.DeleteAsync(id);

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

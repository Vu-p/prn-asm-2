using models;

namespace services;

public interface IAccountService
{
    Task<Account?> GetByIdAsync(short id);
    Task<Account?> GetByEmailAsync(string email);
    Task<List<Account>> GetAllAsync(string? search);
    Task AddAsync(Account account);
    Task UpdateAsync(Account account);
    Task DeleteAsync(short id);
    Task<Account?> AuthenticateAsync(string email, string password);
}

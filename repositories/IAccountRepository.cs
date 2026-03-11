using models;

namespace repositories;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(int id);
    Task<Account?> GetByEmailAsync(string email);
    Task<List<Account>> GetAllAsync(string? search);
    Task AddAsync(Account account);
    Task UpdateAsync(Account account);
    Task DeleteAsync(int id);
}

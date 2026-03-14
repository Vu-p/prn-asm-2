using models;

namespace repositories;

public interface IAccountRepository : IBaseRepository<Account>
{
    Task<Account?> GetByEmailAsync(string email);
    Task<List<Account>> GetAllAsync(string? search);
}

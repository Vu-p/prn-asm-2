using Microsoft.EntityFrameworkCore;
using models;

namespace repositories;

public class AccountRepository : BaseRepository<Account>, IAccountRepository
{
    public AccountRepository(AppDbContext db) : base(db) { }

    public async Task<Account?> GetByEmailAsync(string email)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(a => a.AccountEmail == email);
    }

    public async Task<List<Account>> GetAllAsync(string? search)
    {
        IQueryable<Account> query = _dbSet;
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(a => a.AccountEmail.Contains(term) || a.AccountName.Contains(term));
        }

        return await query.OrderBy(a => a.AccountName).ToListAsync();
    }
}

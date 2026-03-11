using Microsoft.EntityFrameworkCore;
using models;

namespace repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _db;

    public AccountRepository(AppDbContext db) => _db = db;

    public Task<Account?> GetByIdAsync(int id) => 
        _db.Accounts.FirstOrDefaultAsync(a => a.AccountId == (short)id);

    public Task<Account?> GetByEmailAsync(string email) =>
        _db.Accounts.FirstOrDefaultAsync(a => a.AccountEmail == email);

    public async Task<List<Account>> GetAllAsync(string? search)
    {
        IQueryable<Account> query = _db.Accounts;
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(a => a.AccountEmail.Contains(term) || a.AccountName.Contains(term));
        }

        return await query.OrderBy(a => a.AccountName).ToListAsync();
    }

    public async Task AddAsync(Account account)
    {
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Account account)
    {
        _db.Accounts.Update(account);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.Accounts.FirstOrDefaultAsync(a => a.AccountId == (short)id);
        if (entity == null) return;
        _db.Accounts.Remove(entity);
        await _db.SaveChangesAsync();
    }
}

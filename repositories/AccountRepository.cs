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

    public override async Task DeleteAsync(object id)
    {
        if (id is not short accountId)
        {
            await base.DeleteAsync(id);
            return;
        }

        var entity = await _dbSet.FirstOrDefaultAsync(a => a.AccountId == accountId);
        if (entity == null) return;

        var relatedNews = await _db.NewsArticles
            .Where(n => n.CreatedById == accountId || n.UpdatedById == accountId)
            .ToListAsync();

        foreach (var article in relatedNews)
        {
            if (article.CreatedById == accountId) article.CreatedById = null;
            if (article.UpdatedById == accountId) article.UpdatedById = null;
        }

        _dbSet.Remove(entity);
        await _db.SaveChangesAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using models;

namespace repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext db) : base(db) { }

    public async Task<List<Category>> GetAllAsync(string? search)
    {
        IQueryable<Category> query = _dbSet.Include(c => c.ParentCategory);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c => c.CategoryName.Contains(term));
        }
        return await query.OrderBy(c => c.CategoryName).ToListAsync();
    }
}

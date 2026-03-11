using Microsoft.EntityFrameworkCore;
using models;

namespace repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db) => _db = db;

    public Task<Category?> GetByIdAsync(int id) => 
        _db.Categories.FirstOrDefaultAsync(c => c.CategoryId == (short)id);

    public async Task<List<Category>> GetAllAsync(string? search)
    {
        IQueryable<Category> query = _db.Categories;
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c => c.CategoryName.Contains(term) || c.CategoryDesciption.Contains(term));
        }

        return await query.OrderBy(c => c.CategoryName).ToListAsync();
    }

    public async Task AddAsync(Category category)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _db.Categories.Update(category);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.Categories
            .Include(c => c.NewsArticles)
            .FirstOrDefaultAsync(c => c.CategoryId == (short)id);
        
        if (entity == null) return;

        if (entity.NewsArticles.Any())
        {
            throw new InvalidOperationException("Cannot delete category as it is associated with news articles.");
        }

        _db.Categories.Remove(entity);
        await _db.SaveChangesAsync();
    }
}

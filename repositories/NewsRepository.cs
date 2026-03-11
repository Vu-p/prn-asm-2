using Microsoft.EntityFrameworkCore;
using models;

namespace repositories;

public class NewsRepository : INewsRepository
{
    private readonly AppDbContext _db;

    public NewsRepository(AppDbContext db) => _db = db;

    public Task<NewsArticle?> GetByIdAsync(string id) =>
        _db.NewsArticles.FirstOrDefaultAsync(n => n.NewsArticleId == id);

    public Task<NewsArticle?> GetByIdWithDetailsAsync(string id) =>
        _db.NewsArticles
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
            .FirstOrDefaultAsync(n => n.NewsArticleId == id);

    public Task<List<NewsArticle>> GetAllAsync() =>
        _db.NewsArticles
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();

    public Task<List<NewsArticle>> GetActiveNewsAsync() =>
        _db.NewsArticles
            .Include(n => n.Category)
            .Where(n => n.NewsStatus == 1)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();

    public Task<List<NewsArticle>> GetHistoryByStaffAsync(short staffId) =>
        _db.NewsArticles
            .Include(n => n.Category)
            .Where(n => n.CreatedById == staffId)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();

    public Task<List<NewsArticle>> SearchAsync(string? term)
    {
        IQueryable<NewsArticle> query = _db.NewsArticles.Include(n => n.Category);
        if (!string.IsNullOrWhiteSpace(term))
        {
            var t = term.Trim();
            query = query.Where(n => n.NewsTitle.Contains(t) || n.Headline.Contains(t));
        }
        return query.OrderByDescending(n => n.CreatedDate).ToListAsync();
    }

    public async Task AddAsync(NewsArticle article, List<int> tagIds)
    {
        _db.NewsArticles.Add(article);
        foreach (var tagId in tagIds)
        {
            _db.NewsTags.Add(new NewsTag { NewsArticleId = article.NewsArticleId, TagId = tagId });
        }
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(NewsArticle article, List<int> tagIds)
    {
        var existing = await _db.NewsArticles
            .Include(n => n.NewsTags)
            .FirstOrDefaultAsync(n => n.NewsArticleId == article.NewsArticleId);

        if (existing == null) return;

        _db.Entry(existing).CurrentValues.SetValues(article);
        
        // Update Tags
        _db.NewsTags.RemoveRange(existing.NewsTags);
        foreach (var tagId in tagIds)
        {
            _db.NewsTags.Add(new NewsTag { NewsArticleId = article.NewsArticleId, TagId = tagId });
        }

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(string id)
    {
        var entity = await _db.NewsArticles.FirstOrDefaultAsync(n => n.NewsArticleId == id);
        if (entity == null) return;
        _db.NewsArticles.Remove(entity);
        await _db.SaveChangesAsync();
    }

    public async Task<List<NewsArticle>> ReportAsync(DateTime startDate, DateTime endDate)
    {
        return await _db.NewsArticles
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }
}

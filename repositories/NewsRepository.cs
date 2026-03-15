using Microsoft.EntityFrameworkCore;
using models;

namespace repositories;

public class NewsRepository : BaseRepository<NewsArticle>, INewsRepository
{
    public NewsRepository(AppDbContext db) : base(db) { }

    public Task<NewsArticle?> GetByIdWithDetailsAsync(string id) =>
        _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
            .FirstOrDefaultAsync(n => n.NewsArticleId == id);

    public override Task<List<NewsArticle>> GetAllAsync() =>
        _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();

    public Task<List<NewsArticle>> GetActiveNewsAsync() =>
        _dbSet
            .Include(n => n.Category)
            .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
            .Where(n => n.NewsStatus == 1)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();

    public Task<List<NewsArticle>> GetHistoryByStaffAsync(short staffId) =>
        _dbSet
            .Include(n => n.Category)
            .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
            .Where(n => n.CreatedById == staffId)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();

    public Task<List<NewsArticle>> SearchAsync(string? term)
    {
        IQueryable<NewsArticle> query = _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag);
            
        if (!string.IsNullOrWhiteSpace(term))
        {
            var t = term.Trim();
            query = query.Where(n => n.NewsTitle.Contains(t) || n.Headline.Contains(t) || n.NewsContent.Contains(t));
        }
        return query.OrderByDescending(n => n.CreatedDate).ToListAsync();
    }

    public async Task AddAsync(NewsArticle article, List<int> tagIds)
    {
        _dbSet.Add(article);
        foreach (var tagId in tagIds)
        {
            _db.NewsTags.Add(new NewsTag { NewsArticleId = article.NewsArticleId, TagId = tagId });
        }
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(NewsArticle article, List<int> tagIds)
    {
        var existing = await _dbSet
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

    public Task<List<NewsArticle>> ReportAsync(DateTime startDate, DateTime endDate)
    {
        return _dbSet
            .Include(n => n.Category)
            .Include(n => n.CreatedBy)
            .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public Task<List<NewsArticle>> GetRelatedNewsAsync(string articleId, short categoryId, int count)
    {
        return _dbSet
            .Include(n => n.Category)
            .Include(n => n.NewsTags).ThenInclude(nt => nt.Tag)
            .Where(n => n.NewsArticleId != articleId && n.NewsStatus == 1)
            .Where(n => n.CategoryId == categoryId)
            .OrderByDescending(n => n.CreatedDate)
            .Take(count)
            .ToListAsync();
    }
}

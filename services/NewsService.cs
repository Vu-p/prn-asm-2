using models;
using repositories;

namespace services;

public class NewsService : INewsService
{
    private readonly INewsRepository _repo;

    public NewsService(INewsRepository repo) => _repo = repo;

    public Task<NewsArticle?> GetByIdAsync(string id) => _repo.GetByIdAsync(id);
    public Task<NewsArticle?> GetByIdWithDetailsAsync(string id) => _repo.GetByIdWithDetailsAsync(id);
    public Task<List<NewsArticle>> GetAllAsync() => _repo.GetAllAsync();
    public Task<List<NewsArticle>> GetActiveNewsAsync() => _repo.GetActiveNewsAsync();
    public Task<List<NewsArticle>> GetHistoryByStaffAsync(short staffId) => _repo.GetHistoryByStaffAsync(staffId);
    public Task<List<NewsArticle>> SearchAsync(string? term) => _repo.SearchAsync(term);

    public async Task AddAsync(NewsArticle article, List<int> tagIds, short userId, bool isAdmin)
    {
        article.CreatedDate = DateTime.UtcNow;
        article.CreatedById = userId == 0 ? null : userId;
        
        // Business Rule: Non-admins cannot approve articles
        if (!isAdmin)
        {
            article.NewsStatus = (byte)NewsStatus.Draft;
        }

        await _repo.AddAsync(article, tagIds);
    }

    public async Task UpdateAsync(NewsArticle article, List<int> tagIds, short userId, bool isAdmin)
    {
        var existing = await _repo.GetByIdAsync(article.NewsArticleId);
        if (existing == null) throw new InvalidOperationException("Article not found.");

        // Permission check: Staff/Lecturer cannot edit approved articles or others' articles
        if (!isAdmin)
        {
            if (existing.NewsStatus == (byte)NewsStatus.Approved || existing.CreatedById != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to edit this article.");
            }
            // Ensure they can't sneak an "Approved" status in
            article.NewsStatus = (byte)NewsStatus.Draft;
        }

        article.CreatedById = existing.CreatedById;
        article.CreatedDate = existing.CreatedDate;
        
        if (string.IsNullOrEmpty(article.ThumbnailUrl))
        {
            article.ThumbnailUrl = existing.ThumbnailUrl;
        }

        article.UpdatedById = userId == 0 ? null : userId;
        article.ModifiedDate = DateTime.UtcNow;

        await _repo.UpdateAsync(article, tagIds);
    }

    public async Task DeleteAsync(string id, short userId, bool isAdmin)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return;

        // Permission check
        if (!isAdmin)
        {
            if (existing.NewsStatus == (byte)NewsStatus.Approved || existing.CreatedById != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this article.");
            }
        }

        await _repo.DeleteAsync(id);
    }

    public Task<List<NewsArticle>> ReportAsync(DateTime startDate, DateTime endDate) => 
        _repo.ReportAsync(startDate, endDate);

    public async Task<List<NewsArticle>> GetRelatedNewsAsync(string articleId, int count)
    {
        var article = await _repo.GetByIdWithDetailsAsync(articleId);
        if (article == null || !article.NewsTags.Any()) return new List<NewsArticle>();

        var tagIds = article.NewsTags.Select(nt => nt.TagId).ToList();
        return await _repo.GetRelatedNewsAsync(articleId, tagIds, count);
    }
}

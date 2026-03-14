using models;

namespace services;

public interface INewsService
{
    Task<NewsArticle?> GetByIdAsync(string id);
    Task<NewsArticle?> GetByIdWithDetailsAsync(string id);
    Task<List<NewsArticle>> GetAllAsync();
    Task<List<NewsArticle>> GetActiveNewsAsync();
    Task<List<NewsArticle>> GetHistoryByStaffAsync(short staffId);
    Task<List<NewsArticle>> SearchAsync(string? term);
    
    // Permission-aware methods
    Task AddAsync(NewsArticle article, List<int> tagIds, short userId, bool isAdmin);
    Task UpdateAsync(NewsArticle article, List<int> tagIds, short userId, bool isAdmin);
    Task DeleteAsync(string id, short userId, bool isAdmin);
    
    Task<List<NewsArticle>> ReportAsync(DateTime startDate, DateTime endDate);
}

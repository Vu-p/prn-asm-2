using models;

namespace repositories;

public interface INewsRepository
{
    Task<NewsArticle?> GetByIdAsync(string id);
    Task<NewsArticle?> GetByIdWithDetailsAsync(string id);
    Task<List<NewsArticle>> GetAllAsync();
    Task<List<NewsArticle>> GetActiveNewsAsync();
    Task<List<NewsArticle>> GetHistoryByStaffAsync(short staffId);
    Task<List<NewsArticle>> SearchAsync(string? term);
    Task AddAsync(NewsArticle article, List<int> tagIds);
    Task UpdateAsync(NewsArticle article, List<int> tagIds);
    Task DeleteAsync(string id);
    Task<List<NewsArticle>> ReportAsync(DateTime startDate, DateTime endDate);
}

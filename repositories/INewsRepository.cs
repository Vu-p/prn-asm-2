using models;

namespace repositories;

public interface INewsRepository : IBaseRepository<NewsArticle>
{
    Task<NewsArticle?> GetByIdWithDetailsAsync(string id);
    Task<List<NewsArticle>> GetActiveNewsAsync();
    Task<List<NewsArticle>> GetHistoryByStaffAsync(short staffId);
    Task<List<NewsArticle>> SearchAsync(string? term);
    Task AddAsync(NewsArticle article, List<int> tagIds);
    Task UpdateAsync(NewsArticle article, List<int> tagIds);
    Task<List<NewsArticle>> ReportAsync(DateTime startDate, DateTime endDate);
    Task<List<NewsArticle>> GetRelatedNewsAsync(string articleId, short categoryId, int count);
}

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
    public Task AddAsync(NewsArticle article, List<int> tagIds) => _repo.AddAsync(article, tagIds);
    public Task UpdateAsync(NewsArticle article, List<int> tagIds) => _repo.UpdateAsync(article, tagIds);
    public Task DeleteAsync(string id) => _repo.DeleteAsync(id);
    public Task<List<NewsArticle>> ReportAsync(DateTime startDate, DateTime endDate) => _repo.ReportAsync(startDate, endDate);
}

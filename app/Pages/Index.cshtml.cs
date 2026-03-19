using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;

namespace app.Pages
{
    public class IndexModel : PageModel
    {
        private readonly INewsService _newsService;
        private readonly IAccountService _accountService;
        private readonly ICategoryService _categoryService;

        public IndexModel(INewsService newsService, IAccountService accountService, ICategoryService categoryService)
        {
            _newsService = newsService;
            _accountService = accountService;
            _categoryService = categoryService;
        }

        public List<NewsArticle> LatestNews { get; set; } = new();
        public int TotalArticles { get; set; }
        public int TotalUsers { get; set; }
        public int TotalCategories { get; set; }

        public async Task OnGetAsync()
        {
            var activeNews = await _newsService.GetActiveNewsAsync();
            var allAccounts = await _accountService.GetAllAsync(null);
            var allCategories = await _categoryService.GetAllAsync(null);

            TotalArticles = activeNews.Count;
            TotalUsers = allAccounts.Count;
            TotalCategories = allCategories.Count(c => c.IsActive);

            LatestNews = activeNews
                .OrderByDescending(n => n.CreatedDate)
                .Take(4)
                .ToList();
        }
    }
}

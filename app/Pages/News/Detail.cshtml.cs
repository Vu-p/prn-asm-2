using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;

namespace app.Pages.News;

public class DetailModel : PageModel
{
    private readonly INewsService _newsService;
    private readonly ICategoryService _categoryService;

    public DetailModel(INewsService newsService, ICategoryService categoryService)
    {
        _newsService = newsService;
        _categoryService = categoryService;
    }

    public NewsArticle? Article { get; set; }
    public List<Category> CategoryHierarchy { get; set; } = new();
    public List<Category> AllCategories { get; set; } = new();
    public List<NewsArticle> RelatedNews { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        Article = await _newsService.GetByIdWithDetailsAsync(id);
        if (Article == null || Article.NewsStatus != 1)
            return NotFound();

        CategoryHierarchy = await _categoryService.GetCategoryHierarchyAsync(Article.CategoryId);
        AllCategories = await _categoryService.GetAllAsync(null);
        RelatedNews = await _newsService.GetRelatedNewsAsync(id, 4);

        return Page();
    }
}

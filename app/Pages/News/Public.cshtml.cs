using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;

namespace app.Pages.News;

public class PublicModel : PageModel
{
    private readonly INewsService _newsService;
    private readonly ICategoryService _categoryService;

    public PublicModel(INewsService newsService, ICategoryService categoryService)
    {
        _newsService = newsService;
        _categoryService = categoryService;
    }

    public List<NewsArticle> Articles { get; set; } = new();
    public List<Category> Categories { get; set; } = new();

    [BindProperty(SupportsGet = true, Name = "categoryId")]
    public short? SelectedCategoryId { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Categories = await _categoryService.GetAllAsync(null);
        var all = await _newsService.GetActiveNewsAsync();

        Articles = SelectedCategoryId.HasValue
            ? all.Where(a => a.CategoryId == SelectedCategoryId.Value).ToList()
            : all;

        return Page();
    }
}

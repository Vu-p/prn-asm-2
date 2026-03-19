using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;
using System.Text.Json;

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
    public List<Category> CategoryHierarchy { get; set; } = new();
    public HashSet<short> SelectedCategoryIds { get; set; } = new();
    public string SelectedCategoryIdsJson { get; set; } = "[]";

    [BindProperty(SupportsGet = true, Name = "categoryId")]
    public short? SelectedCategoryId { get; set; }

    [BindProperty(SupportsGet = true, Name = "searchTerm")]
    public string? SearchTerm { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Categories = await _categoryService.GetAllAsync(null);
        var all = await _newsService.GetActiveNewsAsync();

        if (SelectedCategoryId.HasValue)
        {
            SelectedCategoryIds = CollectCategoryTreeIds(SelectedCategoryId.Value);
            Articles = all.Where(a => SelectedCategoryIds.Contains(a.CategoryId)).ToList();
            CategoryHierarchy = await _categoryService.GetCategoryHierarchyAsync(SelectedCategoryId.Value);
        }
        else
        {
            Articles = all;
        }

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            var term = SearchTerm.Trim();
            Articles = Articles
                .Where(a =>
                    a.NewsTitle.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    a.Headline.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    a.NewsContent.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        SelectedCategoryIdsJson = JsonSerializer.Serialize(SelectedCategoryIds);

        return Page();
    }

    private HashSet<short> CollectCategoryTreeIds(short rootCategoryId)
    {
        var collected = new HashSet<short> { rootCategoryId };
        var queue = new Queue<short>();
        queue.Enqueue(rootCategoryId);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var children = Categories
                .Where(c => c.IsActive && c.ParentCategoryId == current)
                .Select(c => c.CategoryId);

            foreach (var childId in children)
            {
                if (collected.Add(childId))
                {
                    queue.Enqueue(childId);
                }
            }
        }

        return collected;
    }
}

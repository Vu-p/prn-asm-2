using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;

namespace app.Pages.News;

public class DetailModel : PageModel
{
    private readonly INewsService _newsService;

    public DetailModel(INewsService newsService)
    {
        _newsService = newsService;
    }

    public NewsArticle? Article { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return NotFound();

        Article = await _newsService.GetByIdWithDetailsAsync(id);
        if (Article == null || Article.NewsStatus != 1)
            return NotFound();

        return Page();
    }
}

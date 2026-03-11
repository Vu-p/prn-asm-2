using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;

namespace app.Pages.News;

public class PublicModel : PageModel
{
    private readonly INewsService _newsService;

    public PublicModel(INewsService newsService)
    {
        _newsService = newsService;
    }

    public List<NewsArticle> Articles { get; set; } = new();

    public async Task OnGetAsync()
    {
        Articles = await _newsService.GetActiveNewsAsync();
    }
}

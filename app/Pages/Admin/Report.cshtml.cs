using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;

namespace app.Pages.Admin;

[Authorize(Roles = "Admin")]
public class ReportModel : PageModel
{
    private readonly INewsService _newsService;

    public ReportModel(INewsService newsService)
    {
        _newsService = newsService;
    }

    public List<NewsArticle> ReportData { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public DateTime StartDate { get; set; } = DateTime.Now.AddMonths(-1);

    [BindProperty(SupportsGet = true)]
    public DateTime EndDate { get; set; } = DateTime.Now;

    public async Task OnGetAsync()
    {
        // Adjust EndDate to include the full day
        var end = EndDate.Date.AddDays(1).AddTicks(-1);
        ReportData = await _newsService.ReportAsync(StartDate.Date, end);
    }
}

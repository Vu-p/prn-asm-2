using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;
using System.Security.Claims;

namespace app.Pages.Staff;

[Authorize(Roles = "Staff,Admin")]
public class MyNewsModel : PageModel
{
    private readonly INewsService _newsService;

    public MyNewsModel(INewsService newsService)
    {
        _newsService = newsService;
    }

    public List<NewsArticle> Articles { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !short.TryParse(userIdClaim, out short userId))
        {
            return RedirectToPage("/Auth/Login");
        }

        Articles = await _newsService.GetHistoryByStaffAsync(userId);
        return Page();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;

namespace app.Pages.Staff.Categories;

[Authorize(Roles = "Staff,Admin")]
public class IndexModel : PageModel
{
    private readonly ICategoryService _categoryService;

    public IndexModel(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public List<Category> Categories { get; set; } = new();

    [BindProperty]
    public Category UpsertCategory { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync()
    {
        Categories = await _categoryService.GetAllAsync(SearchTerm);
    }

    public async Task<IActionResult> OnPostUpsertAsync()
    {
        if (!ModelState.IsValid)
        {
            Categories = await _categoryService.GetAllAsync(null);
            return Page();
        }

        if (UpsertCategory.CategoryId == 0)
        {
            await _categoryService.AddAsync(UpsertCategory);
        }
        else
        {
            await _categoryService.UpdateAsync(UpsertCategory);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        try
        {
            await _categoryService.DeleteAsync(id);
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage();
    }
}

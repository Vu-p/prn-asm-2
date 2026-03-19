using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;
using System.Security.Claims;

namespace app.Pages.Admin.Accounts;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IAccountService _accountService;

    public IndexModel(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public List<Account> Accounts { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty]
    public Account UpsertAccount { get; set; } = new();

    public async Task OnGetAsync()
    {
        Accounts = await _accountService.GetAllAsync(SearchTerm);
    }

    public async Task<IActionResult> OnPostUpsertAsync()
    {
        if (!ModelState.IsValid)
        {
            Accounts = await _accountService.GetAllAsync(SearchTerm);
            return Page();
        }

        if (UpsertAccount.AccountId == 0)
        {
            await _accountService.AddAsync(UpsertAccount);
        }
        else
        {
            await _accountService.UpdateAsync(UpsertAccount);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(short id)
    {
        var currentUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (short.TryParse(currentUserIdClaim, out var currentUserId) && currentUserId == id)
        {
            TempData["ErrorMessage"] = "Bạn không thể xóa tài khoản đang đăng nhập.";
            return RedirectToPage();
        }

        try
        {
            await _accountService.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }
}

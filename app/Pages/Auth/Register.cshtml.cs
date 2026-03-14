using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using models;
using services;

namespace app.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly IAccountService _accounts;

    public RegisterModel(IAccountService accounts)
    {
        _accounts = accounts;
    }

    [BindProperty]
    public RegisterInputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class RegisterInputModel
    {
        [Required, StringLength(100)]
        public string AccountName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string AccountEmail { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string AccountPassword { get; set; } = string.Empty;

        [Required]
        public int AccountRole { get; set; }
    }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existingAccount = await _accounts.GetByEmailAsync(Input.AccountEmail);
        if (existingAccount != null)
        {
            ErrorMessage = "Email is already in use.";
            return Page();
        }

        var newAccount = new Account
        {
            AccountName = Input.AccountName,
            AccountEmail = Input.AccountEmail,
            AccountPassword = Input.AccountPassword,
            AccountRole = Input.AccountRole
        };

        await _accounts.AddAsync(newAccount);

        return RedirectToPage("/Auth/Login");
    }
}

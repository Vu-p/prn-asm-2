using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace app.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (!User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Auth/Login");
            }

            if (User.IsInRole("Admin"))
            {
                return RedirectToPage("/Admin/Accounts/Index");
            }
            else if (User.IsInRole("Staff"))
            {
                return RedirectToPage("/Staff/News/Index");
            }
            else
            {
                return RedirectToPage("/News/Public");
            }
        }
    }
}

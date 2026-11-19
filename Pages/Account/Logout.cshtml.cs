using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Restaurant_Management_System.Accounts
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            HttpContext.Session.Clear();   // forget the user
            return RedirectToPage("/Index");
        }
    }
}

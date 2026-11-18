using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant_Management_System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Restaurant_Management_System.Accounts
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public LoginModel(ApplicationDbContext db) { _db = db; }

        [BindProperty] public string Email { get; set; } = "";
        [BindProperty] public string Password { get; set; } = "";
        public string? ErrorMessage { get; set; }

        public void OnGet() { }
        
            public async Task<IActionResult> OnPostAsync()
        {
            // 1. Hash the typed password the same way SQL did
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Password)));

            // 2. Find the user
            var user = await _db.UsersR.FirstOrDefaultAsync(u => u.Email == Email && u.PasswordHash == hash);
            if (user == null)
            {
                ErrorMessage = "Wrong email or password.";
                return Page();
            }

            // 3. Remember them in the Session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToPage("/Index");
        }

    }
}


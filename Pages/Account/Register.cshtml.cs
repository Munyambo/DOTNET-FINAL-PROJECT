using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant_Management_System.Data;
using Restaurant_Management_System.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Restaurant_Management_System.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public RegisterModel(ApplicationDbContext db) { _db = db; }

        // The form fields
        [BindProperty, Required, Display(Name = "Full name")]
        public string FullName { get; set; } = "";

        [BindProperty, Required, EmailAddress]
        public string Email { get; set; } = "";

        [BindProperty]
        public string? Phone { get; set; }

        [BindProperty]
        public string? Address { get; set; }

        [BindProperty, Required, MinLength(6)]
        public string Password { get; set; } = "";

        [BindProperty, Required, Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = "";

        public void OnGet() { }
        
             public async Task<IActionResult> OnPostAsync()
        {
            // 1. Check the form rules (Required, EmailAddress, Compare...)
            if (!ModelState.IsValid) return Page();

            // 2. Is this email already used?
            if (await _db.UsersR.AnyAsync(u => u.Email == Email))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return Page();
            }

            // 3. Save the login in UsersR with role Customer
            var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Password)));
            var user = new UsersR { Email = Email, PasswordHash = hash, Role = "Customer" };
            _db.UsersR.Add(user);
            await _db.SaveChangesAsync();          // now user.Id has its new number

            // 4. Save the customer profile, linked to that user
            _db.Customers.Add(new Customer
            {
                FullName = FullName,
                Phone = Phone,
                Address = Address,
                UserId = user.Id
            });
            await _db.SaveChangesAsync();

            // 5. Log them in straight away
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("Name", FullName);

            return RedirectToPage("/Index");
        }
    }
}
 

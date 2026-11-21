using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant_Management_System.Data;
using Restaurant_Management_System.Models;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_Management_System.Pages.CustomerArea
{
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public ProfileModel(ApplicationDbContext db) { _db = db; }

        // Fields the customer can edit
        [BindProperty, Required, Display(Name = "Full name")]
        public string FullName { get; set; } = "";

        [BindProperty]
        public string? Phone { get; set; }

        [BindProperty]
        public string? Address { get; set; }

        // Shown but NOT editable
        public string Email { get; set; } = "";

        // Green "saved" message that survives the redirect
        [TempData]
        public string? Message { get; set; }

        // Page opens → fill the form with the customer's current data
        public IActionResult OnGet()
        {
            var customer = FindCustomer();
            if (customer == null) return RedirectToPage("/Account/Login");

            FullName = customer.FullName;
            Phone = customer.Phone;
            Address = customer.Address;
            Email = HttpContext.Session.GetString("Email") ?? "";
            return Page();
        }

        // Save button clicked → update the customer's row
        public IActionResult OnPost()
        {
            Email = HttpContext.Session.GetString("Email") ?? "";
            if (!ModelState.IsValid) return Page();

            var customer = FindCustomer();
            if (customer == null) return RedirectToPage("/Account/Login");

            customer.FullName = FullName;
            customer.Phone = Phone;
            customer.Address = Address;
            _db.SaveChanges();                      // UPDATE Customers SET ...

            HttpContext.Session.SetString("Name", FullName);   // navbar shows the new name
            Message = "Your profile has been saved.";
            return RedirectToPage();
        }

        // Find the customer linked to the logged-in user
        private Customer? FindCustomer()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            return _db.Customers.FirstOrDefault(c => c.UserId == userId);
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.StaffPages;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Staff Staff { get; set; } = default!;

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; } = "";

    [BindProperty, Required, MinLength(6)]
    public string Password { get; set; } = "";

    public SelectList Branches { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Load the dropdown first, so every "return Page()" below has it
        Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");

        // 1. Check the form rules
        if (!ModelState.IsValid) return Page();

        // 2. Is this email already used?
        if (await _context.UsersR.AnyAsync(u => u.Email == Email))
        {
            ModelState.AddModelError("Email", "This email is already registered.");
            return Page();
        }

        // 3. Save the login in UsersR with role Staff
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Password)));
        var user = new UsersR { Email = Email, PasswordHash = hash, Role = "Staff" };
        _context.UsersR.Add(user);
        await _context.SaveChangesAsync();   // now user.Id has its new number

        // 4. Save the staff profile, linked to that user
        Staff.UserId = user.Id;
        _context.Staffs.Add(Staff);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
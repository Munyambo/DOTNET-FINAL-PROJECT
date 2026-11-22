using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.StaffPages;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Staff Staff { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var staff = await _context.Staffs.Include(s => s.Branch).FirstOrDefaultAsync(m => m.Id == id);
        if (staff is null)
        {
            return NotFound();
        }
        else
        {
            Staff = staff;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var staff = await _context.Staffs.FindAsync(id);
        if (staff != null)
        {
            // Remove the login too, so this person can no longer sign in
            if (staff.UserId != null)
            {
                var user = await _context.UsersR.FindAsync(staff.UserId.Value);
                if (user != null)
                {
                    _context.UsersR.Remove(user);
                }
            }

            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
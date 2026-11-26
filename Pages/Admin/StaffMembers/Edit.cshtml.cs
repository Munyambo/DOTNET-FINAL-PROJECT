using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.StaffPages;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Staff Staff { get; set; } = default!;

    public SelectList Branches { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var staff = await _context.Staffs.FirstOrDefaultAsync(m => m.Id == id);
        if (staff is null)
        {
            return NotFound();
        }
        Staff = staff;

        Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // Load the dropdown first, so every "return Page()" below has it
        Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Staff).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StaffExists(Staff.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool StaffExists(int id)
    {
        return _context.Staffs.Any(e => e.Id == id);
    }
}
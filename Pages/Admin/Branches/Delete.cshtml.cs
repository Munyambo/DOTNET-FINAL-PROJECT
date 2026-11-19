using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.BranchPages;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Branch Branch { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var branch = await _context.Branches.FirstOrDefaultAsync(m => m.Id == id);
        if (branch is null)
        {
            return NotFound();
        }
        else
        {
            Branch = branch;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var branch = await _context.Branches.FindAsync(id);
        if (branch != null)
        {
            Branch = branch;
            _context.Branches.Remove(Branch);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}

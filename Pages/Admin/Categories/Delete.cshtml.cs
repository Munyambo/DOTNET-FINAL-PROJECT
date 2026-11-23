using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.CategoryPages;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Category Category { get; set; } = default!;

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
        if (category is null)
        {
            return NotFound();
        }
        else
        {
            Category = category;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            Category = category;

            // A category that still has menu items cannot be deleted
            if (await _context.MenuItems.AnyAsync(m => m.CategoryId == category.Id))
            {
                ErrorMessage = "This category still has menu items. Move or delete them first.";
                return Page();
            }

            _context.Categories.Remove(Category);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}

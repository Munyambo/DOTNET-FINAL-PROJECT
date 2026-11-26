using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.MenuItemPages;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public MenuItem MenuItem { get; set; } = default!;

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var menuitem = await _context.MenuItems.Include(m => m.Category).FirstOrDefaultAsync(m => m.Id == id);
        if (menuitem is null)
        {
            return NotFound();
        }
        else
        {
            MenuItem = menuitem;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var menuitem = await _context.MenuItems.Include(m => m.Category).FirstOrDefaultAsync(m => m.Id == id);
        if (menuitem != null)
        {
            MenuItem = menuitem;

            // An item that was already ordered cannot be deleted (it is kept in past orders)
            if (await _context.OrderItems.AnyAsync(o => o.MenuItemId == menuitem.Id))
            {
                ErrorMessage = "This item is part of existing orders, so it cannot be deleted. Edit it and untick Available instead.";
                return Page();
            }

            _context.MenuItems.Remove(MenuItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}

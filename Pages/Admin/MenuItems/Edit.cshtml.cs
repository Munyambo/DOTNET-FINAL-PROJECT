using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.MenuItemPages;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public MenuItem MenuItem { get; set; } = default!;

    public SelectList Categories { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var menuitem = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == id);
        if (menuitem is null)
        {
            return NotFound();
        }
        MenuItem = menuitem;
        Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");

        // The Category object is not part of the form, so don't validate it
        ModelState.Remove("MenuItem.Category");

        if (MenuItem.CategoryId == 0 || !await _context.Categories.AnyAsync(c => c.Id == MenuItem.CategoryId))
        {
            ModelState.AddModelError("MenuItem.CategoryId", "Please select a category.");
        }
        if (MenuItem.Price < 0)
        {
            ModelState.AddModelError("MenuItem.Price", "Price cannot be negative.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(MenuItem).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MenuItemExists(MenuItem.Id))
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

    private bool MenuItemExists(int id)
    {
        return _context.MenuItems.Any(e => e.Id == id);
    }
}

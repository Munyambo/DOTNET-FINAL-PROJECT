using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.MenuItemPages;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public MenuItem MenuItem { get; set; } = default!;

    public SelectList Categories { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
        MenuItem = new MenuItem();   // so "Available" is ticked by default
        return Page();
    }

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        // Load the dropdown first, so every "return Page()" below has it
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

        _context.MenuItems.Add(MenuItem);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}

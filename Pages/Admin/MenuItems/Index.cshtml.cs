using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.MenuItemPages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<MenuItem> MenuItem { get; set; } = default!;

    public async Task OnGetAsync()
    {
        MenuItem = await _context.MenuItems.Include(m => m.Category).ToListAsync();
    }
}

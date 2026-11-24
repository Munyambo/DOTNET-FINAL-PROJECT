using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Data;
using Restaurant_Management_System.Helpers;
using Restaurant_Management_System.Models;

namespace Restaurant_Management_System.Pages.CustomerArea;

public class MenuModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public MenuModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Available food, grouped by category name (e.g. "Drinks" -> [Cola, Water])
    public List<IGrouping<string, MenuItem>> Groups { get; set; } = new();

    public int CartCount { get; set; }

    [TempData]
    public string? Message { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostAddAsync(int menuItemId, int quantity)
    {
        // Only available items can be added
        var item = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == menuItemId && m.IsAvailable);
        if (item == null)
        {
            Message = "Sorry, that item is not available.";
            return RedirectToPage();
        }

        if (quantity < 1) quantity = 1;
        if (quantity > 50) quantity = 50;

        // If the item is already in the cart, add to its quantity; otherwise add a new line
        var cart = CartHelper.GetCart(HttpContext.Session);
        var line = cart.FirstOrDefault(c => c.MenuItemId == menuItemId);
        if (line == null)
            cart.Add(new CartItem { MenuItemId = menuItemId, Quantity = quantity });
        else
            line.Quantity = Math.Min(line.Quantity + quantity, 50);

        CartHelper.SaveCart(HttpContext.Session, cart);

        Message = $"{item.Name} added to your cart.";
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var items = await _context.MenuItems
            .Include(m => m.Category)
            .Where(m => m.IsAvailable)
            .OrderBy(m => m.Category!.Name)
            .ThenBy(m => m.Name)
            .ToListAsync();

        Groups = items.GroupBy(m => m.Category!.Name).ToList();
        CartCount = CartHelper.GetCart(HttpContext.Session).Sum(c => c.Quantity);
    }
}

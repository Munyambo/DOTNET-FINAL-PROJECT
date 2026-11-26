using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Restaurant_Management_System.Data;
using Restaurant_Management_System.Helpers;

namespace Restaurant_Management_System.Pages.CustomerArea;

public class CartModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CartModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<CartLine> Lines { get; set; } = new();

    public decimal Total => Lines.Sum(l => l.LineTotal);

    [TempData]
    public string? Message { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    // "Update Cart" button: saves the new quantity of every line (0 removes the line)
    public async Task<IActionResult> OnPostUpdateAsync(List<int> menuItemIds, List<int> quantities)
    {
        var cart = CartHelper.GetCart(HttpContext.Session);

        for (int i = 0; i < menuItemIds.Count && i < quantities.Count; i++)
        {
            var line = cart.FirstOrDefault(c => c.MenuItemId == menuItemIds[i]);
            if (line == null) continue;

            line.Quantity = Math.Min(quantities[i], 50);
            if (line.Quantity < 1) cart.Remove(line);
        }

        CartHelper.SaveCart(HttpContext.Session, cart);
        Message = "Cart updated.";
        return RedirectToPage();
    }

    public IActionResult OnPostRemove(int menuItemId)
    {
        var cart = CartHelper.GetCart(HttpContext.Session);
        cart.RemoveAll(c => c.MenuItemId == menuItemId);
        CartHelper.SaveCart(HttpContext.Session, cart);

        Message = "Item removed from your cart.";
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var (lines, removed) = await CartHelper.BuildLinesAsync(_context, HttpContext.Session);
        Lines = lines;

        if (removed.Any())
        {
            Message = "These items are no longer available and were removed: " + string.Join(", ", removed) + ".";
        }
    }
}

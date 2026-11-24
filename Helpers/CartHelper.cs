using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Data;
using Restaurant_Management_System.Models;

namespace Restaurant_Management_System.Helpers;

// One line in the customer's cart (kept in the Session, not in the database)
public class CartItem
{
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
}

// One cart line with the food's current name and price, ready to show on a page
public class CartLine
{
    public MenuItem MenuItem { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal LineTotal => MenuItem.Price * Quantity;
}

// Reads and saves the cart in the Session, so every page (Menu, Cart, Checkout) uses the same code
public static class CartHelper
{
    private const string CartKey = "Cart";

    public static List<CartItem> GetCart(ISession session)
    {
        var json = session.GetString(CartKey);
        if (string.IsNullOrEmpty(json)) return new List<CartItem>();
        return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
    }

    public static void SaveCart(ISession session, List<CartItem> cart)
    {
        session.SetString(CartKey, JsonSerializer.Serialize(cart));
    }

    public static void ClearCart(ISession session)
    {
        session.Remove(CartKey);
    }

    // Turns the cart into lines with up-to-date prices from the database.
    // Food that was deleted or made unavailable since it was added is removed from the cart;
    // "removed" tells the page which names were dropped so it can warn the customer.
    public static async Task<(List<CartLine> Lines, List<string> Removed)> BuildLinesAsync(
        ApplicationDbContext db, ISession session)
    {
        var cart = GetCart(session);
        var ids = cart.Select(c => c.MenuItemId).ToList();
        var items = await db.MenuItems.Where(m => ids.Contains(m.Id)).ToListAsync();

        var lines = new List<CartLine>();
        var removed = new List<string>();
        foreach (var c in cart)
        {
            var item = items.FirstOrDefault(m => m.Id == c.MenuItemId);
            if (item == null) continue;                       // deleted: drop silently
            if (!item.IsAvailable) { removed.Add(item.Name); continue; }
            lines.Add(new CartLine { MenuItem = item, Quantity = c.Quantity });
        }

        // Save the cleaned-up cart if anything was dropped
        if (lines.Count != cart.Count)
        {
            SaveCart(session, lines.Select(l => new CartItem { MenuItemId = l.MenuItem.Id, Quantity = l.Quantity }).ToList());
        }
        return (lines, removed);
    }
}

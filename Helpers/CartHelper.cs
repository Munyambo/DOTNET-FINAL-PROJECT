using System.Text.Json;

namespace Restaurant_Management_System.Helpers;

// One line in the customer's cart (kept in the Session, not in the database)
public class CartItem
{
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
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
}

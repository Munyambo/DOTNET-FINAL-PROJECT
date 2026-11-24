using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Data;
using Restaurant_Management_System.Helpers;
using Restaurant_Management_System.Models;

namespace Restaurant_Management_System.Pages.CustomerArea;

public class CheckoutModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CheckoutModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<CartLine> Lines { get; set; } = new();

    public decimal Total => Lines.Sum(l => l.LineTotal);

    public SelectList Branches { get; set; } = default!;

    [BindProperty, Required(ErrorMessage = "Please select a branch.")]
    public int? BranchId { get; set; }

    // One note per cart line (same order as Lines), saved as the order item's SpecialInstructions
    [BindProperty]
    public List<string?> Instructions { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadAsync();
        if (!Lines.Any()) return RedirectToPage("./Cart");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadAsync();
        if (!Lines.Any()) return RedirectToPage("./Cart");

        // 1. A branch must be chosen
        if (BranchId != null && !await _context.Branches.AnyAsync(b => b.Id == BranchId))
        {
            ModelState.AddModelError("BranchId", "Please select a branch.");
        }
        if (!ModelState.IsValid) return Page();

        // 2. Who is logged in? Map the login to the Customer record (same idea as My Orders)
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        var customerId = await _context.Customers
            .Where(c => c.UserId == userId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();
        if (customerId == 0)
        {
            ModelState.AddModelError("", "We could not find your customer profile. Please contact the restaurant.");
            return Page();
        }

        // 3. Build the order. Prices come from the database, never from the page.
        var order = new Order
        {
            CustomerId = customerId,
            BranchId = BranchId!.Value,
            Status = "Pending",
            OrderDate = DateTime.Now,
            TotalAmount = Total
        };

        for (int i = 0; i < Lines.Count; i++)
        {
            var note = i < Instructions.Count ? Instructions[i]?.Trim() : null;
            order.Items.Add(new OrderItem
            {
                MenuItemId = Lines[i].MenuItem.Id,
                Quantity = Lines[i].Quantity,
                UnitPrice = Lines[i].MenuItem.Price,
                SpecialInstructions = string.IsNullOrEmpty(note) ? null : (note.Length > 300 ? note[..300] : note)
            });
        }

        // 4. Save the order and its items together, then empty the cart
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        CartHelper.ClearCart(HttpContext.Session);

        return RedirectToPage("./Orders/Details", new { id = order.Id });
    }

    private async Task LoadAsync()
    {
        var (lines, removed) = await CartHelper.BuildLinesAsync(_context, HttpContext.Session);
        Lines = lines;
        Branches = new SelectList(await _context.Branches.ToListAsync(), "Id", "Name");
    }
}

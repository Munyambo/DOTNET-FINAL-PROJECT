using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.CustomerArea.Orders;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Order Order { get; set; } = default!;

    // The normal order flow (Cancelled is not part of it)
    public static readonly string[] Steps = { "Pending", "Confirmed", "Preparing", "Ready", "Completed" };

    public bool IsCancelled => Order.Status == "Cancelled";

    // Position of the current status in the flow: 0 = Pending ... 4 = Completed
    public int CurrentStep => Array.IndexOf(Steps, Order.Status);

    // Progress bar width: Pending = 20%, Confirmed = 40% ... Completed = 100%
    public int Percent => (CurrentStep + 1) * 100 / Steps.Length;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        // Login -> Customer record (same idea as the Index page)
        var customerId = await _context.Customers
            .Where(c => c.UserId == userId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();

        if (customerId == 0)
        {
            return NotFound();
        }

        // The order must match the id in the URL AND belong to THIS customer
        var order = await _context.Orders
            .Include(o => o.Branch)
            .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
            .FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == customerId);

        if (order is null)
        {
            return NotFound();
        }

        Order = order;
        return Page();
    }
}
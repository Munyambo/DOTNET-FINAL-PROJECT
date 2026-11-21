using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.CustomerArea.Orders;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Order> Orders { get; set; } = new List<Order>();

    public async Task<IActionResult> OnGetAsync()
    {
        // 1. Who is logged in? (set by Alvin's Login page)
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        // 2. Map the login (UserId) to the Customer record, we only need its Id
        var customerId = await _context.Customers
            .Where(c => c.UserId == userId)
            .Select(c => c.Id)
            .FirstOrDefaultAsync();   // returns 0 if there is no customer record

        // 3. Get only THIS customer's orders, newest first
        Orders = await _context.Orders
            .Include(o => o.Branch)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return Page();
    }

    // Picks a Bootstrap colour for each status
    public string GetBadgeClass(string status)
    {
        return status switch
        {
            "Pending" => "bg-secondary",
            "Confirmed" => "bg-info text-dark",
            "Preparing" => "bg-warning text-dark",
            "Ready" => "bg-primary",
            "Completed" => "bg-success",
            "Cancelled" => "bg-danger",
            _ => "bg-dark"
        };
    }
}
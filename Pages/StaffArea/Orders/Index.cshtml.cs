using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.StaffArea.Orders;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Order> Orders { get; set; } = new List<Order>();
    public bool IsAdmin { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        IsAdmin = HttpContext.Session.GetString("Role") == "Admin";

        // Start with ALL orders (nothing is read from the database yet)
        var query = _context.Orders
            .Include(o => o.Branch)
            .Include(o => o.Customer)
            .AsQueryable();

        if (!IsAdmin)
        {
            // Staff: login -> Staff record -> BranchId
            var branchId = await _context.Staffs
                .Where(s => s.UserId == userId)
                .Select(s => s.BranchId)
                .FirstOrDefaultAsync();   // 0 if there is no staff record

            // Only keep orders of THIS staff member's branch
            query = query.Where(o => o.BranchId == branchId);
        }

        Orders = await query
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return Page();
    }

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
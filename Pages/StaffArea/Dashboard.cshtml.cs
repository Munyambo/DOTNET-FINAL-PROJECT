using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.StaffArea;

public class DashboardModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DashboardModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public decimal TotalSales { get; set; }
    public string Scope { get; set; } = "";   // text shown under the title

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

        // Start with ALL orders (nothing is read from the database yet)
        var query = _context.Orders.AsQueryable();

        if (isAdmin)
        {
            Scope = "All branches";
        }
        else
        {
            // Staff: login -> Staff record -> BranchId and the branch name
            var staff = await _context.Staffs
                .Where(s => s.UserId == userId)
                .Select(s => new { s.BranchId, BranchName = s.Branch!.Name })
                .FirstOrDefaultAsync();

            // No staff record: BranchId 0 matches no order, so all numbers are 0
            var branchId = staff?.BranchId ?? 0;
            Scope = staff?.BranchName ?? "No branch assigned";

            query = query.Where(o => o.BranchId == branchId);
        }

        // Each line below is one SQL query on the (already filtered) orders
        TotalOrders = await query.CountAsync();
        PendingOrders = await query.CountAsync(o => o.Status == "Pending");
        TotalSales = await query
            .Where(o => o.Status == "Completed")
            .SumAsync(o => o.TotalAmount);

        return Page();
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.StaffArea;

public class ReportsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ReportsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // Small classes that hold one row of each report
    public class BranchSalesRow
    {
        public string BranchName { get; set; } = "";
        public int Orders { get; set; }
        public decimal Sales { get; set; }
    }

    public class TopItemRow
    {
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Revenue { get; set; }
    }

    // The statuses, in the order we want to display them
    public static readonly string[] AllStatuses =
        { "Pending", "Confirmed", "Preparing", "Ready", "Completed", "Cancelled" };

    // Filter values come from the URL (?From=...&To=...)
    [BindProperty(SupportsGet = true)]
    public DateTime? From { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? To { get; set; }

    public string Scope { get; set; } = "";
    public string? ErrorMessage { get; set; }

    public IList<BranchSalesRow> SalesPerBranch { get; set; } = new List<BranchSalesRow>();
    public IList<TopItemRow> TopItems { get; set; } = new List<TopItemRow>();
    public Dictionary<string, int> StatusCounts { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

        // 1. Start with ALL orders (nothing is read from the database yet)
        var orders = _context.Orders.AsQueryable();

        // 2. Branch rule: staff only get their own branch
        if (isAdmin)
        {
            Scope = "All branches";
        }
        else
        {
            var staff = await _context.Staffs
                .Where(s => s.UserId == userId)
                .Select(s => new { s.BranchId, BranchName = s.Branch!.Name })
                .FirstOrDefaultAsync();

            var branchId = staff?.BranchId ?? 0;
            Scope = staff?.BranchName ?? "No branch assigned";

            orders = orders.Where(o => o.BranchId == branchId);
        }

        // 3. Date filter (From and To are both optional)
        if (From.HasValue && To.HasValue && From.Value.Date > To.Value.Date)
        {
            ErrorMessage = "The From date must be before the To date.";
            return Page();
        }
        if (From.HasValue)
        {
            orders = orders.Where(o => o.OrderDate >= From.Value.Date);
        }
        if (To.HasValue)
        {
            // "< next day" so the whole To day is included
            orders = orders.Where(o => o.OrderDate < To.Value.Date.AddDays(1));
        }

        // REPORT 1: sales per branch (Completed orders only)
        SalesPerBranch = await orders
            .Where(o => o.Status == "Completed")
            .GroupBy(o => o.Branch!.Name)
            .Select(g => new BranchSalesRow
            {
                BranchName = g.Key,
                Orders = g.Count(),
                Sales = g.Sum(o => o.TotalAmount)
            })
            .OrderByDescending(r => r.Sales)
            .ToListAsync();

        // REPORT 2: top 5 ordered items (Cancelled orders do not count)
        TopItems = await orders
            .Where(o => o.Status != "Cancelled")
            .SelectMany(o => o.Items)
            .GroupBy(i => i.MenuItem!.Name)
            .Select(g => new TopItemRow
            {
                Name = g.Key,
                Quantity = g.Sum(i => i.Quantity),
                Revenue = g.Sum(i => i.Quantity * i.UnitPrice)
            })
            .OrderByDescending(r => r.Quantity)
            .ThenBy(r => r.Name)
            .Take(5)
            .ToListAsync();

        // REPORT 3: orders grouped by status
        StatusCounts = await orders
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);

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
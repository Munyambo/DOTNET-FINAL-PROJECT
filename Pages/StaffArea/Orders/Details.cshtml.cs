using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.StaffArea.Orders;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Order Order { get; set; } = default!;

    // The only statuses we accept
    public static readonly string[] AllStatuses =
        { "Pending", "Confirmed", "Preparing", "Ready", "Completed", "Cancelled" };

    // Survives the redirect, so we can show "Status updated" once
    [TempData]
    public string? Message { get; set; }

    // Used by BOTH OnGet and OnPost, so the branch rule lives in one place
    private async Task<Order?> LoadOrderAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var isAdmin = HttpContext.Session.GetString("Role") == "Admin";

        var query = _context.Orders
            .Include(o => o.Branch)
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
            .Where(o => o.Id == id);

        if (!isAdmin)
        {
            // Staff: login -> Staff record -> BranchId
            var branchId = await _context.Staffs
                .Where(s => s.UserId == userId)
                .Select(s => s.BranchId)
                .FirstOrDefaultAsync();

            // Order must belong to THIS staff member's branch
            query = query.Where(o => o.BranchId == branchId);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var order = await LoadOrderAsync(id.Value);
        if (order is null)
        {
            return NotFound();
        }

        Order = order;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id, string? newStatus)
    {
        if (id is null)
        {
            return NotFound();
        }

        // Reject any status that is not in our list
        if (newStatus is null || !AllStatuses.Contains(newStatus))
        {
            return BadRequest();
        }

        // Same branch check as OnGet: another branch's order = not found
        var order = await LoadOrderAsync(id.Value);
        if (order is null)
        {
            return NotFound();
        }

        order.Status = newStatus;
        await _context.SaveChangesAsync();

        Message = $"Order #{order.Id} is now {newStatus}.";
        return RedirectToPage(new { id = order.Id });
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Restaurant_Management_System.Models;
using Restaurant_Management_System.Data;

namespace Restaurant_Management_System.Pages.CustomerPages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Customer> Customer { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string? SearchName { get; set; }

 
    public Dictionary<int, bool> ActiveByUser { get; set; } = new();

    public async Task OnGetAsync()
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchName))
        {
            query = query.Where(c => c.FullName.Contains(SearchName));
        }

        Customer = await query.ToListAsync();

        
        var userIds = Customer.Where(c => c.UserId != null).Select(c => c.UserId!.Value).ToList();
        ActiveByUser = await _context.UsersR
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.IsActive);
    }

    
    public async Task<IActionResult> OnPostToggleActiveAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer?.UserId == null) return NotFound();

        var user = await _context.UsersR.FindAsync(customer.UserId.Value);
        if (user == null) return NotFound();

        user.IsActive = !user.IsActive;     
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }
}
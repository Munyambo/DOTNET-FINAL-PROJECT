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

    public async Task OnGetAsync()
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchName))
        {
            query = query.Where(c => c.FullName.Contains(SearchName));
        }

        Customer = await query.ToListAsync();
    }
}
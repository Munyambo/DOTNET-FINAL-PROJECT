using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMSWebApi.Data;
using RMSWebApi.Models;

namespace RMSWebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ApiContext _context;

        public CustomerController(ApiContext context)
        {
            _context = context;
        }

        // READ ALL
        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            var customers = _context.Customers
                .Where(c => search == null || c.FullName.Contains(search))
                .Select(c => new
                {
                    c.Id,
                    c.FullName,
                    c.Phone,
                    c.Address,
                    c.UserId,
                    Email = _context.UsersR.Where(u => u.Id == c.UserId).Select(u => u.Email).FirstOrDefault(),
                    IsActive = _context.UsersR.Where(u => u.Id == c.UserId).Select(u => u.IsActive).FirstOrDefault()
                })
                .ToList();
            return Ok(customers);
        }

        // READ ONE
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        // UPDATE
        [HttpPut]
        public IActionResult Update(Customer customer)
        {
            _context.Customers.Update(customer);
            _context.SaveChanges();
            return Ok(customer);
        }

        // ACTIVATE / DEACTIVATE TOGGLE
        [HttpPut("{id:int}")]
        public IActionResult ToggleActive(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();

            var user = _context.UsersR.Find(customer.UserId);
            if (user == null) return NotFound("This customer has no login.");

            user.IsActive = !user.IsActive; // true becomes false and false becomes true
            _context.SaveChanges();

            return Ok(user.IsActive ? "Customer activated." : "Customer deactivated.");
        }
    }
}
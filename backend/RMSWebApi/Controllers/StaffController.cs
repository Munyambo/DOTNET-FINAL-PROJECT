using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMSWebApi.Data;
using RMSWebApi.Models;
using System.Text;

namespace RMSWebApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly ApiContext _context;

        public StaffController(ApiContext context)
        {
            _context = context;
        }

        private string HashPassword(string password)
        {
            var bytes = System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        // READ ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Staffs.Include(staff => staff.Branch).ToList());
        }

        // READ ONE 
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var staff = _context.Staffs.Include(staff => staff.Branch).FirstOrDefault(staff => staff.Id == id);
            if (staff == null) return NotFound();
            return Ok(staff);
        }

        // CREATE
        [HttpPost]
        public IActionResult Create(StaffRequest request)
        {
            
            if (_context.UsersR.Any(user => user.Email == request.Email))
                return BadRequest("This email is already used.");

            // create the login with role Staff
            var user = new UsersR
            {
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Role = "Staff",
                IsActive = true
            };
            _context.UsersR.Add(user);
            _context.SaveChanges(); 

            // create the staff details, linked to that login
            var staff = new Staff
            {
                FullName = request.FullName,
                Phone = request.Phone,
                Position = request.Position,
                IsManager = request.IsManager,
                BranchId = request.BranchId,
                UserId = user.Id
            };
            _context.Staffs.Add(staff);
            _context.SaveChanges();

            return Ok(staff);
        }

        // UPDATE
        [HttpPut]
        public IActionResult Update(Staff staff)
        {
            _context.Staffs.Update(staff);
            _context.SaveChanges();
            return Ok(staff);
        }

        // DELETE
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var staff = _context.Staffs.Find(id);
            if (staff == null) return NotFound();

            // delete the staff first (it points to the login)
            _context.Staffs.Remove(staff);

            // delete their login
            var user = _context.UsersR.Find(staff.UserId);
            if (user != null) _context.UsersR.Remove(user);

            _context.SaveChanges();
            return Ok(staff);
        }
    }
}
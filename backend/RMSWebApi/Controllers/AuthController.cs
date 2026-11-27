using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RMSWebApi.Data;
using RMSWebApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RMSWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

   
        private readonly ApiContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApiContext context, IConfiguration config)
        {
            // comes form the db
            _context = context;

            // comes from appsettings.json
            _config = config;
        }

        private string HashPassword(string password)
        {
            var bytes = System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            // 1. Find the user
            var user = _context.UsersR.FirstOrDefault(u => u.Email == request.Email);
            if (user == null) return Unauthorized("Wrong email or password.");

            // 2. Check the password
            if (user.PasswordHash != HashPassword(request.Password)) return Unauthorized("Wrong email or password.");

            // 3. Check the account is active
            if (!user.IsActive) return Unauthorized("This account is deactivated.");

            // 4. Make the token (the wristband)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            // 5. Send it back
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                role = user.Role,
                name = user.Email,
                userId = user.Id
            });
        }

        [HttpPost]
        public IActionResult Register(RegisterRequest request)
        {
            // 1. Is the email already used?
            if (_context.UsersR.Any(user=> user.Email == request.Email))
                return BadRequest("This email is already registered.");

            // 2. Save the login in UsersR
            var user = new UsersR
            {
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Role = "Customer",
                IsActive = true
            };
            _context.UsersR.Add(user);
            _context.SaveChanges(); // now user.Id has a value

            // 3. Save the customer details, linked to that login
            var customer = new Customer
            {
                FullName = request.FullName,
                Phone = request.Phone,
                Address = request.Address,
                UserId = user.Id
            };
            _context.Customers.Add(customer);
            _context.SaveChanges();

            return Ok("Account created. You can now log in.");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            return Ok(new { userId, email, role });
        }
    }
}
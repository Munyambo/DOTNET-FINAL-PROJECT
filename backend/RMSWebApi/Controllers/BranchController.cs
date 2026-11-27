using Microsoft.AspNetCore.Mvc;
using RMSWebApi.Data;
using RMSWebApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace RMSWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly ApiContext _context;

        public BranchController(ApiContext context)
        {
            _context = context;
        }

        //CREATE
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Branch branch)
        {
            branch.Id = 0;
            _context.Branches.Add(branch);
            _context.SaveChanges();
            return Ok(branch);
        }

        //VIEW ALL
       
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _context.Branches.ToList();
            return Ok(result);
        }

        //VIEW ONE
        
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var branch = _context.Branches.Find(id);
            if (branch == null)
            {
                return NotFound();
            }
            return Ok(branch);
        }

        //UPDATE
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public IActionResult Update(Branch branch)
        {
            _context.Branches.Update(branch);
            _context.SaveChanges();
            return Ok(branch);
        }

        //DELETE
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var branch = _context.Branches.Find(id);
            if (branch == null)
            {
                return NotFound();
            }

            // A branch that still has staff or orders cannot be deleted
            if (_context.Staffs.Any(s => s.BranchId == id) || _context.Orders.Any(o => o.BranchId == id))
            {
                return BadRequest("This branch still has staff or orders, so it cannot be deleted.");
            }

            _context.Branches.Remove(branch);
            _context.SaveChanges();
            return Ok(branch);
        }
    }
}
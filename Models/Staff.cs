using System.ComponentModel.DataAnnotations;

namespace Restaurant_Management_System.Models
{
    public class Staff
    {
        public int Id { get; set; }
        [Required] public string FullName { get; set; } = "";
        public string? Phone { get; set; }
        public string? Position { get; set; }
        public bool IsManager { get; set; }
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
        public string? UserId { get; set; }
    }
}

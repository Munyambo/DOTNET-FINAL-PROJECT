using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMSWebApi.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = "";
        [Column(TypeName = "decimal(18,2)")] public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}

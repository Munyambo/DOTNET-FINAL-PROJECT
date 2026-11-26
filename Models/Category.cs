using System.ComponentModel.DataAnnotations;

namespace Restaurant_Management_System.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = "";
    }
}

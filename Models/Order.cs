using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant_Management_System.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
        [Column(TypeName = "decimal(18,2)")] public decimal TotalAmount { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}

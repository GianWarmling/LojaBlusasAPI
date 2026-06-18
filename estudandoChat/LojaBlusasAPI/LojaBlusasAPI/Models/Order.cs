using System.ComponentModel.DataAnnotations;

namespace LojaBlusasAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public string CustomerName { get; set; } = string.Empty;
        [Required]
        public string CustomerEmail { get; set; } = string.Empty;
        [Required]
        public string CustomerPhone { get; set; } = string.Empty;
        [Required] 
        public string Address { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public List<OrderItem> Items { get; set; } = new();
    }
}

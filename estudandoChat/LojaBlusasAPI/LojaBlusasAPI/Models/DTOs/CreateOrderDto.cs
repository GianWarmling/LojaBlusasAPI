namespace LojaBlusasAPI.Models.DTOs
{
    public class CreateOrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public List<CreacteOrderItemDto> Items { get; set; } = new();
    }
}

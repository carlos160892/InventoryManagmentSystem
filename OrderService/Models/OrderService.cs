namespace OrderService.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public required string Status { get; set; }  // Por ejemplo: "Pending", "Completed", "Canceled"
        public decimal TotalAmount { get; set; }
        
        // Relación con los ítems del pedido
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }  // ID del producto asociado
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;  // Calculado automáticamente
    }
}

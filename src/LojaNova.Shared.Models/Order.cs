using System.ComponentModel.DataAnnotations.Schema;

namespace LojaNova.Shared.Models;

public class Order
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending"; // Ex: Pending, Processing, Shipped, Completed, Cancelled

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public Client? Client { get; set; } // Navegação
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    public Order? Order { get; set; } // Navegação
    public Product? Product { get; set; } // Navegação
}
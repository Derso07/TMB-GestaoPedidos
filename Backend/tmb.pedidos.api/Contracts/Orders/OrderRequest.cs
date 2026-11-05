using System.ComponentModel.DataAnnotations;

namespace tmb.pedidos.API.Contracts.Orders;

public class OrderRequest
{
    [Required]
    [MaxLength(200)]
    public string Customer { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Product { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Value { get; set; }
}
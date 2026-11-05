using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tmb.pedidos.Domain.Entities;
using tmb.pedidos.Domain.Enum;

namespace tmb.pedidos.Domain.Request;

public class OrderRequest
{
    [Required]
    [MaxLength(200)]
    public string customer { get; set; }
    [Required]
    [MaxLength(200)]
    public string Product { get; set; }
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Value { get; set; }
}
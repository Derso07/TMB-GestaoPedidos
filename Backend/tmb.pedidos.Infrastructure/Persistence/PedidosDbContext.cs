using Microsoft.EntityFrameworkCore;
using tmb.pedidos.Domain.Entities;

namespace tmb.pedidos.Infrastructure.Persistence;

public class PedidosDbContext : DbContext
{
    public PedidosDbContext(DbContextOptions<PedidosDbContext> options) :  base(options) { }

    public DbSet<Order> Orders{ get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    { 
        modelBuilder.Entity<Order>(entity => 
        { 
            entity.HasKey(e => e.Id); 
            entity.Property(e => e.Customer).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Product).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Value).IsRequired().HasColumnType("numeric(18,2)");
            entity.Property(e => e.CreatOn).IsRequired().HasDefaultValueSql("NOW()"); 
        }); 
    } 
}
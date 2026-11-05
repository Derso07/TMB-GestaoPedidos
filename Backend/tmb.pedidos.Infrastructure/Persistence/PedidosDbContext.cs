using Microsoft.EntityFrameworkCore;
using tmb.pedidos.Domain.Entities;

namespace tmb.pedidos.Infrastructure.Persistence;

public class PedidosDbContext : DbContext
{
    public PedidosDbContext(DbContextOptions<PedidosDbContext> options) :  base(options) { }

    public DbSet<Order> Orders{ get; set; }

    public DbSet<OrderStatusHistory> OrderStatusHistories{ get; set; }
    
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
        
        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.ToTable("order_status_history");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Id).ValueGeneratedNever();

            entity.Property(x => x.PreviousStatus)
                .HasColumnName("previous_status")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired(false);

            entity.Property(x => x.NewStatus)
                .HasColumnName("new_status")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.ChangeAt)
                .HasColumnName("changed_at")
                .IsRequired();

            entity.HasOne(x => x.Order)
                .WithMany(x => x.StatusHistory)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new
            {
                x.OrderId,
                x.ChangeAt
            });
        });
    } 
}
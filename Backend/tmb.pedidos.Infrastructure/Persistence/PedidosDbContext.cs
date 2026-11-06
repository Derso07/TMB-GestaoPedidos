using Microsoft.EntityFrameworkCore;
using tmb.pedidos.Domain.Entities;
using tmb.pedidos.Infrastructure.Outbox;

namespace tmb.pedidos.Infrastructure.Persistence;

public class PedidosDbContext : DbContext
{
    public PedidosDbContext(DbContextOptions<PedidosDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    
    public DbSet<OutboxMessage> OutboxMessages { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(PedidosDbContext).Assembly);
        
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

        modelBuilder.Entity<OutboxMessage>(entity =>
            {
                entity.ToTable("outbox_messages");

                entity.HasKey(message => message.Id);

                entity.Property(message => message.Type)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(message => message.Payload)
                    .IsRequired();

                entity.Property(message => message.OccurredOnUtc)
                    .IsRequired();

                entity.Property(message => message.LastError)
                    .HasMaxLength(2000);

                entity.HasIndex(message =>
                    new
                    {
                        message.ProcessedOnUtc,
                        message.NextAttemptOnUtc
                    });
            }
        );
    }
}
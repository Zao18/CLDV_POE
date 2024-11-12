using Microsoft.EntityFrameworkCore;
using CLDV_POE.Models;

public class SqlService : DbContext
{
    public DbSet<CustomerSql> Customers { get; set; } // (dotnet-bot, 2024)
    public DbSet<ProductSql> Products { get; set; } // (dotnet-bot, 2024)
    public DbSet<OrderSql> Orders { get; set; } // (dotnet-bot, 2024)

    public SqlService(DbContextOptions<SqlService> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<CustomerSql>().HasKey(c => c.CustomerId); // (AndriySvyryd, 2022)
        modelBuilder.Entity<ProductSql>().HasKey(p => p.ProductId); // (AndriySvyryd, 2022)
        modelBuilder.Entity<OrderSql>().HasKey(o => o.Order_Id); // (AndriySvyryd, 2022)
    }
}









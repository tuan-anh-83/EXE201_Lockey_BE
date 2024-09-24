using EXE201_Lockey.Models;
using Microsoft.EntityFrameworkCore;

namespace EXE201_Lockey.Data
{
	public class DataContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Template> Templates { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<Payment> Payments { get; set; }
		public DbSet<Theme> Themes { get; set; }

		public DataContext(DbContextOptions<DataContext> options)
			: base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			// Chỉ định tên bảng cho các thực thể
			modelBuilder.Entity<User>().ToTable("User");
			modelBuilder.Entity<Template>().ToTable("Template");
			modelBuilder.Entity<Product>().ToTable("Product");
			modelBuilder.Entity<Order>().ToTable("Order");
			modelBuilder.Entity<Payment>().ToTable("Payment");
			modelBuilder.Entity<Theme>().ToTable("Theme");

			// User-Product relationship (One-to-Many)
			modelBuilder.Entity<Product>()
				.HasOne(p => p.User)
				.WithMany(u => u.Products)
				.HasForeignKey(p => p.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			// Product-Template relationship (Many-to-One)
			modelBuilder.Entity<Product>()
				.HasOne(p => p.Template)
				.WithMany(t => t.Products)
				.HasForeignKey(p => p.TemplateID)
				.OnDelete(DeleteBehavior.Restrict);

			// Template-Theme relationship (Many-to-One)
			modelBuilder.Entity<Template>()
				.HasOne(t => t.Theme)
				.WithMany(th => th.Templates)
				.HasForeignKey(t => t.ThemeID)
				.OnDelete(DeleteBehavior.Restrict);

			// Order-Product relationship (Many-to-One)
			modelBuilder.Entity<Order>()
				.HasOne(o => o.Product)
				.WithMany(p => p.Orders)
				.HasForeignKey(o => o.ProductID)
				.OnDelete(DeleteBehavior.Cascade);

			// Order-Payment relationship (One-to-One)
			modelBuilder.Entity<Payment>()
				.HasOne(p => p.Order)
				.WithOne(o => o.Payment)
				.HasForeignKey<Payment>(p => p.OrderID)
				.OnDelete(DeleteBehavior.Cascade);

			// Unique constraint on User.Email
			modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();

			// Cấu hình kiểu dữ liệu cho Price (Product) và TotalPrice (Order)
			modelBuilder.Entity<Product>()
				.Property(p => p.Price)
				.HasColumnType("decimal(18,2)"); // Cấu hình kiểu decimal với độ chính xác 18,2

			modelBuilder.Entity<Order>()
				.Property(o => o.TotalPrice)
				.HasColumnType("decimal(18,2)"); // Cấu hình kiểu decimal với độ chính xác 18,2

			modelBuilder.Entity<Order>()
				.Property(p => p.Amount)
				.HasColumnType("decimal(18,2)");
		}
	}



}

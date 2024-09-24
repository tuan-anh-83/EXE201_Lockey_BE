using EXE201_Lockey.Models;
using Microsoft.EntityFrameworkCore;

namespace EXE201_Lockey.Data
{
	public class DataContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Template> Templates { get; set; }
		public DbSet<CustomDesign> CustomDesigns { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
		public DbSet<Payment> Payments { get; set; }

		public DataContext(DbContextOptions<DataContext> options)
			: base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().ToTable("User");

			// Chỉ định khóa chính cho CustomDesign
			modelBuilder.Entity<CustomDesign>()
				.HasKey(cd => cd.DesignID);

			// Order-User relationship (One-to-Many)
			modelBuilder.Entity<Order>()
				.HasOne(o => o.User)
				.WithMany(u => u.Orders)
				.HasForeignKey(o => o.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			// User-CustomDesign relationship (One-to-Many)
			modelBuilder.Entity<CustomDesign>()
				.HasOne(cd => cd.User)
				.WithMany(u => u.CustomDesigns)
				.HasForeignKey(cd => cd.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			// Template-CustomDesign relationship (One-to-Many)
			modelBuilder.Entity<CustomDesign>()
				.HasOne(cd => cd.Template)
				.WithMany(t => t.CustomDesigns)
				.HasForeignKey(cd => cd.TemplateID)
				.OnDelete(DeleteBehavior.Restrict);

			// Product-OrderDetail relationship (One-to-Many)
			modelBuilder.Entity<OrderDetail>()
				.HasOne(od => od.Product)
				.WithMany(p => p.OrderDetails)
				.HasForeignKey(od => od.ProductID)
				.OnDelete(DeleteBehavior.Cascade);

			// Order-OrderDetail relationship (One-to-Many)
			modelBuilder.Entity<OrderDetail>()
				.HasOne(od => od.Order)
				.WithMany(o => o.OrderDetails)
				.HasForeignKey(od => od.OrderID)
				.OnDelete(DeleteBehavior.Cascade);

			// Order-Payment relationship (One-to-One)
			modelBuilder.Entity<Payment>()
				.HasOne(p => p.Order)
				.WithOne(o => o.Payment)
				.HasForeignKey<Payment>(p => p.OrderID)
				.OnDelete(DeleteBehavior.Cascade);

			// CustomDesign-Product relationship (One-to-One)
			modelBuilder.Entity<Product>()
				.HasOne(p => p.Design)
				.WithOne(cd => cd.Product)
				.HasForeignKey<Product>(p => p.DesignID)
				.OnDelete(DeleteBehavior.Restrict);

			// Unique constraint on User.Email
			modelBuilder.Entity<User>()
				.HasIndex(u => u.Email)
				.IsUnique();

			// Cấu hình kiểu dữ liệu cho TotalAmount (Order) và Price (Product)
			modelBuilder.Entity<Order>()
				.Property(o => o.TotalAmount)
				.HasColumnType("decimal(18,2)"); // Cấu hình kiểu decimal với độ chính xác 18,2

			modelBuilder.Entity<Product>()
				.Property(p => p.Price)
				.HasColumnType("decimal(18,2)"); // Cấu hình kiểu decimal với độ chính xác 18,2

			modelBuilder.Entity<OrderDetail>()
				.Property(p => p.Price)
				.HasColumnType("decimal(18,2)");

			modelBuilder.Entity<Payment>()
				.Property(p => p.AmountPaid)
				.HasColumnType("decimal(18,2)");
		}

	}


}

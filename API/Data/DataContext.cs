using System;
using API.Entities.Other;
using API.Entities.ProductEntities;
using API.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Photo> Photos { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // builder.Entity<Product>()
            //     .HasOne(p => p.SubCategory)
            //     .WithMany(l => l.Products);

            // builder.Entity<Product>()
            //     .HasOne(p => p.Brand)
            //     .WithMany(l => l.Products);

            // builder.Entity<Product>()
            //     .HasOne(p => p.Area)
            //     .WithMany(l => l.Products);

            // builder.Entity<Product>()
            //     .HasOne(p => p.Collection)
            //     .WithMany(l => l.Products);

            // builder.Entity<ProductColor>()
            //     .HasKey(k => new { k.ProductId, k.ColorId });

            // builder.Entity<ProductColor>()
            //     .HasOne(pc => pc.Product)
            //     .WithMany(p => p.ProductColors)
            //     .HasForeignKey(pc => pc.ProductId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // builder.Entity<ProductColor>()
            //     .HasOne(pc => pc.Color)
            //     .WithMany(p => p.ProductColors)
            //     .HasForeignKey(pc => pc.ColorId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // builder.Entity<SubCategory>()
            //     .HasOne(p => p.Category)
            //     .WithMany(l => l.SubCategories);

            // builder.Entity<CustomerFavorite>()
            //     .HasKey(k => new { k.ProductId, k.CustomerId });

            // builder.Entity<CustomerFavorite>()
            //     .HasOne(pc => pc.Product)
            //     .WithMany(p => p.FavoriteByCustomers)
            //     .HasForeignKey(pc => pc.ProductId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // builder.Entity<CustomerFavorite>()
            //     .HasOne(pc => pc.Customer)
            //     .WithMany(p => p.FavoriteProducts)
            //     .HasForeignKey(pc => pc.CustomerId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // builder.Entity<Cart>()
            //     .HasKey(c => new { c.ProductId, c.CustomerId });

            // builder.Entity<Cart>()
            //     .HasOne(pc => pc.Customer)
            //     .WithMany(p => p.Carts)
            //     .HasForeignKey(pc => pc.CustomerId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // builder.Entity<Cart>()
            //     .HasOne(pc => pc.Product)
            //     .WithMany(p => p.Carts)
            //     .HasForeignKey(pc => pc.ProductId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // builder.Entity<Cart>()
            //     .HasOne(pc => pc.Color)
            //     .WithMany(p => p.Carts)
            //     .HasForeignKey(pc => pc.ColorId)
            //     .IsRequired(false)
            //     .OnDelete(DeleteBehavior.Cascade);


            // builder.Entity<Order>()
            //     .HasOne(p => p.Customer)
            //     .WithMany(l => l.Orders);

            // builder.Entity<Order>()
            //     .HasOne(p => p.ShippingMethod)
            //     .WithMany(l => l.Orders);

            // builder.Entity<Order>()
            //     .HasOne(p => p.Promotion)
            //     .WithMany(l => l.Orders)
            //     .IsRequired(false);

            // builder.Entity<Order>()
            //     .HasOne(p => p.PaymentMethod)
            //     .WithMany(l => l.Orders);

            // builder.Entity<OrderDetail>()
            //     .HasKey(k => new { k.ProductId, k.OrderId });

            // builder.Entity<OrderDetail>()
            //     .HasOne(od => od.Order)
            //     .WithMany(o => o.OrderDetails)
            //     .HasForeignKey(od => od.OrderId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // builder.Entity<OrderDetail>()
            //     .HasOne(od => od.Product)
            //     .WithMany(o => o.OrderDetail)
            //     .HasForeignKey(od => od.ProductId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // builder.Entity<OrderDetail>()
            //     .HasOne(od => od.Color)
            //     .WithMany(o => o.OrderDetails)
            //     .HasForeignKey(od => od.ColorId)
            //     .IsRequired(false)
            //     .OnDelete(DeleteBehavior.Cascade);

            // builder.Entity<Paragraph>()
            //     .HasOne(p => p.Article)
            //     .WithMany(l => l.Paragraphs)
            //     .OnDelete(DeleteBehavior.Cascade);

            builder.ApplyUtcDateTimeConverter();

        }



    }

    public static class UtcDateAnnotation
    {
        private const String IsUtcAnnotation = "IsUtc";
        private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
          new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableConverter =
          new ValueConverter<DateTime?, DateTime?>(v => v, v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

        public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, Boolean isUtc = true) =>
          builder.HasAnnotation(IsUtcAnnotation, isUtc);

        public static Boolean IsUtc(this IMutableProperty property) =>
          ((Boolean?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true;

        public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (!property.IsUtc())
                    {
                        continue;
                    }

                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(UtcConverter);
                    }

                    if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(UtcNullableConverter);
                    }
                }
            }
        }
    }
}
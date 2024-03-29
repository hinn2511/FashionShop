using System;
using API.Entities.OrderModel;
using API.Entities.Other;
using API.Entities.OtherModel;
using API.Entities.ProductModel;
using API.Entities.User;
using API.Entities.UserModel;
using API.Entities.WebPageModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppPermission, int,
        IdentityUserClaim<int>, AppUserPermission, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }
        public DbSet<UploadedFile> Files { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<HomePage> HomePages { get; set; }
        public DbSet<Carousel> Carousels { get; set; }
        public DbSet<FeatureCategory> FeatureCategories { get; set; }
        public DbSet<FeatureProduct> FeatureProducts { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<UserLike> UserLikes { get; set; }
        public DbSet<UserReview> UserReviews { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<AppRolePermission> RolePermissions { get; set; }
        public DbSet<Settings> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasMany(ur => ur.UserPermissions)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AppPermission>()
                .HasMany(ur => ur.UserPermissions)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AppRole>()
                .HasMany(role => role.RolePermissions)
                .WithOne(rolePermission => rolePermission.AppRole)
                .HasForeignKey(rolePermission => rolePermission.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<AppPermission>()
                .HasMany(permission => permission.RolePermissions)
                .WithOne(rolePermission => rolePermission.AppPermission)
                .HasForeignKey(rolePermission => rolePermission.PermissionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Category>(c =>
            {
                c.HasOne(x => x.Parent)
                    .WithMany(x => x.SubCategories)
                    .HasForeignKey(x => x.ParentId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            builder.Entity<Category>(c =>
            {
                c.HasMany(x => x.SubCategories)
                    .WithOne(x => x.Parent)
                    .HasForeignKey(x => x.ParentId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Option>(o =>
            {
                o.HasMany(x => x.UserReviews)
                    .WithOne(x => x.Option)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<UserReview>(o =>
           {
               o.HasOne(x => x.Option)
               .WithMany(x => x.UserReviews)
               .HasForeignKey(x => x.OptionId)
               .IsRequired()
               .OnDelete(DeleteBehavior.NoAction);
           });

            builder.Entity<Order>(o =>
           {
               o.HasMany(x => x.UserReviews)
                   .WithOne(x => x.Order)
                   .OnDelete(DeleteBehavior.Cascade);
           });

            builder.Entity<UserReview>(o =>
           {
               o.HasOne(x => x.Order)
               .WithMany(x => x.UserReviews)
               .HasForeignKey(x => x.OrderId)
               .IsRequired()
               .OnDelete(DeleteBehavior.NoAction);
           });

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
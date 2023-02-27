using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Data;
using API.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using API.Entities.ProductModel;
using API.Extensions;
using System.Linq;
using API.Entities.UserModel;
using API.Entities.WebPageModel;

namespace API.Entities
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/SeedData/UserSeedData.json");

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            if (users == null) return;

            await users.ForEachAsync(async user =>
            {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Pa$$w0rd");
            });

            var admin = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");

            var manager = new AppUser
            {
                UserName = "manager"
            };

            await userManager.CreateAsync(manager, "Pa$$w0rd");

            var superAdmin = new AppUser
            {
                UserName = "sa"
            };

            await userManager.CreateAsync(superAdmin, "Pa$$w0rd");

            var supperAdminPermissions = new List<AppPermission>
            {
                new AppPermission{Name = "AdministratorAccess"},

                new AppPermission{PermissionGroup = "Role", Name = "RoleManagerAccess"},
                new AppPermission{PermissionGroup = "Role", Name = "CreateAccount"},
                new AppPermission{PermissionGroup = "Role", Name = "ViewRoles"},
                new AppPermission{PermissionGroup = "Role", Name = "ViewRoleDetail"},
                new AppPermission{PermissionGroup = "Role", Name = "ViewRolePermissions"},
                new AppPermission{PermissionGroup = "Role", Name = "CreateRole"},
                new AppPermission{PermissionGroup = "Role", Name = "DeleteRole"},
                new AppPermission{PermissionGroup = "Role", Name = "UpdateRolePermissions"},
                new AppPermission{PermissionGroup = "Role", Name = "ViewPermissions"},
                new AppPermission{PermissionGroup = "Role", Name = "CreatePermission" },

                new AppPermission{PermissionGroup = "User", Name = "UserManagerAccess"},
                new AppPermission{PermissionGroup = "User", Name = "ViewUsers"},
                new AppPermission{PermissionGroup = "User", Name = "SetRole"},
                new AppPermission{PermissionGroup = "User", Name = "RemoveRole"},
                new AppPermission{PermissionGroup = "User", Name = "ActivateUsers"},
                new AppPermission{PermissionGroup = "User", Name = "DeactivateUsers" },
                new AppPermission{PermissionGroup = "User", Name = "DeleteUsers"},

            };

            await userManager.AddToRolesAsync(superAdmin, supperAdminPermissions.Select(x => x.Name).ToArray());

            var sales = new AppUser
            {
                UserName = "sales"
            };

            await userManager.CreateAsync(sales, "Pa$$w0rd");
        }

        public static async Task SeedSetting(DataContext context)
        {
            var setting = new Settings();

            if (context.Settings.Any())
                return;

            setting.AdministratorLoginBackground = "";
            setting.ClientLoginBackground = "";
            setting.ClientRegisterBackground = "";
            setting.ClientLoginPhotoId = 0;
            setting.ClientRegisterPhotoId = 0;
            setting.AdministratorLoginPhotoId = 0;

            await context.Settings.AddAsync(setting);

            await context.SaveChangesAsync();
        }



        public static async Task SeedPermissions(RoleManager<AppPermission> roleManager)
        {
            if (await roleManager.Roles.AnyAsync()) return;

            var permissions = new List<AppPermission>
            {
                new AppPermission{Name = "AdministratorAccess"},

                new AppPermission{Name = "ClientAccess"},

                new AppPermission{PermissionGroup = "Role", Name = "RoleManagerAccess"},
                new AppPermission{PermissionGroup = "Role", Name = "CreateAccount"},
                new AppPermission{PermissionGroup = "Role", Name = "ViewRoles"},
                new AppPermission{PermissionGroup = "Role", Name = "ViewRoleDetail"},
                new AppPermission{PermissionGroup = "Role", Name = "ViewRolePermissions"},
                new AppPermission{PermissionGroup = "Role", Name = "CreateRole"},
                new AppPermission{PermissionGroup = "Role", Name = "DeleteRole"},
                new AppPermission{PermissionGroup = "Role", Name = "UpdateRolePermissions"},
                new AppPermission{PermissionGroup = "Role", Name = "ViewPermissions"},
                new AppPermission{PermissionGroup = "Role", Name = "CreatePermission" },

                new AppPermission{PermissionGroup = "User", Name = "UserManagerAccess"},
                new AppPermission{PermissionGroup = "User", Name = "ViewUsers"},
                new AppPermission{PermissionGroup = "User", Name = "SetRole"},
                new AppPermission{PermissionGroup = "User", Name = "RemoveRole"},
                new AppPermission{PermissionGroup = "User", Name = "ActivateUsers"},
                new AppPermission{PermissionGroup = "User", Name = "DeactivateUsers" },
                new AppPermission{PermissionGroup = "User", Name = "DeleteUsers"},

                new AppPermission{PermissionGroup = "Article", Name = "ArticleManagerAccess"},
                new AppPermission{PermissionGroup = "Article", Name = "ViewArticles"},
                new AppPermission{PermissionGroup = "Article", Name = "ViewArticleDetail"},
                new AppPermission{PermissionGroup = "Article", Name = "CreateArticle"},
                new AppPermission{PermissionGroup = "Article", Name = "EditArticle"},
                new AppPermission{PermissionGroup = "Article", Name = "SoftDeleteArticles"},
                new AppPermission{PermissionGroup = "Article", Name = "HardDeleteArticles" },
                new AppPermission{PermissionGroup = "Article", Name = "HideArticles"},
                new AppPermission{PermissionGroup = "Article", Name = "ActivateArticles"},
                new AppPermission{PermissionGroup = "Article", Name = "DemoteArticles" },
                new AppPermission{PermissionGroup = "Article", Name = "PromoteArticles"},

                new AppPermission{PermissionGroup = "Category", Name = "CategoryManagerAccess"},
                new AppPermission{PermissionGroup = "Category", Name = "ViewCategories"},
                new AppPermission{PermissionGroup = "Category", Name = "ViewCategoryDetail"},
                new AppPermission{PermissionGroup = "Category", Name = "ViewCatalogue"},
                new AppPermission{PermissionGroup = "Category", Name = "CreateCategory"},
                new AppPermission{PermissionGroup = "Category", Name = "EditCategory"},
                new AppPermission{PermissionGroup = "Category", Name = "SoftDeleteCategories"},
                new AppPermission{PermissionGroup = "Category", Name = "HardDeleteCategories" },
                new AppPermission{PermissionGroup = "Category", Name = "HideCategories"},
                new AppPermission{PermissionGroup = "Category", Name = "ActivateCategories"},
                new AppPermission{PermissionGroup = "Category", Name = "DemoteCategories" },
                new AppPermission{PermissionGroup = "Category", Name = "PromoteCategories"},

                new AppPermission{PermissionGroup = "Carousel", Name = "CarouselManagerAccess"},
                new AppPermission{PermissionGroup = "Carousel", Name = "ViewCarousels"},
                new AppPermission{PermissionGroup = "Carousel", Name = "ViewCarouselDetail"},
                new AppPermission{PermissionGroup = "Carousel", Name = "CreateCarousel"},
                new AppPermission{PermissionGroup = "Carousel", Name = "EditCarousel"},
                new AppPermission{PermissionGroup = "Carousel", Name = "SoftDeleteCarousels"},
                new AppPermission{PermissionGroup = "Carousel", Name = "HardDeleteCarousels" },
                new AppPermission{PermissionGroup = "Carousel", Name = "HideCarousels"},
                new AppPermission{PermissionGroup = "Carousel", Name = "ActivateCarousels"},

                new AppPermission{PermissionGroup = "Order", Name = "OrderManagerAccess"},
                new AppPermission{PermissionGroup = "Order", Name = "ViewOrders"},
                new AppPermission{PermissionGroup = "Order", Name = "ViewOrderSummary"},
                new AppPermission{PermissionGroup = "Order", Name = "ViewOrderDetail"},
                new AppPermission{PermissionGroup = "Order", Name = "VerifyOrder"},
                new AppPermission{PermissionGroup = "Order", Name = "ShippingOrder"},
                new AppPermission{PermissionGroup = "Order", Name = "ConfirmOrderShipped"},
                new AppPermission{PermissionGroup = "Order", Name = "CancelOrder"},
                new AppPermission{PermissionGroup = "Order", Name = "AcceptReturnOrder"},
                new AppPermission{PermissionGroup = "Order", Name = "AcceptCancelOrder"},

                new AppPermission{PermissionGroup = "Product option", Name = "ProductOptionManagerAccess"},
                new AppPermission{PermissionGroup = "Product option", Name = "ViewProductOptions"},
                new AppPermission{PermissionGroup = "Product option", Name = "ViewProductOptionDetail"},
                new AppPermission{PermissionGroup = "Product option", Name = "CreateProductOption"},
                new AppPermission{PermissionGroup = "Product option", Name = "EditProductOption"},
                new AppPermission{PermissionGroup = "Product option", Name = "SoftDeleteProductOptions"},
                new AppPermission{PermissionGroup = "Product option", Name = "HardDeleteProductOptions" },
                new AppPermission{PermissionGroup = "Product option", Name = "HideProductOptions"},
                new AppPermission{PermissionGroup = "Product option", Name = "ActivateProductOptions"},
                new AppPermission{PermissionGroup = "Product option", Name = "DemoteProductOptions" },
                new AppPermission{PermissionGroup = "Product option", Name = "PromoteProductOptions"},

                new AppPermission{PermissionGroup = "Product", Name = "ProductManagerAccess"},
                new AppPermission{PermissionGroup = "Product", Name = "ViewProducts"},
                new AppPermission{PermissionGroup = "Product", Name = "ViewProductDetail"},
                new AppPermission{PermissionGroup = "Product", Name = "CreateProduct"},
                new AppPermission{PermissionGroup = "Product", Name = "EditProduct"},
                new AppPermission{PermissionGroup = "Product", Name = "SoftDeleteProducts"},
                new AppPermission{PermissionGroup = "Product", Name = "HardDeleteProducts" },
                new AppPermission{PermissionGroup = "Product", Name = "HideProducts"},
                new AppPermission{PermissionGroup = "Product", Name = "ActivateProducts"},
                new AppPermission{PermissionGroup = "Product", Name = "DemoteProducts" },
                new AppPermission{PermissionGroup = "Product", Name = "PromoteProducts"},
                new AppPermission{PermissionGroup = "Product", Name = "CreateProductSale" },
                new AppPermission{PermissionGroup = "Product", Name = "AddProductPhoto"},
                new AppPermission{PermissionGroup = "Product", Name = "AddProductVideo"},
                new AppPermission{PermissionGroup = "Product", Name = "SetProductMainPhoto" },
                new AppPermission{PermissionGroup = "Product", Name = "DeleteProductPhoto"},
                new AppPermission{PermissionGroup = "Product", Name = "HideProductPhoto" },
                new AppPermission{PermissionGroup = "Product", Name = "ActivateProductPhoto"},

                new AppPermission{PermissionGroup = "Dashboard", Name = "DashboardAccess"},
                new AppPermission{PermissionGroup = "Dashboard", Name = "ViewDashboardOrderStatus"},
                new AppPermission{PermissionGroup = "Dashboard", Name = "ViewDashboardPopularProduct" },
                new AppPermission{PermissionGroup = "Dashboard", Name = "ViewDashboardOrderRate"},
                new AppPermission{PermissionGroup = "Dashboard", Name = "ViewDashboardRevenue" },

                new AppPermission{PermissionGroup = "File", Name = "UploadFile"},
                new AppPermission{PermissionGroup = "File", Name = "UploadImage"},

            };

            await permissions.ForEachAsync(async permission =>
            {
                await roleManager.CreateAsync(permission);
            });
        }


        public static async Task SeedRoles(DataContext context, RoleManager<AppPermission> roleManager)
        {
            if (await context.AppRoles.AnyAsync()) return;

            var roles = new List<AppRole>
            {
                new AppRole { RoleName = "Administrator" },
                new AppRole { RoleName = "Manager" },
                new AppRole { RoleName = "Sales" },
                new AppRole { RoleName = "Customer" },
            };

            await roles.ForEachAsync(async role =>
            {
                await context.AppRoles.AddAsync(role);
            });

            await SeedPermissions(roleManager);

            await context.SaveChangesAsync();
        }

        public static async Task SeedProducts(DataContext context)
        {
            if (await context.Products.AnyAsync()) return;

            var productData = await System.IO.File.ReadAllTextAsync("Data/SeedData/ProductSeedData.json");

            var products = JsonSerializer.Deserialize<List<Product>>(productData);

            await products.ForEachAsync(async product =>
            {
                await context.Products.AddAsync(product);
            });

            await context.SaveChangesAsync();
        }

        public static async Task SeedCategories(DataContext context)
        {
            if (await context.Categories.AnyAsync()) return;

            var categoryData = await System.IO.File.ReadAllTextAsync("Data/SeedData/CategorySeedData.json");

            var categories = JsonSerializer.Deserialize<List<Category>>(categoryData);

            await categories.ForEachAsync(async category =>
           {
               category.Slug = category.CategoryName.GenerateSlug();
               await context.Categories.AddAsync(category);
           });


            await context.SaveChangesAsync();
        }

        public static async Task SeedBrands(DataContext context)
        {
            if (await context.Brands.AnyAsync()) return;

            var brandData = await System.IO.File.ReadAllTextAsync("Data/SeedData/BrandSeedData.json");

            var brands = JsonSerializer.Deserialize<List<Brand>>(brandData);

            await brands.ForEachAsync(async brand =>
           {
               await context.Brands.AddAsync(brand);
           });

            await context.SaveChangesAsync();
        }

        // public static async Task SeedColors(DataContext context)
        // {
        //     if (await context.Colors.AnyAsync()) return;

        //     var colorData = await System.IO.File.ReadAllTextAsync("Data/SeedData/ColorSeedData.json");

        //     var colors = JsonSerializer.Deserialize<List<Color>>(colorData);

        //     foreach (var color in colors)
        //     {
        //         await context.Colors.AddAsync(color);
        //     }

        //     await context.SaveChangesAsync();
        // }

        // public static async Task SeedSizes(DataContext context)
        // {
        //     if (await context.Sizes.AnyAsync()) return;

        //     var sizeData = await System.IO.File.ReadAllTextAsync("Data/SeedData/SizeSeedData.json");

        //     var sizes = JsonSerializer.Deserialize<List<Size>>(sizeData);

        //     foreach (var size in sizes)
        //     {
        //         await context.Sizes.AddAsync(size);
        //     }

        //     await context.SaveChangesAsync();
        // }


        //     public static async Task SeedSubCategories(DataContext context)
        //     {
        //         if (await context.SubCategories.AnyAsync()) return;

        //         var subCategoryData = await System.IO.File.ReadAllTextAsync("Data/SeedData/SubCategorySeedData.json");

        //         var subCategories = JsonSerializer.Deserialize<List<SubCategory>>(subCategoryData);

        //         foreach ( var subCategory in subCategories)
        //         {
        //             subCategory.SubCategoryName = subCategory.SubCategoryName.ToLower();

        //             await context.SubCategories.AddAsync(subCategory);
        //         }

        //         await context.SaveChangesAsync();
        //     }

        //     public static async Task SeedColors(DataContext context)
        //     {
        //         if (await context.Colors.AnyAsync()) return;

        //         var colorData = await System.IO.File.ReadAllTextAsync("Data/SeedData/ColorSeedData.json");

        //         var colors = JsonSerializer.Deserialize<List<Color>>(colorData);

        //         foreach ( var color in colors)
        //         {
        //             color.ColorName = color.ColorName.ToLower();
        //             color.HexCode = color.HexCode.ToLower();
        //             await context.Colors.AddAsync(color);
        //         }

        //         await context.SaveChangesAsync();
        //     }

        //     public static async Task SeedBrands(DataContext context)
        //     {
        //         if (await context.Brands.AnyAsync()) return;

        //         var brandData = await System.IO.File.ReadAllTextAsync("Data/SeedData/BrandSeedData.json");

        //         var brands = JsonSerializer.Deserialize<List<Brand>>(brandData);

        //         foreach ( var brand in brands)
        //         {
        //             brand.BrandName = brand.BrandName.ToLower();

        //             await context.Brands.AddAsync(brand);
        //         }

        //         await context.SaveChangesAsync();
        //     }

        //     public static async Task SeedCollections(DataContext context)
        //     {
        //         if (await context.Collections.AnyAsync()) return;

        //         var collectionData = await System.IO.File.ReadAllTextAsync("Data/SeedData/CollectionSeedData.json");

        //         var collections = JsonSerializer.Deserialize<List<Collection>>(collectionData);

        //         foreach ( var collection in collections)
        //         {
        //             collection.CollectionName = collection.CollectionName.ToLower();

        //             await context.Collections.AddAsync(collection);
        //         }()

        //         await context.SaveChangesAsync();
        //     }

        //     public static async Task SeedShippingMethods(DataContext context)
        //     {
        //         if (await context.ShippingMethods.AnyAsync()) return;

        //         var shippingMethodData = await System.IO.File.ReadAllTextAsync("Data/SeedData/ShippingMethodSeedData.json");

        //         var shippingMethods = JsonSerializer.Deserialize<List<ShippingMethod>>(shippingMethodData);

        //         foreach ( var shippingMethod in shippingMethods)
        //         {
        //             shippingMethod.Name = shippingMethod.Name.ToLower();

        //             context.ShippingMethods.Add(shippingMethod);
        //         }

        //         await context.SaveChangesAsync();
        //     }

        //     public static async Task SeedPromotions(DataContext context)
        //     {
        //         if (await context.Promotions.AnyAsync()) return;

        //         var promotionData = await System.IO.File.ReadAllTextAsync("Data/SeedData/PromotionSeedData.json");

        //         var promotions = JsonSerializer.Deserialize<List<Promotion>>(promotionData);

        //         foreach ( var promotion in promotions)
        //         {
        //             context.Promotions.Add(promotion);
        //         }

        //         await context.SaveChangesAsync();
        //     }

        //     public static async Task SeedPaymentMethods(DataContext context)
        //     {
        //         if (await context.PaymentMethods.AnyAsync()) return;

        //         var paymentData = await System.IO.File.ReadAllTextAsync("Data/SeedData/PaymentMethodSeedData.json");

        //         var payments = JsonSerializer.Deserialize<List<PaymentMethod>>(paymentData);

        //         foreach ( var payment in payments)
        //         {
        //             context.PaymentMethods.Add(payment);
        //         }

        //         await context.SaveChangesAsync();
        //     }

        //     public static async Task SeedAreas(DataContext context)
        //     {
        //         if (await context.Areas.AnyAsync()) return;

        //         var areaData = await System.IO.File.ReadAllTextAsync("Data/SeedData/AreaSeedData.json");

        //         var areas = JsonSerializer.Deserialize<List<Area>>(areaData);

        //         foreach ( var area in areas)
        //         {
        //             area.Name = area.Name.ToLower();
        //             context.Areas.Add(area);
        //         }

        //         await context.SaveChangesAsync();
        //     }
    }
}
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class UpdateBaseEntity2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserLikes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SubCategories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrderHistories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "HomePages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FeatureProducts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FeatureCategories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Brands");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "UserLikes",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "SubCategories",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "Stocks",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "ProductPhotos",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "Orders",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "OrderHistories",
                newName: "OrderStatus");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "OrderDetails",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "Options",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "HomePages",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "Files",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "FeatureProducts",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "FeatureCategories",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "Categories",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "Carts",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "Carousels",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IsHidden",
                table: "Brands",
                newName: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "UserLikes",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "SubCategories",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Stocks",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ProductPhotos",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Orders",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "OrderStatus",
                table: "OrderHistories",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "OrderDetails",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Options",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "HomePages",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Files",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "FeatureProducts",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "FeatureCategories",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Categories",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Carts",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Carousels",
                newName: "IsHidden");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Brands",
                newName: "IsHidden");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserLikes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SubCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Stocks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProductPhotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrderHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrderDetails",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Options",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "HomePages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Files",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FeatureProducts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FeatureCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Carts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Carousels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Brands",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}

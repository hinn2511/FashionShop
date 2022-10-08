using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class UpdateBaseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "UserLikes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "UserLikes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserLikes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "UserLikes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "SubCategories",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "SubCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SubCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "SubCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "Stocks",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "Stocks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Stocks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Stocks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "ProductPhotos",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "ProductPhotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProductPhotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "ProductPhotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "OrderHistories",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "OrderHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrderHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "OrderHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "OrderDetails",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "OrderDetails",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OrderDetails",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "OrderDetails",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "Options",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "Options",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Options",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Options",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "HomePages",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "HomePages",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "HomePages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "HomePages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "Files",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "Files",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Files",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Files",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "FeatureProducts",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "FeatureProducts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FeatureProducts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "FeatureProducts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "FeatureCategories",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "FeatureCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FeatureCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "FeatureCategories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "Categories",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "Carts",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "Carts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Carts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Carts",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "Carousels",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "Carousels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Carousels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Carousels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "Brands",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "Brands",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Brands",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "Brands",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "UserLikes");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "UserLikes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserLikes");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "UserLikes");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "SubCategories");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "SubCategories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SubCategories");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "SubCategories");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "OrderHistories");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "OrderHistories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrderHistories");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "OrderHistories");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "HomePages");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "HomePages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "HomePages");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "HomePages");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "FeatureProducts");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "FeatureProducts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FeatureProducts");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "FeatureProducts");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "FeatureCategories");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "FeatureCategories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FeatureCategories");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "FeatureCategories");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "Brands");
        }
    }
}

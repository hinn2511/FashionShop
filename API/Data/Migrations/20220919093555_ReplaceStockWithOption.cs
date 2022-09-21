using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class ReplaceStockWithOption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Stocks_StockId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Colors_ColorId",
                table: "Stocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Sizes_SizeId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_ColorId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "AdditionalPrice",
                table: "Stocks");

            migrationBuilder.RenameColumn(
                name: "SizeId",
                table: "Stocks",
                newName: "OptionId");

            migrationBuilder.RenameColumn(
                name: "ColorId",
                table: "Stocks",
                newName: "LastUpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Stocks_SizeId",
                table: "Stocks",
                newName: "IX_Stocks_OptionId");

            migrationBuilder.RenameColumn(
                name: "StockId",
                table: "OrderDetails",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_StockId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_ProductId");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Stocks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Stocks",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "Stocks",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Stocks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Stocks",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "OptionId",
                table: "OrderDetails",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Options",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    ColorId = table.Column<int>(type: "INTEGER", nullable: false),
                    SizeId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdditionalPrice = table.Column<double>(type: "REAL", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateDeleted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeletedByUserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Options", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Options_Colors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Options_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Options_Sizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "Sizes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OptionId",
                table: "OrderDetails",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Options_ColorId",
                table: "Options",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Options_ProductId",
                table: "Options",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Options_SizeId",
                table: "Options",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Options_OptionId",
                table: "OrderDetails",
                column: "OptionId",
                principalTable: "Options",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Options_OptionId",
                table: "Stocks",
                column: "OptionId",
                principalTable: "Options",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Options_OptionId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Products_ProductId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Options_OptionId",
                table: "Stocks");

            migrationBuilder.DropTable(
                name: "Options");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OptionId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "OptionId",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "OptionId",
                table: "Stocks",
                newName: "SizeId");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedByUserId",
                table: "Stocks",
                newName: "ColorId");

            migrationBuilder.RenameIndex(
                name: "IX_Stocks_OptionId",
                table: "Stocks",
                newName: "IX_Stocks_SizeId");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderDetails",
                newName: "StockId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_ProductId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_StockId");

            migrationBuilder.AddColumn<double>(
                name: "AdditionalPrice",
                table: "Stocks",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ColorId",
                table: "Stocks",
                column: "ColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Stocks_StockId",
                table: "OrderDetails",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Colors_ColorId",
                table: "Stocks",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Sizes_SizeId",
                table: "Stocks",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

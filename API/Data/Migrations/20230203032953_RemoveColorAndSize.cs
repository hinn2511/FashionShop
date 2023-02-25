using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class RemoveColorAndSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Options_Colors_ColorId",
                table: "Options");

            migrationBuilder.DropForeignKey(
                name: "FK_Options_Sizes_SizeId",
                table: "Options");

            // migrationBuilder.DropTable(
            //     name: "Colors");

            migrationBuilder.DropTable(
                name: "Sizes");

            migrationBuilder.DropIndex(
                name: "IX_Options_ColorId",
                table: "Options");

            migrationBuilder.DropIndex(
                name: "IX_Options_SizeId",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "Options");

            migrationBuilder.AddColumn<string>(
                name: "ColorCode",
                table: "Options",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ColorName",
                table: "Options",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SizeName",
                table: "Options",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorCode",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "ColorName",
                table: "Options");

            migrationBuilder.DropColumn(
                name: "SizeName",
                table: "Options");

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "Options",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "Options",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ColorCode = table.Column<string>(type: "TEXT", nullable: true),
                    ColorName = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateDeleted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateHidden = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeletedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    HiddenByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateDeleted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateHidden = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeletedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    HiddenByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    SizeName = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sizes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Options_ColorId",
                table: "Options",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Options_SizeId",
                table: "Options",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Options_Colors_ColorId",
                table: "Options",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Options_Sizes_SizeId",
                table: "Options",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

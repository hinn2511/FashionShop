using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class UpdateProductPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPhotos_Photos_PhotoId",
                table: "ProductPhotos");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_ProductPhotos_PhotoId",
                table: "ProductPhotos");

            migrationBuilder.RenameColumn(
                name: "PhotoId",
                table: "ProductPhotos",
                newName: "LastUpdatedByUserId");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "ProductPhotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "ProductPhotos",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "ProductPhotos",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "ProductPhotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "ProductPhotos",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "ProductPhotos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "ProductPhotos",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "ProductPhotos");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedByUserId",
                table: "ProductPhotos",
                newName: "PhotoId");

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PublicId = table.Column<string>(type: "TEXT", nullable: true),
                    Url = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductPhotos_PhotoId",
                table: "ProductPhotos",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPhotos_Photos_PhotoId",
                table: "ProductPhotos",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

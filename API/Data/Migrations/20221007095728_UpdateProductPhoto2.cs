using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class UpdateProductPhoto2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "ProductPhotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductPhotos_FileId",
                table: "ProductPhotos",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPhotos_Files_FileId",
                table: "ProductPhotos",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPhotos_Files_FileId",
                table: "ProductPhotos");

            migrationBuilder.DropIndex(
                name: "IX_ProductPhotos_FileId",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "ProductPhotos");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class UpdateCloudinary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Files_FileId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_FileId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "Photos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "Photos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Photos_FileId",
                table: "Photos",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Files_FileId",
                table: "Photos",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

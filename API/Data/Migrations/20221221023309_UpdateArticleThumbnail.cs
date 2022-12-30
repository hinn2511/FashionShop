using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class UpdateArticleThumbnail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Photos_PhotoId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_PhotoId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Articles");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Articles",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Articles");

            migrationBuilder.AddColumn<int>(
                name: "PhotoId",
                table: "Articles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_PhotoId",
                table: "Articles",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Photos_PhotoId",
                table: "Articles",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

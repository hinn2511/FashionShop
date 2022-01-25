using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class PhotoAndProductPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "ProductPhotos");

            migrationBuilder.AddColumn<int>(
                name: "PhotoId",
                table: "ProductPhotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    PublicId = table.Column<string>(type: "TEXT", nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPhotos_Photos_PhotoId",
                table: "ProductPhotos");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_ProductPhotos_PhotoId",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "ProductPhotos");

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
    }
}

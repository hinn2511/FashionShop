using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class UpdateEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Note",
                table: "OrderHistories",
                newName: "HistoryDescription");

            migrationBuilder.RenameColumn(
                name: "CategoryUrl",
                table: "FeatureCategories",
                newName: "Link");

            migrationBuilder.RenameColumn(
                name: "TextLink",
                table: "Carousels",
                newName: "Link");

            migrationBuilder.RenameColumn(
                name: "Text",
                table: "Carousels",
                newName: "Header");

            migrationBuilder.AddColumn<int>(
                name: "FileType",
                table: "ProductPhotos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Orders",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "FeatureProducts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ButtonText",
                table: "Carousels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Carousels",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileType",
                table: "ProductPhotos");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "FeatureProducts");

            migrationBuilder.DropColumn(
                name: "ButtonText",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Carousels");

            migrationBuilder.RenameColumn(
                name: "HistoryDescription",
                table: "OrderHistories",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "Link",
                table: "FeatureCategories",
                newName: "CategoryUrl");

            migrationBuilder.RenameColumn(
                name: "Link",
                table: "Carousels",
                newName: "TextLink");

            migrationBuilder.RenameColumn(
                name: "Header",
                table: "Carousels",
                newName: "Text");
        }
    }
}

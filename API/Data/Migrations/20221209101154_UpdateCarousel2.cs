using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class UpdateCarousel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NavigationText",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "TextPaddingBottom",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "TextPaddingLeft",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "TextPaddingRight",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "TextPaddingTop",
                table: "Carousels");

            migrationBuilder.DropColumn(
                name: "TextPosition",
                table: "Carousels");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NavigationText",
                table: "Carousels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TextPaddingBottom",
                table: "Carousels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TextPaddingLeft",
                table: "Carousels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TextPaddingRight",
                table: "Carousels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TextPaddingTop",
                table: "Carousels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TextPosition",
                table: "Carousels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}

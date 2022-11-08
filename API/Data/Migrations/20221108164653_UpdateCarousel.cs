using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class UpdateCarousel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carousels_HomePages_HomePageId",
                table: "Carousels");

            migrationBuilder.RenameColumn(
                name: "Header",
                table: "Carousels",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "ButtonText",
                table: "Carousels",
                newName: "NavigationText");

            migrationBuilder.AlterColumn<int>(
                name: "HomePageId",
                table: "Carousels",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Carousels_HomePages_HomePageId",
                table: "Carousels",
                column: "HomePageId",
                principalTable: "HomePages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carousels_HomePages_HomePageId",
                table: "Carousels");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Carousels",
                newName: "Header");

            migrationBuilder.RenameColumn(
                name: "NavigationText",
                table: "Carousels",
                newName: "ButtonText");

            migrationBuilder.AlterColumn<int>(
                name: "HomePageId",
                table: "Carousels",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Carousels_HomePages_HomePageId",
                table: "Carousels",
                column: "HomePageId",
                principalTable: "HomePages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

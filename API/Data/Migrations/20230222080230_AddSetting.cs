using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class AddSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientLoginBackground = table.Column<string>(type: "TEXT", nullable: true),
                    ClientLoginPhotoId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClientRegisterBackground = table.Column<string>(type: "TEXT", nullable: true),
                    ClientRegisterPhotoId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdministratorLoginBackground = table.Column<string>(type: "TEXT", nullable: true),
                    AdministratorLoginPhotoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Settings");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class AddUserReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "OrderDetails",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReviewEditCount",
                table: "OrderDetails",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    Score = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdatedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateDeleted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DeletedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateHidden = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HiddenByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserReviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserReviews_Options_OptionId",
                        column: x => x.OptionId,
                        principalTable: "Options",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserReviews_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_OptionId",
                table: "UserReviews",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_OrderId",
                table: "UserReviews",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserReviews_UserId",
                table: "UserReviews",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserReviews");

            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ReviewEditCount",
                table: "OrderDetails");
        }
    }
}

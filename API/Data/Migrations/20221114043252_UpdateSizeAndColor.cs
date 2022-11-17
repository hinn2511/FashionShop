using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class UpdateSizeAndColor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Sizes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Sizes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "Sizes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "Sizes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Sizes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "Sizes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Sizes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LastUpdatedByUserId",
                table: "Sizes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Sizes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Colors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Colors",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateDeleted",
                table: "Colors",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateHidden",
                table: "Colors",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                table: "Colors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HiddenByUserId",
                table: "Colors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Colors",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LastUpdatedByUserId",
                table: "Colors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Colors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "LastUpdatedByUserId",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Sizes");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "DateDeleted",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "DateHidden",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "HiddenByUserId",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "LastUpdatedByUserId",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Colors");
        }
    }
}

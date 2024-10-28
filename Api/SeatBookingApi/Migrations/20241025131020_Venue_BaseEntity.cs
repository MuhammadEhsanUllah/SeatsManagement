using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatBookingApi.Migrations
{
    public partial class Venue_BaseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Venues",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "Venues",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Venues",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "Venues");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Venues");
        }
    }
}

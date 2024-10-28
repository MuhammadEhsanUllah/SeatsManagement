using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatBookingApi.Migrations
{
    public partial class VenueSection_BaseEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "VenueSections",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "VenueSections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "VenueSections",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "VenueSections");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "VenueSections");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "VenueSections");
        }
    }
}

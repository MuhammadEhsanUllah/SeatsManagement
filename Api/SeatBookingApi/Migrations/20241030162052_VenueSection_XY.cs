using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatBookingApi.Migrations
{
    public partial class VenueSection_XY : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "X",
                table: "VenueSections",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Y",
                table: "VenueSections",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "X",
                table: "VenueSections");

            migrationBuilder.DropColumn(
                name: "Y",
                table: "VenueSections");
        }
    }
}

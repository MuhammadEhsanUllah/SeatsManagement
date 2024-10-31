using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatBookingApi.Migrations
{
    public partial class Seat_ClientId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Seats",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Seats");
        }
    }
}

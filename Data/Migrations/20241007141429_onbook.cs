using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mio_Rest_Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class onbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OccupationStatusOnBook",
                table: "Reservations",
                newName: "OccupationStatusSoirOnBook");

            migrationBuilder.AddColumn<string>(
                name: "FreeTable1330",
                table: "Reservations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OccupationStatusMidiOnBook",
                table: "Reservations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreeTable1330",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "OccupationStatusMidiOnBook",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "OccupationStatusSoirOnBook",
                table: "Reservations",
                newName: "OccupationStatusOnBook");
        }
    }
}

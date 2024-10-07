using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mio_Rest_Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class occstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OccStatus",
                table: "OccupationStatus",
                newName: "OccStatusMidi");

            migrationBuilder.AddColumn<string>(
                name: "OccStatusDiner",
                table: "OccupationStatus",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OccStatusDiner",
                table: "OccupationStatus");

            migrationBuilder.RenameColumn(
                name: "OccStatusMidi",
                table: "OccupationStatus",
                newName: "OccStatus");
        }
    }
}

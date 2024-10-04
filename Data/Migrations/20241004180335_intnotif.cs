using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mio_Rest_Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class intnotif : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Toggles");

            migrationBuilder.AddColumn<int>(
                name: "NotificationCount",
                table: "Toggles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationCount",
                table: "Toggles");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Toggles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

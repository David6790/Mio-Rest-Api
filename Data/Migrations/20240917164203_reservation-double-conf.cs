using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mio_Rest_Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class reservationdoubleconf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DoubleConfirmation",
                table: "Reservations",
                type: "varchar(1)",
                unicode: false,
                maxLength: 1,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoubleConfirmation",
                table: "Reservations");
        }
    }
}

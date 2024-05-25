using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mio_Rest_Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class nomprenom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nom",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Prenom",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nom",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Prenom",
                table: "Users");
        }
    }
}

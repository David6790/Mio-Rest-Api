using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mio_Rest_Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class libelle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Libelle",
                table: "HECStatuts",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Libelle",
                table: "HECStatuts");
        }
    }
}

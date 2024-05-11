using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mio_Rest_Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ajoutMenuJour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Menu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Entree = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Plat = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Cheesecake = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DessertJour = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Menu");
        }
    }
}

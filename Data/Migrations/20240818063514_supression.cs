using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mio_Rest_Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class supression : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Allocations_TableId_Date_Period",
                table: "Allocations");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_TableId",
                table: "Allocations",
                column: "TableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Allocations_TableId",
                table: "Allocations");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_TableId_Date_Period",
                table: "Allocations",
                columns: new[] { "TableId", "Date", "Period" },
                unique: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mio_Rest_Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class test3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allocation_Reservations_ReservationId",
                table: "Allocation");

            migrationBuilder.DropForeignKey(
                name: "FK_Allocation_TableEntity_TableId",
                table: "Allocation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TableEntity",
                table: "TableEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Allocation",
                table: "Allocation");

            migrationBuilder.RenameTable(
                name: "TableEntity",
                newName: "Tables");

            migrationBuilder.RenameTable(
                name: "Allocation",
                newName: "Allocations");

            migrationBuilder.RenameIndex(
                name: "IX_Allocation_TableId_Date_Period",
                table: "Allocations",
                newName: "IX_Allocations_TableId_Date_Period");

            migrationBuilder.RenameIndex(
                name: "IX_Allocation_ReservationId",
                table: "Allocations",
                newName: "IX_Allocations_ReservationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tables",
                table: "Tables",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Allocations",
                table: "Allocations",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_Reservations_ReservationId",
                table: "Allocations",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_Tables_TableId",
                table: "Allocations",
                column: "TableId",
                principalTable: "Tables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_Reservations_ReservationId",
                table: "Allocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_Tables_TableId",
                table: "Allocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tables",
                table: "Tables");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Allocations",
                table: "Allocations");

            migrationBuilder.RenameTable(
                name: "Tables",
                newName: "TableEntity");

            migrationBuilder.RenameTable(
                name: "Allocations",
                newName: "Allocation");

            migrationBuilder.RenameIndex(
                name: "IX_Allocations_TableId_Date_Period",
                table: "Allocation",
                newName: "IX_Allocation_TableId_Date_Period");

            migrationBuilder.RenameIndex(
                name: "IX_Allocations_ReservationId",
                table: "Allocation",
                newName: "IX_Allocation_ReservationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TableEntity",
                table: "TableEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Allocation",
                table: "Allocation",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Allocation_Reservations_ReservationId",
                table: "Allocation",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Allocation_TableEntity_TableId",
                table: "Allocation",
                column: "TableId",
                principalTable: "TableEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

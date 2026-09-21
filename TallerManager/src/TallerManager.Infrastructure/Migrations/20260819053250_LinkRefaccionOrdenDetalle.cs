using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TallerManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkRefaccionOrdenDetalle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RefaccionId",
                table: "OrdenDetalles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenDetalles_RefaccionId",
                table: "OrdenDetalles",
                column: "RefaccionId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenDetalles_Refacciones_RefaccionId",
                table: "OrdenDetalles",
                column: "RefaccionId",
                principalTable: "Refacciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdenDetalles_Refacciones_RefaccionId",
                table: "OrdenDetalles");

            migrationBuilder.DropIndex(
                name: "IX_OrdenDetalles_RefaccionId",
                table: "OrdenDetalles");

            migrationBuilder.DropColumn(
                name: "RefaccionId",
                table: "OrdenDetalles");
        }
    }
}

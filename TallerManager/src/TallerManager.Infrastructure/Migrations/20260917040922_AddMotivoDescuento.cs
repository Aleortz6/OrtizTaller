using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TallerManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMotivoDescuento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MotivoDescuento",
                table: "Ordenes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MotivoDescuento",
                table: "Ordenes");
        }
    }
}

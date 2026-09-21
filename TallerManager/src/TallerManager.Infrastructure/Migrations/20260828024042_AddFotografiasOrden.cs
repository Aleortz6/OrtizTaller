using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TallerManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFotografiasOrden : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FotografiasOrden",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TallerId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrdenId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    RutaArchivo = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: true),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotografiasOrden", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FotografiasOrden_Ordenes_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "Ordenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FotografiasOrden_OrdenId",
                table: "FotografiasOrden",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_FotografiasOrden_TallerId",
                table: "FotografiasOrden",
                column: "TallerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FotografiasOrden");
        }
    }
}

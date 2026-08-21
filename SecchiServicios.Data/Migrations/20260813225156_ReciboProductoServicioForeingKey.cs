using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecchiServicios.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReciboProductoServicioForeingKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recibos_ProductosServicios_ProductoServicioId",
                table: "Recibos");

            migrationBuilder.DropIndex(
                name: "IX_Recibos_ProductoServicioId",
                table: "Recibos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Recibos_ProductoServicioId",
                table: "Recibos",
                column: "ProductoServicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recibos_ProductosServicios_ProductoServicioId",
                table: "Recibos",
                column: "ProductoServicioId",
                principalTable: "ProductosServicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

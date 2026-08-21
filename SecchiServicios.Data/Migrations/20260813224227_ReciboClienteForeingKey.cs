using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecchiServicios.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReciboClienteForeingKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recibos_Clientes_ClienteId",
                table: "Recibos");

            migrationBuilder.DropIndex(
                name: "IX_Recibos_ClienteId",
                table: "Recibos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Recibos_ClienteId",
                table: "Recibos",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recibos_Clientes_ClienteId",
                table: "Recibos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

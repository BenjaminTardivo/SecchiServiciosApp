using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecchiServicios.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedLocalidadToCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocalidadId",
                table: "Clientes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_LocalidadId",
                table: "Clientes",
                column: "LocalidadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Localidades_LocalidadId",
                table: "Clientes",
                column: "LocalidadId",
                principalTable: "Localidades",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Localidades_LocalidadId",
                table: "Clientes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_LocalidadId",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "LocalidadId",
                table: "Clientes");
        }
    }
}

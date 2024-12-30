using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversidadWEB.Migrations
{
    /// <inheritdoc />
    public partial class FixColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Credito",
                table: "Cursos",
                newName: "Creditos");

            migrationBuilder.RenameColumn(
                name: "catedraticoID",
                table: "Catedraticos",
                newName: "CatedraticoID");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Catedraticos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Nombres",
                table: "Alumnos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Creditos",
                table: "Cursos",
                newName: "Credito");

            migrationBuilder.RenameColumn(
                name: "CatedraticoID",
                table: "Catedraticos",
                newName: "catedraticoID");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Catedraticos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombres",
                table: "Alumnos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}

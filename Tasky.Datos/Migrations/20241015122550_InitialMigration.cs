using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tasky.Datos.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuario_email",
                table: "Usuario");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "Usuario",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_email",
                table: "Usuario",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuario_email",
                table: "Usuario");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "Usuario",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_email",
                table: "Usuario",
                column: "email",
                unique: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Atmos.Web.Migrations
{
    public partial class removeIndexing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movies_Path",
                table: "Movies");

            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "Movies",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Path",
                table: "Movies",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Path",
                table: "Movies",
                column: "Path",
                unique: true);
        }
    }
}

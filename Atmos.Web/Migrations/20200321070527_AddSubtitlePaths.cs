using Microsoft.EntityFrameworkCore.Migrations;

namespace Atmos.Web.Migrations
{
    public partial class AddSubtitlePaths : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubtitlePath",
                table: "Movies",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubtitlePath",
                table: "Movies");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Atmos.Web.Migrations
{
    public partial class AddSubtitles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubtitlePath",
                table: "Movies");

            migrationBuilder.CreateTable(
                name: "Subtitles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Language = table.Column<string>(nullable: false),
                    Path = table.Column<string>(nullable: false),
                    MovieId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subtitles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subtitles_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subtitles_MovieId",
                table: "Subtitles",
                column: "MovieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subtitles");

            migrationBuilder.AddColumn<string>(
                name: "SubtitlePath",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

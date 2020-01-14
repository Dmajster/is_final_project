using Microsoft.EntityFrameworkCore.Migrations;

namespace MusicShare.Migrations
{
    public partial class AddedThumbnailLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailLink",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailLink",
                table: "Posts");
        }
    }
}

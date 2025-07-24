using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirJourney_Blog.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddFileIdOnBlogPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileId",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileId",
                table: "BlogPosts");
        }
    }
}

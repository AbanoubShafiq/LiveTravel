using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirJourney_Blog.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addSoftDeleteForBlogAndCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "BlogPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "BlogCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "BlogCategories");
        }
    }
}

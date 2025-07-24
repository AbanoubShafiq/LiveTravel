using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirJourney_Blog.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameBlogNameAndContentToLocalizedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "BlogPosts",
                newName: "TitleEs");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "BlogPosts",
                newName: "ContentEs");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPosts_Title_BlogCategoryId",
                table: "BlogPosts",
                newName: "IX_BlogPosts_TitleEs_BlogCategoryId");

            migrationBuilder.AddColumn<string>(
                name: "ContentEn",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "BlogPosts",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_TitleEn_BlogCategoryId",
                table: "BlogPosts",
                columns: new[] { "TitleEn", "BlogCategoryId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_TitleEn_BlogCategoryId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ContentEn",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "BlogPosts");

            migrationBuilder.RenameColumn(
                name: "TitleEs",
                table: "BlogPosts",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "ContentEs",
                table: "BlogPosts",
                newName: "Content");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPosts_TitleEs_BlogCategoryId",
                table: "BlogPosts",
                newName: "IX_BlogPosts_Title_BlogCategoryId");
        }
    }
}

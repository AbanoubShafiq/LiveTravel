using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirJourney_Blog.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RenameCategoryNameToLocalizedNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "BlogCategories",
                newName: "NameEs");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "BlogCategories",
                newName: "DescriptionEs");

            migrationBuilder.RenameIndex(
                name: "IX_BlogCategories_Name",
                table: "BlogCategories",
                newName: "IX_BlogCategories_NameEs");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "BlogCategories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NameEn",
                table: "BlogCategories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategories_NameEn",
                table: "BlogCategories",
                column: "NameEn",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlogCategories_NameEn",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "NameEn",
                table: "BlogCategories");

            migrationBuilder.RenameColumn(
                name: "NameEs",
                table: "BlogCategories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DescriptionEs",
                table: "BlogCategories",
                newName: "Description");

            migrationBuilder.RenameIndex(
                name: "IX_BlogCategories_NameEs",
                table: "BlogCategories",
                newName: "IX_BlogCategories_Name");
        }
    }
}

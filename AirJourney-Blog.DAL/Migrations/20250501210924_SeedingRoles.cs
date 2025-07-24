using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirJourney_Blog.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedingRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[,]
                {
                    {
                        "Admin",
                        "ADMIN",
                        Guid.NewGuid().ToString()
                    },
                    {
                        "Customer",
                        "CUSTOMER",
                        Guid.NewGuid().ToString()
                    }
                        });
                }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    DELETE FROM [AspNetRoles] 
                    WHERE [Name] IN ('Admin', 'Customer')
                ");
        }
    }
}

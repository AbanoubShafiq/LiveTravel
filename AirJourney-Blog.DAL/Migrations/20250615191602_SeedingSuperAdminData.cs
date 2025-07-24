using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirJourney_Blog.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedingSuperAdminData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert SuperAdmin role
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[]
                {
            "SuperAdmin",
            "SUPERADMIN",
            Guid.NewGuid().ToString()
                });

            // Insert SuperAdmin user with all required fields including IsActive
            var hasher = new PasswordHasher<IdentityUser>();
            var passwordHash = hasher.HashPassword(null, "SuperAdmin@123");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[]
                {
            "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
            "EmailConfirmed", "PasswordHash", "SecurityStamp",
            "FirstName", "LastName", "IsActive",
            "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled",
            "LockoutEnabled", "AccessFailedCount", "ConcurrencyStamp"
                },
                values: new object[]
                {
            "superadmin@example.com",
            "SUPERADMIN@EXAMPLE.COM",
            "superadmin@example.com",
            "SUPERADMIN@EXAMPLE.COM",
            true,    // EmailConfirmed
            passwordHash,
            Guid.NewGuid().ToString("D"),
            "Super", // FirstName
            "Admin", // LastName
            true,    // IsActive = true
            null,    // PhoneNumber
            false,   // PhoneNumberConfirmed
            false,   // TwoFactorEnabled
            false,   // LockoutEnabled
            0,       // AccessFailedCount
            Guid.NewGuid().ToString() // ConcurrencyStamp
                });

            // Assign role
            migrationBuilder.Sql(@"
                INSERT INTO AspNetUserRoles (UserId, RoleId)
                SELECT u.Id, r.Id
                FROM AspNetUsers u, AspNetRoles r
                WHERE u.UserName = 'superadmin@example.com'
                AND r.Name = 'SuperAdmin'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM AspNetUserRoles 
                WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE UserName = 'superadmin@example.com')
                AND RoleId IN (SELECT Id FROM AspNetRoles WHERE Name = 'SuperAdmin')
            ");

            migrationBuilder.Sql(@"
                DELETE FROM AspNetUsers WHERE UserName = 'superadmin@example.com'
            ");

            migrationBuilder.Sql(@"
                DELETE FROM AspNetRoles WHERE Name = 'SuperAdmin'
            ");

        }
    }
}




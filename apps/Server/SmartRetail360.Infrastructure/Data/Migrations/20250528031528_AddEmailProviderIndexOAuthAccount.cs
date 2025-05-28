using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailProviderIndexOAuthAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_oauth_accounts_Email_Provider",
                table: "oauth_accounts",
                columns: new[] { "Email", "Provider" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_oauth_accounts_Email_Provider",
                table: "oauth_accounts");
        }
    }
}

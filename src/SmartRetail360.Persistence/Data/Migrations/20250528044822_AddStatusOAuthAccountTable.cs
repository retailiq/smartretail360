#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusOAuthAccountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedAt",
                table: "oauth_accounts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeactivatedBy",
                table: "oauth_accounts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeactivationReason",
                table: "oauth_accounts",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "none");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "oauth_accounts",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "oauth_accounts",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeactivatedAt",
                table: "oauth_accounts");

            migrationBuilder.DropColumn(
                name: "DeactivatedBy",
                table: "oauth_accounts");

            migrationBuilder.DropColumn(
                name: "DeactivationReason",
                table: "oauth_accounts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "oauth_accounts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "oauth_accounts");
        }
    }
}

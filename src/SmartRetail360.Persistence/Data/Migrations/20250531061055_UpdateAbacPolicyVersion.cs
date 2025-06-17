#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAbacPolicyVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_abac_policies_TenantId_ResourceTypeId_ActionId_EnvironmentI~",
                table: "abac_policies");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "abac_policies");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "abac_policies",
                type: "uuid",
                maxLength: 64,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "VersionNumber",
                table: "abac_policies",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "abac_policies");

            migrationBuilder.DropColumn(
                name: "VersionNumber",
                table: "abac_policies");

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "abac_policies",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_abac_policies_TenantId_ResourceTypeId_ActionId_EnvironmentI~",
                table: "abac_policies",
                columns: new[] { "TenantId", "ResourceTypeId", "ActionId", "EnvironmentId", "Version" },
                unique: true);
        }
    }
}

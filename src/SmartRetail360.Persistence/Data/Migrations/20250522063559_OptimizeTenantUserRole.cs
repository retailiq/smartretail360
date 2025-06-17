#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeTenantUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tenant_users_Role",
                table: "tenant_users");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "tenant_users");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "tenant_users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_tenant_users_RoleId",
                table: "tenant_users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tenant_users_RoleId",
                table: "tenant_users");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "tenant_users");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "tenant_users",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "member");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_users_Role",
                table: "tenant_users",
                column: "Role");
        }
    }
}

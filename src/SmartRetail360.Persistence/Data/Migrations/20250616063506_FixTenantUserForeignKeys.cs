using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTenantUserForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_tenant_users_tenants_TenantId",
                table: "tenant_users",
                column: "TenantId",
                principalTable: "tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_users_roles_RoleId",
                table: "tenant_users",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tenant_users_tenants_TenantId",
                table: "tenant_users");

            migrationBuilder.DropForeignKey(
                name: "FK_tenant_users_roles_RoleId",
                table: "tenant_users");
        }
    }
}

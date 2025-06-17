#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNavPropertyTenantUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tenant_users_UserId",
                table: "tenant_users",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_tenant_users_users_UserId",
                table: "tenant_users",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tenant_users_users_UserId",
                table: "tenant_users");

            migrationBuilder.DropIndex(
                name: "IX_tenant_users_UserId",
                table: "tenant_users");
        }
    }
}

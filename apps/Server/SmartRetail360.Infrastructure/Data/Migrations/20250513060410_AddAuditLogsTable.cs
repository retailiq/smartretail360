using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tenants",
                table: "Tenants");

            migrationBuilder.RenameTable(
                name: "Tenants",
                newName: "tenants");

            migrationBuilder.RenameIndex(
                name: "IX_Tenants_Slug",
                table: "tenants",
                newName: "IX_tenants_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Tenants_DeletedAt",
                table: "tenants",
                newName: "IX_tenants_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Tenants_AdminEmail",
                table: "tenants",
                newName: "IX_tenants_AdminEmail");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenants",
                table: "tenants",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Action = table.Column<string>(type: "text", nullable: false),
                    IsSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    TraceId = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<Dictionary<string, object>>(type: "jsonb", nullable: true),
                    EvaluatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenants",
                table: "tenants");

            migrationBuilder.RenameTable(
                name: "tenants",
                newName: "Tenants");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_Slug",
                table: "Tenants",
                newName: "IX_Tenants_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_DeletedAt",
                table: "Tenants",
                newName: "IX_Tenants_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_AdminEmail",
                table: "Tenants",
                newName: "IX_Tenants_AdminEmail");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tenants",
                table: "Tenants",
                column: "Id");
        }
    }
}

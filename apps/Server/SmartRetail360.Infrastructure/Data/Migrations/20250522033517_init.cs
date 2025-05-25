using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LogId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Action = table.Column<string>(type: "text", nullable: false),
                    IsSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    TraceId = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string>(type: "jsonb", nullable: true),
                    EvaluatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Level = table.Column<string>(type: "text", nullable: false),
                    SourceModule = table.Column<string>(type: "text", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenant_users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "member"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeactivatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeactivatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeactivationReason = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "none"),
                    TraceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Slug = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Industry = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Size = table.Column<int>(type: "integer", nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "pending_verification"),
                    Plan = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "free"),
                    TraceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastUpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DeactivatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeactivatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeactivationReason = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "none")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Email = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "pending_verification"),
                    IsEmailVerified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsFirstLogin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    TraceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    LastUpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastEmailSentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    DeactivatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeactivatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeactivationReason = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, defaultValue: "none")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tenant_users_CreatedAt",
                table: "tenant_users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_users_DeactivatedAt",
                table: "tenant_users",
                column: "DeactivatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_users_DeletedAt",
                table: "tenant_users",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_users_IsActive",
                table: "tenant_users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_users_Role",
                table: "tenant_users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_users_TenantId_UserId",
                table: "tenant_users",
                columns: new[] { "TenantId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenant_users_TraceId",
                table: "tenant_users",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_CreatedAt",
                table: "tenants",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_DeactivatedAt",
                table: "tenants",
                column: "DeactivatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_DeletedAt",
                table: "tenants",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_IsActive",
                table: "tenants",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_Slug",
                table: "tenants",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_TraceId",
                table: "tenants",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_users_CreatedAt",
                table: "users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_users_DeactivatedAt",
                table: "users",
                column: "DeactivatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_users_DeletedAt",
                table: "users",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_IsActive",
                table: "users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_users_IsEmailVerified_Status",
                table: "users",
                columns: new[] { "IsEmailVerified", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_users_LastLoginAt",
                table: "users",
                column: "LastLoginAt");

            migrationBuilder.CreateIndex(
                name: "IX_users_TraceId",
                table: "users",
                column: "TraceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "tenant_users");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}

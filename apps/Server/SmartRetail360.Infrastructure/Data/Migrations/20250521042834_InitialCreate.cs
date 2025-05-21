using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                name: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Slug = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    AdminEmail = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Industry = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Size = table.Column<int>(type: "integer", nullable: true),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "pending_verification"),
                    Plan = table.Column<string>(type: "text", nullable: false, defaultValue: "free"),
                    EmailVerificationToken = table.Column<string>(type: "text", nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsFirstLogin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    TraceId = table.Column<string>(type: "text", nullable: false),
                    LastUpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastEmailSentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tenants_AdminEmail",
                table: "tenants",
                column: "AdminEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_DeletedAt",
                table: "tenants",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_Slug",
                table: "tenants",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "tenants");
        }
    }
}

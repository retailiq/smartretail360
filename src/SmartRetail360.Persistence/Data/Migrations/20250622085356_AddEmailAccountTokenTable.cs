using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailAccountTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_activation_tokens");
            
            migrationBuilder.CreateTable(
                name: "account_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "pending"),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TraceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false, defaultValue: "none")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_tokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_tokens_CreatedAt",
                table: "account_tokens",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_account_tokens_ExpiresAt",
                table: "account_tokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_account_tokens_Source",
                table: "account_tokens",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_account_tokens_Status",
                table: "account_tokens",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_account_tokens_TenantId",
                table: "account_tokens",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_account_tokens_Token",
                table: "account_tokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_account_tokens_UserId",
                table: "account_tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_account_tokens_UserId_Status_ExpiresAt",
                table: "account_tokens",
                columns: new[] { "UserId", "Status", "ExpiresAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_tokens");
            
            migrationBuilder.CreateTable(
                name: "account_activation_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false, defaultValue: "none"),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "pending"),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    TraceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_activation_tokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_activation_tokens_CreatedAt",
                table: "account_activation_tokens",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_account_activation_tokens_ExpiresAt",
                table: "account_activation_tokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_account_activation_tokens_Source",
                table: "account_activation_tokens",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_account_activation_tokens_Status",
                table: "account_activation_tokens",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_account_activation_tokens_TenantId",
                table: "account_activation_tokens",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_account_activation_tokens_Token",
                table: "account_activation_tokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_account_activation_tokens_UserId",
                table: "account_activation_tokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_account_activation_tokens_UserId_Status_ExpiresAt",
                table: "account_activation_tokens",
                columns: new[] { "UserId", "Status", "ExpiresAt" });
        }
    }
}

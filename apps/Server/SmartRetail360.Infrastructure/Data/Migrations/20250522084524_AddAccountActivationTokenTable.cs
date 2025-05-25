using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountActivationTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account_activation_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, defaultValue: "pending"),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TraceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
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
                name: "IX_account_activation_tokens_Status",
                table: "account_activation_tokens",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_account_activation_tokens_Token",
                table: "account_activation_tokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_account_activation_tokens_UserId",
                table: "account_activation_tokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_activation_tokens");
        }
    }
}

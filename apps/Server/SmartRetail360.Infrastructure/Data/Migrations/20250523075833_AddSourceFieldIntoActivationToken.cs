using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSourceFieldIntoActivationToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "account_activation_tokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_account_activation_tokens_Source",
                table: "account_activation_tokens",
                column: "Source");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_account_activation_tokens_Source",
                table: "account_activation_tokens");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "account_activation_tokens");
        }
    }
}

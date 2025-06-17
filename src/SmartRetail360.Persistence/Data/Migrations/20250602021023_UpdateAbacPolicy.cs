#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAbacPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowTemplateSync",
                table: "abac_policies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TemplateId",
                table: "abac_policies",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_abac_policies_TemplateId",
                table: "abac_policies",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_abac_policies_abac_policy_templates_TemplateId",
                table: "abac_policies",
                column: "TemplateId",
                principalTable: "abac_policy_templates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_abac_policies_abac_policy_templates_TemplateId",
                table: "abac_policies");

            migrationBuilder.DropIndex(
                name: "IX_abac_policies_TemplateId",
                table: "abac_policies");

            migrationBuilder.DropColumn(
                name: "AllowTemplateSync",
                table: "abac_policies");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "abac_policies");
        }
    }
}

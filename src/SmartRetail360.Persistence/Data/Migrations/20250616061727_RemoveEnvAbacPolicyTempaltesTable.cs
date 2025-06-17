using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEnvAbacPolicyTempaltesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Environment",
                table: "abac_policy_templates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Environment",
                table: "abac_policy_templates",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }
    }
}

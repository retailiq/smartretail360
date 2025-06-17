#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAbacTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "abac_resource_types",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppliesToRole",
                table: "abac_policies",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BasePolicyId",
                table: "abac_policies",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "abac_policy_templates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemplateName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ResourceType = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Action = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Environment = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RuleJson = table.Column<string>(type: "text", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_abac_policy_templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "abac_resource_groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_abac_resource_groups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_abac_resource_types_GroupId",
                table: "abac_resource_types",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_abac_policies_BasePolicyId",
                table: "abac_policies",
                column: "BasePolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_abac_policies_abac_policies_BasePolicyId",
                table: "abac_policies",
                column: "BasePolicyId",
                principalTable: "abac_policies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_abac_resource_types_abac_resource_groups_GroupId",
                table: "abac_resource_types",
                column: "GroupId",
                principalTable: "abac_resource_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_abac_policies_abac_policies_BasePolicyId",
                table: "abac_policies");

            migrationBuilder.DropForeignKey(
                name: "FK_abac_resource_types_abac_resource_groups_GroupId",
                table: "abac_resource_types");

            migrationBuilder.DropTable(
                name: "abac_policy_templates");

            migrationBuilder.DropTable(
                name: "abac_resource_groups");

            migrationBuilder.DropIndex(
                name: "IX_abac_resource_types_GroupId",
                table: "abac_resource_types");

            migrationBuilder.DropIndex(
                name: "IX_abac_policies_BasePolicyId",
                table: "abac_policies");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "abac_resource_types");

            migrationBuilder.DropColumn(
                name: "AppliesToRole",
                table: "abac_policies");

            migrationBuilder.DropColumn(
                name: "BasePolicyId",
                table: "abac_policies");
        }
    }
}

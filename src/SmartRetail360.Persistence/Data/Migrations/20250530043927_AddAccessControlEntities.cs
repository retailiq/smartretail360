#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessControlEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "abac_actions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_abac_actions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "abac_environments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_abac_environments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "abac_resource_types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_abac_resource_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "abac_policies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnvironmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", maxLength: 64, nullable: false),
                    Version = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    RuleJson = table.Column<string>(type: "text", nullable: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_abac_policies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_abac_policies_abac_actions_ActionId",
                        column: x => x.ActionId,
                        principalTable: "abac_actions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_abac_policies_abac_environments_EnvironmentId",
                        column: x => x.EnvironmentId,
                        principalTable: "abac_environments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_abac_policies_abac_resource_types_ResourceTypeId",
                        column: x => x.ResourceTypeId,
                        principalTable: "abac_resource_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_abac_actions_Name",
                table: "abac_actions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_abac_environments_Name",
                table: "abac_environments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_abac_policies_ActionId",
                table: "abac_policies",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_abac_policies_EnvironmentId",
                table: "abac_policies",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_abac_policies_ResourceTypeId",
                table: "abac_policies",
                column: "ResourceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_abac_policies_TenantId_ResourceTypeId_ActionId_EnvironmentI~",
                table: "abac_policies",
                columns: new[] { "TenantId", "ResourceTypeId", "ActionId", "EnvironmentId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_abac_resource_types_Name",
                table: "abac_resource_types",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "abac_policies");

            migrationBuilder.DropTable(
                name: "abac_actions");

            migrationBuilder.DropTable(
                name: "abac_environments");

            migrationBuilder.DropTable(
                name: "abac_resource_types");
        }
    }
}

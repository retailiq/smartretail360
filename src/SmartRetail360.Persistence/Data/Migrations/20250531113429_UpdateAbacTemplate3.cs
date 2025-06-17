#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAbacTemplate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_abac_resource_types_abac_resource_groups_GroupId",
                table: "abac_resource_types");

            migrationBuilder.CreateTable(
                name: "AbacResourceTypeGroupMaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbacResourceTypeGroupMaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbacResourceTypeGroupMaps_abac_resource_groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "abac_resource_groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbacResourceTypeGroupMaps_abac_resource_types_ResourceTypeId",
                        column: x => x.ResourceTypeId,
                        principalTable: "abac_resource_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbacResourceTypeGroupMaps_GroupId",
                table: "AbacResourceTypeGroupMaps",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AbacResourceTypeGroupMaps_ResourceTypeId",
                table: "AbacResourceTypeGroupMaps",
                column: "ResourceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_abac_resource_types_abac_resource_groups_GroupId",
                table: "abac_resource_types",
                column: "GroupId",
                principalTable: "abac_resource_groups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_abac_resource_types_abac_resource_groups_GroupId",
                table: "abac_resource_types");

            migrationBuilder.DropTable(
                name: "AbacResourceTypeGroupMaps");

            migrationBuilder.AddForeignKey(
                name: "FK_abac_resource_types_abac_resource_groups_GroupId",
                table: "abac_resource_types",
                column: "GroupId",
                principalTable: "abac_resource_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

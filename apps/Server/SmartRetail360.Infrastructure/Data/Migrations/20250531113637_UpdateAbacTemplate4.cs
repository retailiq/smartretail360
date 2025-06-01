using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAbacTemplate4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbacResourceTypeGroupMaps_abac_resource_groups_GroupId",
                table: "AbacResourceTypeGroupMaps");

            migrationBuilder.DropForeignKey(
                name: "FK_AbacResourceTypeGroupMaps_abac_resource_types_ResourceTypeId",
                table: "AbacResourceTypeGroupMaps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AbacResourceTypeGroupMaps",
                table: "AbacResourceTypeGroupMaps");

            migrationBuilder.RenameTable(
                name: "AbacResourceTypeGroupMaps",
                newName: "abac_resource_type_group_maps");

            migrationBuilder.RenameIndex(
                name: "IX_AbacResourceTypeGroupMaps_ResourceTypeId",
                table: "abac_resource_type_group_maps",
                newName: "IX_abac_resource_type_group_maps_ResourceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_AbacResourceTypeGroupMaps_GroupId",
                table: "abac_resource_type_group_maps",
                newName: "IX_abac_resource_type_group_maps_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_abac_resource_type_group_maps",
                table: "abac_resource_type_group_maps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_abac_resource_type_group_maps_abac_resource_groups_GroupId",
                table: "abac_resource_type_group_maps",
                column: "GroupId",
                principalTable: "abac_resource_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_abac_resource_type_group_maps_abac_resource_types_ResourceT~",
                table: "abac_resource_type_group_maps",
                column: "ResourceTypeId",
                principalTable: "abac_resource_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_abac_resource_type_group_maps_abac_resource_groups_GroupId",
                table: "abac_resource_type_group_maps");

            migrationBuilder.DropForeignKey(
                name: "FK_abac_resource_type_group_maps_abac_resource_types_ResourceT~",
                table: "abac_resource_type_group_maps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_abac_resource_type_group_maps",
                table: "abac_resource_type_group_maps");

            migrationBuilder.RenameTable(
                name: "abac_resource_type_group_maps",
                newName: "AbacResourceTypeGroupMaps");

            migrationBuilder.RenameIndex(
                name: "IX_abac_resource_type_group_maps_ResourceTypeId",
                table: "AbacResourceTypeGroupMaps",
                newName: "IX_AbacResourceTypeGroupMaps_ResourceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_abac_resource_type_group_maps_GroupId",
                table: "AbacResourceTypeGroupMaps",
                newName: "IX_AbacResourceTypeGroupMaps_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AbacResourceTypeGroupMaps",
                table: "AbacResourceTypeGroupMaps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AbacResourceTypeGroupMaps_abac_resource_groups_GroupId",
                table: "AbacResourceTypeGroupMaps",
                column: "GroupId",
                principalTable: "abac_resource_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AbacResourceTypeGroupMaps_abac_resource_types_ResourceTypeId",
                table: "AbacResourceTypeGroupMaps",
                column: "ResourceTypeId",
                principalTable: "abac_resource_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

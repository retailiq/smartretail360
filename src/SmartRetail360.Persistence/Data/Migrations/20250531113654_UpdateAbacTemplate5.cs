#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAbacTemplate5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                newName: "abac_resource_type_group_map");

            migrationBuilder.RenameIndex(
                name: "IX_abac_resource_type_group_maps_ResourceTypeId",
                table: "abac_resource_type_group_map",
                newName: "IX_abac_resource_type_group_map_ResourceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_abac_resource_type_group_maps_GroupId",
                table: "abac_resource_type_group_map",
                newName: "IX_abac_resource_type_group_map_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_abac_resource_type_group_map",
                table: "abac_resource_type_group_map",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_abac_resource_type_group_map_abac_resource_groups_GroupId",
                table: "abac_resource_type_group_map",
                column: "GroupId",
                principalTable: "abac_resource_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_abac_resource_type_group_map_abac_resource_types_ResourceTy~",
                table: "abac_resource_type_group_map",
                column: "ResourceTypeId",
                principalTable: "abac_resource_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_abac_resource_type_group_map_abac_resource_groups_GroupId",
                table: "abac_resource_type_group_map");

            migrationBuilder.DropForeignKey(
                name: "FK_abac_resource_type_group_map_abac_resource_types_ResourceTy~",
                table: "abac_resource_type_group_map");

            migrationBuilder.DropPrimaryKey(
                name: "PK_abac_resource_type_group_map",
                table: "abac_resource_type_group_map");

            migrationBuilder.RenameTable(
                name: "abac_resource_type_group_map",
                newName: "abac_resource_type_group_maps");

            migrationBuilder.RenameIndex(
                name: "IX_abac_resource_type_group_map_ResourceTypeId",
                table: "abac_resource_type_group_maps",
                newName: "IX_abac_resource_type_group_maps_ResourceTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_abac_resource_type_group_map_GroupId",
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
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SetABACControlTableNavPropDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_abac_policies_abac_actions_ActionId",
                table: "abac_policies");

            migrationBuilder.DropForeignKey(
                name: "FK_abac_policies_abac_environments_EnvironmentId",
                table: "abac_policies");

            migrationBuilder.DropForeignKey(
                name: "FK_abac_policies_abac_resource_types_ResourceTypeId",
                table: "abac_policies");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "abac_resource_types");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "abac_environments");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "abac_actions");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "abac_resource_types",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "abac_resource_types",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "abac_environments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "abac_environments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "abac_actions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "abac_actions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_abac_policies_abac_actions_ActionId",
                table: "abac_policies",
                column: "ActionId",
                principalTable: "abac_actions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_abac_policies_abac_environments_EnvironmentId",
                table: "abac_policies",
                column: "EnvironmentId",
                principalTable: "abac_environments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_abac_policies_abac_resource_types_ResourceTypeId",
                table: "abac_policies",
                column: "ResourceTypeId",
                principalTable: "abac_resource_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_abac_policies_abac_actions_ActionId",
                table: "abac_policies");

            migrationBuilder.DropForeignKey(
                name: "FK_abac_policies_abac_environments_EnvironmentId",
                table: "abac_policies");

            migrationBuilder.DropForeignKey(
                name: "FK_abac_policies_abac_resource_types_ResourceTypeId",
                table: "abac_policies");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "abac_resource_types");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "abac_resource_types");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "abac_environments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "abac_environments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "abac_actions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "abac_actions");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "abac_resource_types",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "abac_environments",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "abac_actions",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_abac_policies_abac_actions_ActionId",
                table: "abac_policies",
                column: "ActionId",
                principalTable: "abac_actions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_abac_policies_abac_environments_EnvironmentId",
                table: "abac_policies",
                column: "EnvironmentId",
                principalTable: "abac_environments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_abac_policies_abac_resource_types_ResourceTypeId",
                table: "abac_policies",
                column: "ResourceTypeId",
                principalTable: "abac_resource_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEnvAbacPoliciesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_abac_policies_abac_environments_EnvironmentId",
                table: "abac_policies");

            migrationBuilder.DropIndex(
                name: "IX_abac_policies_EnvironmentId",
                table: "abac_policies");

            migrationBuilder.DropColumn(
                name: "EnvironmentId",
                table: "abac_policies");

            migrationBuilder.AddColumn<Guid>(
                name: "AbacEnvironmentId",
                table: "abac_policies",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_abac_policies_AbacEnvironmentId",
                table: "abac_policies",
                column: "AbacEnvironmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_abac_policies_abac_environments_AbacEnvironmentId",
                table: "abac_policies",
                column: "AbacEnvironmentId",
                principalTable: "abac_environments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_abac_policies_abac_environments_AbacEnvironmentId",
                table: "abac_policies");

            migrationBuilder.DropIndex(
                name: "IX_abac_policies_AbacEnvironmentId",
                table: "abac_policies");

            migrationBuilder.DropColumn(
                name: "AbacEnvironmentId",
                table: "abac_policies");

            migrationBuilder.AddColumn<Guid>(
                name: "EnvironmentId",
                table: "abac_policies",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_abac_policies_EnvironmentId",
                table: "abac_policies",
                column: "EnvironmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_abac_policies_abac_environments_EnvironmentId",
                table: "abac_policies",
                column: "EnvironmentId",
                principalTable: "abac_environments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

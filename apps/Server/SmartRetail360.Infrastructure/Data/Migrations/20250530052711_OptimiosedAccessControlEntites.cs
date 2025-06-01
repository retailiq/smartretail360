using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class OptimiosedAccessControlEntites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "abac_resource_types",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "none",
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "abac_environments",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "none",
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "abac_actions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "none",
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "abac_resource_types",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldDefaultValue: "none");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "abac_environments",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldDefaultValue: "none");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "abac_actions",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64,
                oldDefaultValue: "none");
        }
    }
}

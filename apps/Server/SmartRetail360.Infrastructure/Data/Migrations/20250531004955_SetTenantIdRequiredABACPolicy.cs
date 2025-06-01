using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SetTenantIdRequiredABACPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "abac_policies",
                type: "uuid",
                maxLength: 64,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldMaxLength: 64,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "abac_policies",
                type: "uuid",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldMaxLength: 64);
        }
    }
}

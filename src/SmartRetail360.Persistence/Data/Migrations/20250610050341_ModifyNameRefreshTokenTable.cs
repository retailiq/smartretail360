#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifyNameRefreshTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "refresh_tokens",
                newName: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "refresh_tokens",
                newName: "Name");
        }
    }
}

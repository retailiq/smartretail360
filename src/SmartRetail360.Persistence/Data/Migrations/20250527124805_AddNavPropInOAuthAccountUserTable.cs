#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNavPropInOAuthAccountUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "oauth_accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false, defaultValue: "none"),
                    ProviderUserId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    AvatarUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    TraceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_oauth_accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_oauth_accounts_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_oauth_accounts_Email",
                table: "oauth_accounts",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_oauth_accounts_Provider",
                table: "oauth_accounts",
                column: "Provider");

            migrationBuilder.CreateIndex(
                name: "IX_oauth_accounts_ProviderUserId",
                table: "oauth_accounts",
                column: "ProviderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_oauth_accounts_TraceId",
                table: "oauth_accounts",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_oauth_accounts_UserId",
                table: "oauth_accounts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "oauth_accounts");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRetail360.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        CREATE FUNCTION prevent_role_modifications() RETURNS trigger AS $$
        BEGIN
            RAISE EXCEPTION 'System roles are read-only.';
        END;
        $$ LANGUAGE plpgsql;

        CREATE TRIGGER trg_no_role_update_delete
        BEFORE UPDATE OR DELETE ON roles
        FOR EACH ROW EXECUTE FUNCTION prevent_role_modifications();
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        DROP TRIGGER IF EXISTS trg_no_role_update_delete ON roles;
        DROP FUNCTION IF EXISTS prevent_role_modifications;
    ");
        }
    }
}

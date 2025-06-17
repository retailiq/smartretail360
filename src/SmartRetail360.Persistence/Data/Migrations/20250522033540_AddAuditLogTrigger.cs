#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartRetail360.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        CREATE FUNCTION prevent_audit_modifications() RETURNS trigger AS $$
        BEGIN
            RAISE EXCEPTION 'Audit logs are read-only.';
        END;
        $$ LANGUAGE plpgsql;

        CREATE TRIGGER trg_no_audit_update_delete
        BEFORE UPDATE OR DELETE ON audit_logs
        FOR EACH ROW EXECUTE FUNCTION prevent_audit_modifications();
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        DROP TRIGGER IF EXISTS trg_no_audit_update_delete ON audit_logs;
        DROP FUNCTION IF EXISTS prevent_audit_modifications;
    ");
        }

    }
}

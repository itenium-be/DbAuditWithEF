using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbAuditWithEF.DatabaseTrigger.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedByTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE TRIGGER trg_UpdateAuditFields
ON Products
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Products
    SET ModifiedBy = SYSTEM_USER,
        ModifiedByOverwritesAreIgnored = SYSTEM_USER,
        ModifiedOn = GETDATE()
    WHERE Id IN (SELECT Id FROM inserted);
END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop trigger trg_UpdateAuditFields");
        }
    }
}

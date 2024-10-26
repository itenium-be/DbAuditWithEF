using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbAuditWithEF.DatabaseTable.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Audit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TableIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValues = table.Column<string>(type: "xml", nullable: false),
                    NewValues = table.Column<string>(type: "xml", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValueSql: "SYSTEM_USER")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.Sql(@"
CREATE TRIGGER [dbo].[products_audit] ON [dbo].[Products]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
SET NOCOUNT ON;
IF TRIGGER_NESTLEVEL(OBJECT_ID('dbo.products_audit')) > 1
    RETURN;

IF NOT EXISTS(SELECT 1 FROM deleted) AND NOT EXISTS(SELECT 1 FROM inserted)
    RETURN;

DECLARE @ActionType char(1)
IF EXISTS (SELECT * FROM inserted)
    IF EXISTS (SELECT * FROM deleted)
        SELECT @ActionType = 'U'
    ELSE
        SELECT @ActionType = 'I'
ELSE
    SELECT @ActionType = 'D'

declare @inserted xml, @deleted xml
SET @inserted = (SELECT * FROM inserted FOR XML PATH)
SET @deleted = (SELECT * FROM deleted FOR XML PATH)

declare @tableIds varchar(MAX);
SET @tableIds = (SELECT STRING_AGG(CAST(Id as varchar(max)), ',') FROM (SELECT DISTINCT Id FROM (SELECT Id FROM inserted UNION SELECT Id from deleted) ids) distinctIds)

INSERT INTO [Audit] (TableName, TableIds, ActionType, OldValues, NewValues)
SELECT 'Products', @tableIds, @ActionType, @deleted, @inserted;


IF @ActionType = 'U'
    UPDATE dbo.[Products]
    SET ModifiedOn=CURRENT_TIMESTAMP, ModifiedBy=suser_sname()
    WHERE [Id] IN (SELECT [Id] FROM inserted);

END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER products_audit");

            migrationBuilder.DropTable(
                name: "Audit");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbAuditWithEF.DatabaseTrigger.Migrations
{
    /// <inheritdoc />
    public partial class MoreCreatedBys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByOverwritesAreIgnored",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                defaultValueSql: "SYSTEM_USER");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByOverwritesThrow",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                defaultValueSql: "SYSTEM_USER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByOverwritesAreIgnored",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedByOverwritesThrow",
                table: "Products");
        }
    }
}

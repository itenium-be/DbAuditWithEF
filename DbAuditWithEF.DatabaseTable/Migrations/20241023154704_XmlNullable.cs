using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbAuditWithEF.DatabaseTable.Migrations
{
    /// <inheritdoc />
    public partial class XmlNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OldValues",
                table: "Audit",
                type: "xml",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "xml");

            migrationBuilder.AlterColumn<string>(
                name: "NewValues",
                table: "Audit",
                type: "xml",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "xml");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OldValues",
                table: "Audit",
                type: "xml",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "xml",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewValues",
                table: "Audit",
                type: "xml",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "xml",
                oldNullable: true);
        }
    }
}

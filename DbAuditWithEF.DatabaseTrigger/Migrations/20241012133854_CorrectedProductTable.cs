using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DbAuditWithEF.DatabaseTrigger.Migrations
{
    /// <inheritdoc />
    public partial class CorrectedProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByOverwritesAreIgnored",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedByOverwritesThrow",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedByOverwritesAreIgnored",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "FaultyProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValueSql: "SYSTEM_USER"),
                    CreatedByOverwritesAreIgnored = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValueSql: "SYSTEM_USER"),
                    CreatedByOverwritesThrow = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, defaultValueSql: "SYSTEM_USER"),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaultyProducts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FaultyProducts");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedByOverwritesAreIgnored",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
    }
}

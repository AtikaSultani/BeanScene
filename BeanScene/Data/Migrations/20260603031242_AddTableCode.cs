using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeanScene.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTableCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TableNumber",
                table: "Tables");

            migrationBuilder.AddColumn<string>(
                name: "TableCode",
                table: "Tables",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TableCode",
                table: "Tables");

            migrationBuilder.AddColumn<int>(
                name: "TableNumber",
                table: "Tables",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

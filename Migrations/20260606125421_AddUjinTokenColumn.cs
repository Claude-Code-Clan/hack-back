using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XakUjin2026.Migrations
{
    /// <inheritdoc />
    public partial class AddUjinTokenColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UjinToken",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UjinToken",
                table: "Users");
        }
    }
}

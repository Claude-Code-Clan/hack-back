using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace XakUjin2026.Migrations
{
    /// <inheritdoc />
    public partial class AddRssTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RSSLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Link = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RSSLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RSSWidgets",
                columns: table => new
                {
                    IdRss = table.Column<int>(type: "integer", nullable: false),
                    IdWidget = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RSSWidgets", x => new { x.IdRss, x.IdWidget });
                    table.ForeignKey(
                        name: "FK_RSSWidgets_RSSLinks_IdRss",
                        column: x => x.IdRss,
                        principalTable: "RSSLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RSSWidgets_Widgets_IdWidget",
                        column: x => x.IdWidget,
                        principalTable: "Widgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RSSWidgets_IdWidget",
                table: "RSSWidgets",
                column: "IdWidget");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RSSWidgets");

            migrationBuilder.DropTable(
                name: "RSSLinks");
        }
    }
}

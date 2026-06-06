using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XakUjin2026.Migrations
{
    /// <inheritdoc />
    public partial class AddRssLinkTitleUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RSSLinks_Title",
                table: "RSSLinks",
                column: "Title",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RSSLinks_Title",
                table: "RSSLinks");
        }
    }
}

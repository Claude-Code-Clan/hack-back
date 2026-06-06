using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XakUjin2026.Migrations
{
    /// <inheritdoc />
    public partial class WidgetDeviceTypeIndexNonUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Widgets_DeviceId_WidgetTypeId",
                table: "Widgets");

            migrationBuilder.CreateIndex(
                name: "IX_Widgets_DeviceId_WidgetTypeId",
                table: "Widgets",
                columns: new[] { "DeviceId", "WidgetTypeId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Widgets_DeviceId_WidgetTypeId",
                table: "Widgets");

            migrationBuilder.CreateIndex(
                name: "IX_Widgets_DeviceId_WidgetTypeId",
                table: "Widgets",
                columns: new[] { "DeviceId", "WidgetTypeId" },
                unique: true);
        }
    }
}

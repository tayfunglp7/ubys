using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ubys.Migrations
{
    /// <inheritdoc />
    public partial class FakulteKisaAdEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KisaAd",
                table: "Fakulteler",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Fakulteler",
                keyColumn: "FakulteId",
                keyValue: 1L,
                column: "KisaAd",
                value: null);

            migrationBuilder.UpdateData(
                table: "Fakulteler",
                keyColumn: "FakulteId",
                keyValue: 2L,
                column: "KisaAd",
                value: null);

            migrationBuilder.UpdateData(
                table: "Fakulteler",
                keyColumn: "FakulteId",
                keyValue: 3L,
                column: "KisaAd",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KisaAd",
                table: "Fakulteler");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ubys.Migrations
{
    /// <inheritdoc />
    public partial class AdminKullanicisi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Kullanicilar",
                columns: new[] { "KullaniciId", "AdSoyad", "AktifMi", "CreatedDate", "KullaniciAdi", "SifreHash" },
                values: new object[] { 1L, "Sistem Yöneticisi", true, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "admin", "616D9E5D15FD6C71B02D68E2F42838C4BE3F7F7101533390CC66C554C18603C9" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Kullanicilar",
                keyColumn: "KullaniciId",
                keyValue: 1L);
        }
    }
}

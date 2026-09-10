using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ubys.Migrations
{
    /// <inheritdoc />
    public partial class BaslangicVerisi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Fakulteler",
                columns: new[] { "FakulteId", "AktifMi", "CreatedDate", "FakulteAd", "FakulteAdres", "FakulteEposta", "FakulteTelefon", "UpdatedDate" },
                values: new object[,]
                {
                    { 1L, true, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "Mühendislik Fakültesi", "Merkez Kampüs A Blok", "muhendislik@ornekuni.edu.tr", "02124440101", null },
                    { 2L, true, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "Fen-Edebiyat Fakültesi", "Merkez Kampüs B Blok", "fenedebiyat@ornekuni.edu.tr", "02124440102", null },
                    { 3L, true, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "İktisadi ve İdari Bilimler Fakültesi", "Güney Kampüs C Blok", "iibf@ornekuni.edu.tr", "02124440103", null }
                });

            migrationBuilder.InsertData(
                table: "Bolumler",
                columns: new[] { "BolumId", "AktifMi", "BolumAdi", "BolumAdres", "BolumEposta", "BolumTelefon", "CreatedDate", "FakulteId", "UpdatedDate" },
                values: new object[,]
                {
                    { 1L, true, "Bilgisayar Mühendisliği", "A Blok 3. Kat", "bilgisayar@ornekuni.edu.tr", "02124440201", new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), 1L, null },
                    { 2L, true, "Makine Mühendisliği", "A Blok 2. Kat", "makine@ornekuni.edu.tr", "02124440202", new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), 1L, null },
                    { 3L, true, "Matematik", "B Blok 2. Kat", "matematik@ornekuni.edu.tr", "02124440203", new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), 2L, null },
                    { 4L, true, "İşletme", "C Blok 1. Kat", "isletme@ornekuni.edu.tr", "02124440204", new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), 3L, null }
                });

            migrationBuilder.InsertData(
                table: "Akademisyenler",
                columns: new[] { "AkademisyenId", "AkademisyenAd", "AkademisyenAdres", "AkademisyenCinsiyet", "AkademisyenDogumTarihi", "AkademisyenEposta", "AkademisyenSoyad", "AkademisyenTc", "AkademisyenTelefon", "AktifMi", "BolumId", "CreatedDate", "Unvan", "UpdatedDate" },
                values: new object[,]
                {
                    { 1L, "Hakan", "Beşiktaş / İstanbul", "Erkek", new DateTime(1978, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "hakan.gunes@ornekuni.edu.tr", "Güneş", "24449676978", "05339990001", true, 1L, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "Prof. Dr.", null },
                    { 2L, "Ebru", "Sarıyer / İstanbul", "Kadın", new DateTime(1985, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "ebru.tekin@ornekuni.edu.tr", "Tekin", "42492946718", "05339990002", true, 1L, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "Doç. Dr.", null },
                    { 3L, "Cem", "Maltepe / İstanbul", "Erkek", new DateTime(1983, 5, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "cem.yavuz@ornekuni.edu.tr", "Yavuz", "75359841260", "05339990003", true, 3L, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "Dr. Öğr. Üyesi", null }
                });

            migrationBuilder.InsertData(
                table: "Ogrenciler",
                columns: new[] { "OgrenciId", "AktifMi", "BolumId", "CreatedDate", "OgrenciAd", "OgrenciAdres", "OgrenciCinsiyet", "OgrenciDogumTarihi", "OgrenciEposta", "OgrenciSinif", "OgrenciSoyad", "OgrenciTc", "OgrenciTelefon", "UpdatedDate" },
                values: new object[,]
                {
                    { 1L, true, 1L, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "Ayşe", "Bağcılar / İstanbul", "Kadın", new DateTime(2003, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "ayse.yilmaz@ogr.ornekuni.edu.tr", 3, "Yılmaz", "42950115298", "05321110001", null },
                    { 2L, true, 1L, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "Mehmet", "Kadıköy / İstanbul", "Erkek", new DateTime(2004, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "mehmet.demir@ogr.ornekuni.edu.tr", 2, "Demir", "21199630090", "05321110002", null },
                    { 3L, true, 2L, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "Zeynep", "Üsküdar / İstanbul", "Kadın", new DateTime(2002, 1, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "zeynep.kaya@ogr.ornekuni.edu.tr", 4, "Kaya", "26706917794", "05321110003", null },
                    { 4L, true, 3L, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "Emre", "Beylikdüzü / İstanbul", "Erkek", new DateTime(2005, 6, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "emre.sahin@ogr.ornekuni.edu.tr", 1, "Şahin", "39294742426", "05321110004", null },
                    { 5L, true, 4L, new DateTime(2026, 1, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "Gülşah", "Ataşehir / İstanbul", "Kadın", new DateTime(2003, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "gulsah.ozturk@ogr.ornekuni.edu.tr", 3, "Öztürk", "30202008634", "05321110005", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Akademisyenler",
                keyColumn: "AkademisyenId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Akademisyenler",
                keyColumn: "AkademisyenId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Akademisyenler",
                keyColumn: "AkademisyenId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Ogrenciler",
                keyColumn: "OgrenciId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Ogrenciler",
                keyColumn: "OgrenciId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Ogrenciler",
                keyColumn: "OgrenciId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Ogrenciler",
                keyColumn: "OgrenciId",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Ogrenciler",
                keyColumn: "OgrenciId",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Bolumler",
                keyColumn: "BolumId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Bolumler",
                keyColumn: "BolumId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Bolumler",
                keyColumn: "BolumId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Bolumler",
                keyColumn: "BolumId",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Fakulteler",
                keyColumn: "FakulteId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Fakulteler",
                keyColumn: "FakulteId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Fakulteler",
                keyColumn: "FakulteId",
                keyValue: 3L);
        }
    }
}

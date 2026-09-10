using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ubys.Migrations
{
    /// <inheritdoc />
    public partial class IlkOlusturma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fakulteler",
                columns: table => new
                {
                    FakulteId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FakulteAd = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FakulteAdres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FakulteTelefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FakulteEposta = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fakulteler", x => x.FakulteId);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    KullaniciId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SifreHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AdSoyad = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.KullaniciId);
                });

            migrationBuilder.CreateTable(
                name: "Bolumler",
                columns: table => new
                {
                    BolumId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FakulteId = table.Column<long>(type: "bigint", nullable: false),
                    BolumAdi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    BolumAdres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BolumTelefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BolumEposta = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bolumler", x => x.BolumId);
                    table.ForeignKey(
                        name: "FK_Bolumler_Fakulteler_FakulteId",
                        column: x => x.FakulteId,
                        principalTable: "Fakulteler",
                        principalColumn: "FakulteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Akademisyenler",
                columns: table => new
                {
                    AkademisyenId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BolumId = table.Column<long>(type: "bigint", nullable: false),
                    AkademisyenAd = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AkademisyenSoyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unvan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AkademisyenDogumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AkademisyenCinsiyet = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AkademisyenAdres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AkademisyenTelefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AkademisyenEposta = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AkademisyenTc = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Akademisyenler", x => x.AkademisyenId);
                    table.ForeignKey(
                        name: "FK_Akademisyenler_Bolumler_BolumId",
                        column: x => x.BolumId,
                        principalTable: "Bolumler",
                        principalColumn: "BolumId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ogrenciler",
                columns: table => new
                {
                    OgrenciId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BolumId = table.Column<long>(type: "bigint", nullable: false),
                    OgrenciAd = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OgrenciSoyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OgrenciSinif = table.Column<int>(type: "int", nullable: false),
                    OgrenciDogumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OgrenciCinsiyet = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    OgrenciAdres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OgrenciTelefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OgrenciEposta = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OgrenciTc = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ogrenciler", x => x.OgrenciId);
                    table.ForeignKey(
                        name: "FK_Ogrenciler_Bolumler_BolumId",
                        column: x => x.BolumId,
                        principalTable: "Bolumler",
                        principalColumn: "BolumId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Akademisyenler_AkademisyenEposta",
                table: "Akademisyenler",
                column: "AkademisyenEposta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Akademisyenler_AkademisyenTc",
                table: "Akademisyenler",
                column: "AkademisyenTc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Akademisyenler_BolumId",
                table: "Akademisyenler",
                column: "BolumId");

            migrationBuilder.CreateIndex(
                name: "IX_Bolumler_BolumEposta",
                table: "Bolumler",
                column: "BolumEposta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bolumler_FakulteId",
                table: "Bolumler",
                column: "FakulteId");

            migrationBuilder.CreateIndex(
                name: "IX_Fakulteler_FakulteEposta",
                table: "Fakulteler",
                column: "FakulteEposta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_KullaniciAdi",
                table: "Kullanicilar",
                column: "KullaniciAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ogrenciler_BolumId",
                table: "Ogrenciler",
                column: "BolumId");

            migrationBuilder.CreateIndex(
                name: "IX_Ogrenciler_OgrenciEposta",
                table: "Ogrenciler",
                column: "OgrenciEposta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ogrenciler_OgrenciTc",
                table: "Ogrenciler",
                column: "OgrenciTc",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Akademisyenler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Ogrenciler");

            migrationBuilder.DropTable(
                name: "Bolumler");

            migrationBuilder.DropTable(
                name: "Fakulteler");
        }
    }
}

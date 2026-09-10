using Microsoft.EntityFrameworkCore;
using UBYS.Models;

namespace UBYS.Data;

/// <summary>
/// Uygulamanın veritabanı oturumu.
///
/// ADO.NET sürümündeki DÖRT repository sınıfının yerini
/// bu TEK sınıf aldı.
/// </summary>
public class UbysDbContext : DbContext
{
    // ══════════════════════════════════════════════════════
    //  YAPICI METOT
    //
    //  Bağlantı ayarlarını dışarıdan (Program.cs'ten) alıyoruz.
    //  Bu sayede sınıf, bağlantı dizesini kendisi bilmek zorunda kalmıyor.
    //  ADO.NET'te her repository IConfiguration alıyordu — aynı fikir.
    // ══════════════════════════════════════════════════════
    public UbysDbContext(DbContextOptions<UbysDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Öznitelikle anlatılamayan model ayarları burada yapılır.
    /// EF, migration üretirken bu metodu çalıştırır.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── BENZERSİZLİK İNDEKSLERİ ──────────────────────────
        // ADO.NET sürümünde bunları elle CREATE UNIQUE INDEX ile yazmıştık.
        // Şimdi burada tanımlıyoruz, migration onları üretecek.

        modelBuilder.Entity<Fakulte>()
            .HasIndex(f => f.FakulteEposta)
            .IsUnique();

        modelBuilder.Entity<Bolum>()
            .HasIndex(b => b.BolumEposta)
            .IsUnique();

        modelBuilder.Entity<Ogrenci>()
            .HasIndex(o => o.OgrenciEposta)
            .IsUnique();

        modelBuilder.Entity<Ogrenci>()
            .HasIndex(o => o.OgrenciTc)
            .IsUnique();

        modelBuilder.Entity<Akademisyen>()
            .HasIndex(a => a.AkademisyenEposta)
            .IsUnique();

        modelBuilder.Entity<Akademisyen>()
            .HasIndex(a => a.AkademisyenTc)
            .IsUnique();

        modelBuilder.Entity<Kullanici>()
            .HasIndex(k => k.KullaniciAdi)
            .IsUnique();

        // ── SİLME DAVRANIŞI ──────────────────────────────────
        // Varsayılan: Cascade (fakülte silinirse bölümleri de silinir)
        // Biz bunu İSTEMİYORUZ — zaten soft delete kullanacağız.
        // Restrict: bağlı kaydı olan satır silinemez.

        modelBuilder.Entity<Bolum>()
            .HasOne(b => b.Fakulte)
            .WithMany(f => f.Bolumler)
            .HasForeignKey(b => b.FakulteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ogrenci>()
            .HasOne(o => o.Bolum)
            .WithMany(b => b.Ogrenciler)
            .HasForeignKey(o => o.BolumId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Akademisyen>()
            .HasOne(a => a.Bolum)
            .WithMany(b => b.Akademisyenler)
            .HasForeignKey(a => a.BolumId)
            .OnDelete(DeleteBehavior.Restrict);

        // ══════════════════════════════════════════════════════
        //  BAŞLANGIÇ VERİSİ (SEED DATA)
        //
        //  ⚠️ HasData ile verilen kayıtların ID'leri ELLE yazılır.
        //     EF bunları migration dosyasına INSERT olarak koyar.
        //
        //  ⚠️ DateTime.Now KULLANILAMAZ!
        //     Seed verisi migration'a sabit olarak yazılır; her
        //     "migrations add" komutunda tarih değişirse EF
        //     "model değişti" sanar ve boş migration üretir.
        //     Bu yüzden SABİT tarih kullanıyoruz.
        // ══════════════════════════════════════════════════════
        var sabitTarih = new DateTime(2026, 1, 1, 9, 0, 0);

        modelBuilder.Entity<Fakulte>().HasData(
            new Fakulte
            {
                FakulteId = 1,
                FakulteAd = "Mühendislik Fakültesi",
                FakulteAdres = "Merkez Kampüs A Blok",
                FakulteTelefon = "02124440101",
                FakulteEposta = "muhendislik@ornekuni.edu.tr",
                CreatedDate = sabitTarih,
                AktifMi = true
            },
            new Fakulte
            {
                FakulteId = 2,
                FakulteAd = "Fen-Edebiyat Fakültesi",
                FakulteAdres = "Merkez Kampüs B Blok",
                FakulteTelefon = "02124440102",
                FakulteEposta = "fenedebiyat@ornekuni.edu.tr",
                CreatedDate = sabitTarih,
                AktifMi = true
            },
            new Fakulte
            {
                FakulteId = 3,
                FakulteAd = "İktisadi ve İdari Bilimler Fakültesi",
                FakulteAdres = "Güney Kampüs C Blok",
                FakulteTelefon = "02124440103",
                FakulteEposta = "iibf@ornekuni.edu.tr",
                CreatedDate = sabitTarih,
                AktifMi = true
            }
        );

        modelBuilder.Entity<Bolum>().HasData(
            new Bolum
            {
                BolumId = 1,
                FakulteId = 1,
                BolumAdi = "Bilgisayar Mühendisliği",
                BolumAdres = "A Blok 3. Kat",
                BolumTelefon = "02124440201",
                BolumEposta = "bilgisayar@ornekuni.edu.tr",
                CreatedDate = sabitTarih,
                AktifMi = true
            },
            new Bolum
            {
                BolumId = 2,
                FakulteId = 1,
                BolumAdi = "Makine Mühendisliği",
                BolumAdres = "A Blok 2. Kat",
                BolumTelefon = "02124440202",
                BolumEposta = "makine@ornekuni.edu.tr",
                CreatedDate = sabitTarih,
                AktifMi = true
            },
            new Bolum
            {
                BolumId = 3,
                FakulteId = 2,
                BolumAdi = "Matematik",
                BolumAdres = "B Blok 2. Kat",
                BolumTelefon = "02124440203",
                BolumEposta = "matematik@ornekuni.edu.tr",
                CreatedDate = sabitTarih,
                AktifMi = true
            },
            new Bolum
            {
                BolumId = 4,
                FakulteId = 3,
                BolumAdi = "İşletme",
                BolumAdres = "C Blok 1. Kat",
                BolumTelefon = "02124440204",
                BolumEposta = "isletme@ornekuni.edu.tr",
                CreatedDate = sabitTarih,
                AktifMi = true
            }
        );

        modelBuilder.Entity<Ogrenci>().HasData(
            new Ogrenci
            {
                OgrenciId = 1,
                BolumId = 1,
                OgrenciAd = "Ayşe",
                OgrenciSoyad = "Yılmaz",
                OgrenciSinif = 3,
                OgrenciDogumTarihi = new DateTime(2003, 4, 12),
                OgrenciCinsiyet = "Kadın",
                OgrenciAdres = "Bağcılar / İstanbul",
                OgrenciTelefon = "05321110001",
                OgrenciEposta = "ayse.yilmaz@ogr.ornekuni.edu.tr",
                OgrenciTc = "42950115298",
                CreatedDate = sabitTarih,
                AktifMi = true
            },
            new Ogrenci
            {
                OgrenciId = 2,
                BolumId = 1,
                OgrenciAd = "Mehmet",
                OgrenciSoyad = "Demir",
                OgrenciSinif = 2,
                OgrenciDogumTarihi = new DateTime(2004, 9, 25),
                OgrenciCinsiyet = "Erkek",
                OgrenciAdres = "Kadıköy / İstanbul",
                OgrenciTelefon = "05321110002",
                OgrenciEposta = "mehmet.demir@ogr.ornekuni.edu.tr",
                OgrenciTc = "21199630090",
                CreatedDate = sabitTarih,
                AktifMi = true
            },
            new Ogrenci
            {
                OgrenciId = 3,
                BolumId = 2,
                OgrenciAd = "Zeynep",
                OgrenciSoyad = "Kaya",
                OgrenciSinif = 4,
                OgrenciDogumTarihi = new DateTime(2002, 1, 30),
                OgrenciCinsiyet = "Kadın",
                OgrenciAdres = "Üsküdar / İstanbul",
                OgrenciTelefon = "05321110003",
                OgrenciEposta = "zeynep.kaya@ogr.ornekuni.edu.tr",
                OgrenciTc = "26706917794",
                CreatedDate = sabitTarih,
                AktifMi = true
            },
            new Ogrenci
            {
                OgrenciId = 4,
                BolumId = 3,
                OgrenciAd = "Emre",
                OgrenciSoyad = "Şahin",
                OgrenciSinif = 1,
                OgrenciDogumTarihi = new DateTime(2005, 6, 8),
                OgrenciCinsiyet = "Erkek",
                OgrenciAdres = "Beylikdüzü / İstanbul",
                OgrenciTelefon = "05321110004",
                OgrenciEposta = "emre.sahin@ogr.ornekuni.edu.tr",
                OgrenciTc = "39294742426",
                CreatedDate = sabitTarih,
                AktifMi = true
            },
            new Ogrenci
            {
                OgrenciId = 5,
                BolumId = 4,
                OgrenciAd = "Gülşah",
                OgrenciSoyad = "Öztürk",
                OgrenciSinif = 3,
                OgrenciDogumTarihi = new DateTime(2003, 11, 17),
                OgrenciCinsiyet = "Kadın",
                OgrenciAdres = "Ataşehir / İstanbul",
                OgrenciTelefon = "05321110005",
                OgrenciEposta = "gulsah.ozturk@ogr.ornekuni.edu.tr",
                OgrenciTc = "30202008634",
                CreatedDate = sabitTarih,
                AktifMi = true
            }
        );

        modelBuilder.Entity<Akademisyen>().HasData(
            new Akademisyen
            {
                AkademisyenId = 1,
                BolumId = 1,
                AkademisyenAd = "Hakan",
                AkademisyenSoyad = "Güneş",
                Unvan = "Prof. Dr.",
                AkademisyenDogumTarihi = new DateTime(1978, 3, 15),
                AkademisyenCinsiyet = "Erkek",
                AkademisyenAdres = "Beşiktaş / İstanbul",
                AkademisyenTelefon = "05339990001",
                AkademisyenEposta = "hakan.gunes@ornekuni.edu.tr",
                AkademisyenTc = "24449676978",
                CreatedDate = sabitTarih,
                AktifMi = true
            },
            new Akademisyen
            {
                AkademisyenId = 2,
                BolumId = 1,
                AkademisyenAd = "Ebru",
                AkademisyenSoyad = "Tekin",
                Unvan = "Doç. Dr.",
                AkademisyenDogumTarihi = new DateTime(1985, 11, 2),
                AkademisyenCinsiyet = "Kadın",
                AkademisyenAdres = "Sarıyer / İstanbul",
                AkademisyenTelefon = "05339990002",
                AkademisyenEposta = "ebru.tekin@ornekuni.edu.tr",
                AkademisyenTc = "42492946718",
                CreatedDate = sabitTarih,
                AktifMi = true
            },
            new Akademisyen
            {
                AkademisyenId = 3,
                BolumId = 3,
                AkademisyenAd = "Cem",
                AkademisyenSoyad = "Yavuz",
                Unvan = "Dr. Öğr. Üyesi",
                AkademisyenDogumTarihi = new DateTime(1983, 5, 24),
                AkademisyenCinsiyet = "Erkek",
                AkademisyenAdres = "Maltepe / İstanbul",
                AkademisyenTelefon = "05339990003",
                AkademisyenEposta = "cem.yavuz@ornekuni.edu.tr",
                AkademisyenTc = "75359841260",
                CreatedDate = sabitTarih,
                AktifMi = true
            }
        );
    }

    // ══════════════════════════════════════════════════════
    //  TABLOLAR
    //
    //  Her DbSet bir tabloya karşılık gelir.
    //  Özellik adı (Fakulteler) → tablo adı olur.
    //
    //  ⚠️ Model sınıflarını henüz yazmadık, bu satırlar şu an
    //     hata verecek. Modül 1'de yazacağız.
    // ══════════════════════════════════════════════════════
    public DbSet<Fakulte> Fakulteler { get; set; }
    public DbSet<Bolum> Bolumler { get; set; }
    public DbSet<Ogrenci> Ogrenciler { get; set; }
    public DbSet<Akademisyen> Akademisyenler { get; set; }
    public DbSet<Kullanici> Kullanicilar { get; set; }
}
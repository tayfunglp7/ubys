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
using System.ComponentModel.DataAnnotations;

namespace UBYS.Models;

public class Bolum
{
    public long BolumId { get; set; }

    // ⭐ YABANCI ANAHTAR
    // "Fakulte" navigasyon özelliği + "FakulteId" alanı bir arada
    // olduğunda EF bunun bir ilişki olduğunu KENDİLİĞİNDEN anlar.
    // Ayrıca [ForeignKey] yazmaya gerek yok.
    [Required(ErrorMessage = "Fakülte seçmelisiniz.")]
    [Display(Name = "Bağlı olduğu fakülte")]
    public long FakulteId { get; set; }

    [Required(ErrorMessage = "Bölüm adı zorunludur.")]
    [StringLength(255)]
    [Display(Name = "Bölüm adı")]
    public string BolumAdi { get; set; } = "";

    [Required(ErrorMessage = "Adres zorunludur.")]
    [StringLength(500)]
    [Display(Name = "Adres")]
    public string BolumAdres { get; set; } = "";

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [StringLength(20)]
    [Display(Name = "Telefon")]
    public string BolumTelefon { get; set; } = "";

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [StringLength(255)]
    [Display(Name = "E-posta")]
    public string BolumEposta { get; set; } = "";

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? UpdatedDate { get; set; }
    public bool AktifMi { get; set; } = true;

    // ── Navigasyon özellikleri ───────────────────────────────

    // "Bir" tarafı: bu bölüm HANGİ fakülteye ait?
    //
    // ⚠️ Sondaki ? ÖNEMLİ:
    //    Form gönderildiğinde bu nesne dolu gelmez (sadece FakulteId gelir).
    //    ? koymazsak [Required] gibi davranır ve ModelState geçersiz olur.
    //    Bu, EF'te en sık yaşanan hatalardan biridir — Modül 5'te tekrar değineceğiz.
    [Display(Name = "Fakülte")]
    public Fakulte? Fakulte { get; set; }

    // "Çok" tarafı: bu bölümdeki öğrenciler ve akademisyenler
    public List<Ogrenci> Ogrenciler { get; set; } = new();
    public List<Akademisyen> Akademisyenler { get; set; } = new();
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UBYS.Models;

public class Fakulte
{
    // ⭐ ANAHTAR KURALI:
    // EF, "Id" veya "<SınıfAdı>Id" adlı özelliği otomatik olarak
    // birincil anahtar kabul eder. Burada "FakulteId" → PK.
    // Ayrıca [Key] yazmaya gerek yok.
    //
    // long tipi olduğu için SQL'de BIGINT olur ve
    // otomatik artan (IDENTITY) olarak ayarlanır.
    public long FakulteId { get; set; }

    [Required(ErrorMessage = "Fakülte adı zorunludur.")]
    [StringLength(255, ErrorMessage = "En fazla 255 karakter olabilir.")]
    [Display(Name = "Fakülte adı")]
    public string FakulteAd { get; set; } = "";

    [Required(ErrorMessage = "Adres zorunludur.")]
    [StringLength(500)]
    [Display(Name = "Adres")]
    public string FakulteAdres { get; set; } = "";

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [StringLength(20)]
    [Display(Name = "Telefon")]
    public string FakulteTelefon { get; set; } = "";

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [StringLength(255)]
    [Display(Name = "E-posta")]
    public string FakulteEposta { get; set; } = "";

    // ── Denetim alanları ─────────────────────────────────────
    [Display(Name = "Kayıt tarihi")]
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [Display(Name = "Güncelleme tarihi")]
    public DateTime? UpdatedDate { get; set; }

    // ⭐ ADO.NET sürümünde bu alan NVARCHAR(255) idi ve '1'/'0' yazıyorduk.
    //    Orada "yanlış tasarım" demiştik. Code First sayesinde
    //    artık doğrusunu yapabiliyoruz: bool → SQL'de BIT olur.
    public bool AktifMi { get; set; } = true;

    [StringLength(20)]
    [Display(Name = "Kısa ad")]
    public string? KisaAd { get; set; }

    // ── Navigasyon özelliği ──────────────────────────────────
    // Bu fakülteye bağlı bölümler.
    // ⚠️ Veritabanında böyle bir SÜTUN YOK. EF bunu ilişkiden anlar.
    //
    // [NotMapped] yazmaya gerek yok — EF, koleksiyon tipindeki
    // navigasyon özelliklerini sütun sanmaz.
    public List<Bolum> Bolumler { get; set; } = new();
}
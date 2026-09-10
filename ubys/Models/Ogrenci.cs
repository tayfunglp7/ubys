using System.ComponentModel.DataAnnotations;

namespace UBYS.Models;

public class Ogrenci
{
    public long OgrenciId { get; set; }

    [Required(ErrorMessage = "Bölüm seçmelisiniz.")]
    [Display(Name = "Bölüm")]
    public long BolumId { get; set; }

    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(100)]
    [Display(Name = "Ad")]
    public string OgrenciAd { get; set; } = "";

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(100)]
    [Display(Name = "Soyad")]
    public string OgrenciSoyad { get; set; } = "";

    [Required(ErrorMessage = "Sınıf zorunludur.")]
    [Range(1, 6, ErrorMessage = "Sınıf 1 ile 6 arasında olmalıdır.")]
    [Display(Name = "Sınıf")]
    public int OgrenciSinif { get; set; }

    [Required(ErrorMessage = "Doğum tarihi zorunludur.")]
    [DataType(DataType.Date)]
    [Display(Name = "Doğum tarihi")]
    public DateTime OgrenciDogumTarihi { get; set; }

    [Required(ErrorMessage = "Cinsiyet seçmelisiniz.")]
    [StringLength(10)]
    [Display(Name = "Cinsiyet")]
    public string OgrenciCinsiyet { get; set; } = "";

    [Required(ErrorMessage = "Adres zorunludur.")]
    [StringLength(500)]
    [Display(Name = "Adres")]
    public string OgrenciAdres { get; set; } = "";

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [StringLength(20)]
    [Display(Name = "Telefon")]
    public string OgrenciTelefon { get; set; } = "";

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [StringLength(255)]
    [Display(Name = "E-posta")]
    public string OgrenciEposta { get; set; } = "";

    [Required(ErrorMessage = "TC kimlik numarası zorunludur.")]
    [RegularExpression(@"^[1-9][0-9]{10}$",
        ErrorMessage = "TC kimlik numarası 11 haneli olmalı ve 0 ile başlamamalıdır.")]
    [StringLength(11)]
    [Display(Name = "TC kimlik no")]
    public string OgrenciTc { get; set; } = "";

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? UpdatedDate { get; set; }
    public bool AktifMi { get; set; } = true;

    // ── Navigasyon ───────────────────────────────────────────
    [Display(Name = "Bölüm")]
    public Bolum? Bolum { get; set; }

    // ── Hesaplanan özellik ───────────────────────────────────
    // ⚠️ [NotMapped] ŞART!
    //    Olmazsa EF bunu bir sütun sanır ve tabloya "TamAd" ekler.
    //    Sadece "get" olan özellikleri EF zaten atlar ama
    //    niyeti açıkça belirtmek daha güvenlidir.
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string TamAd => OgrenciAd + " " + OgrenciSoyad;
}
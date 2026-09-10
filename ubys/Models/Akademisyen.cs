using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UBYS.Models;

public class Akademisyen
{
    public long AkademisyenId { get; set; }

    [Required(ErrorMessage = "Bölüm seçmelisiniz.")]
    [Display(Name = "Bölüm")]
    public long BolumId { get; set; }

    [Required(ErrorMessage = "Ad zorunludur.")]
    [StringLength(100)]
    [Display(Name = "Ad")]
    public string AkademisyenAd { get; set; } = "";

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [StringLength(100)]
    [Display(Name = "Soyad")]
    public string AkademisyenSoyad { get; set; } = "";

    [StringLength(50)]
    [Display(Name = "Unvan")]
    public string? Unvan { get; set; }          // isteğe bağlı → NULL olabilir

    [Required(ErrorMessage = "Doğum tarihi zorunludur.")]
    [DataType(DataType.Date)]
    [Display(Name = "Doğum tarihi")]
    public DateTime AkademisyenDogumTarihi { get; set; }

    [Required(ErrorMessage = "Cinsiyet seçmelisiniz.")]
    [StringLength(10)]
    [Display(Name = "Cinsiyet")]
    public string AkademisyenCinsiyet { get; set; } = "";

    [Required(ErrorMessage = "Adres zorunludur.")]
    [StringLength(500)]
    [Display(Name = "Adres")]
    public string AkademisyenAdres { get; set; } = "";

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
    [StringLength(20)]
    [Display(Name = "Telefon")]
    public string AkademisyenTelefon { get; set; } = "";

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [StringLength(255)]
    [Display(Name = "E-posta")]
    public string AkademisyenEposta { get; set; } = "";

    [Required(ErrorMessage = "TC kimlik numarası zorunludur.")]
    [RegularExpression(@"^[1-9][0-9]{10}$",
        ErrorMessage = "TC kimlik numarası 11 haneli olmalı ve 0 ile başlamamalıdır.")]
    [StringLength(11)]
    [Display(Name = "TC kimlik no")]
    public string AkademisyenTc { get; set; } = "";

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? UpdatedDate { get; set; }
    public bool AktifMi { get; set; } = true;

    [Display(Name = "Bölüm")]
    public Bolum? Bolum { get; set; }

    [NotMapped]
    public string TamAd => (Unvan == null ? "" : Unvan + " ") + AkademisyenAd + " " + AkademisyenSoyad;
}
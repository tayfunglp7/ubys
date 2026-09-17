namespace UBYS.Models;

/// <summary>
/// Dashboard ekranının taşıyıcısı.
///
/// ⭐ Model ile ViewModel farkı:
///    Model     = bir TABLONUN karşılığı  (Fakulte, Ogrenci)
///    ViewModel = bir EKRANIN ihtiyacı    (bu sınıf)
///
/// Bu sınıfın DbSet'i YOK, tablosu YOK. Sadece veri taşır.
/// </summary>
public class DashboardViewModel
{
    // Üst kartlar
    public int FakulteSayisi { get; set; }
    public int BolumSayisi { get; set; }
    public int OgrenciSayisi { get; set; }
    public int AkademisyenSayisi { get; set; }

    // = new()  →  boş başlasın, null olmasın (view'da çökmesin)
    public List<BolumDagilim> BolumDagilimlari { get; set; } = new();
    public List<SinifDagilim> SinifDagilimlari { get; set; } = new();
    public List<CinsiyetDagilim> CinsiyetDagilimlari { get; set; } = new();
    public List<SonOgrenci> SonEklenenOgrenciler { get; set; } = new();
}

public class BolumDagilim
{
    public string BolumAdi { get; set; } = "";
    public string FakulteAd { get; set; } = "";
    public int OgrenciSayisi { get; set; }
    public int AkademisyenSayisi { get; set; }
}

public class SinifDagilim
{
    public int Sinif { get; set; }
    public int OgrenciSayisi { get; set; }
}

public class CinsiyetDagilim
{
    public string Cinsiyet { get; set; } = "";
    public int OgrenciSayisi { get; set; }
}

public class SonOgrenci
{
    public long OgrenciId { get; set; }
    public string TamAd { get; set; } = "";
    public string BolumAdi { get; set; } = "";
    public int Sinif { get; set; }
    public DateTime CreatedDate { get; set; }
}
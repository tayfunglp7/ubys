using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UBYS.Data;
using UBYS.Models;

namespace UBYS.Controllers;

public class AkademisyenController : Controller
{
    private readonly UbysDbContext _db;

    public AkademisyenController(UbysDbContext db)
    {
        _db = db;
    }

    // ══════════════════════════════════════════════════════
    //  YARDIMCI: bölüm açılır listesi
    //  (OgrenciController'daki ile birebir aynı)
    // ══════════════════════════════════════════════════════
    private async Task BolumListesiniHazirlaAsync(long? secili = null)
    {
        var bolumler = await _db.Bolumler
            .Include(b => b.Fakulte)
            .OrderBy(b => b.Fakulte!.FakulteAd)
            .ThenBy(b => b.BolumAdi)
            .Select(b => new
            {
                b.BolumId,
                Ad = b.BolumAdi + " (" + b.Fakulte!.FakulteAd + ")"
            })
            .ToListAsync();

        ViewBag.Bolumler = new SelectList(bolumler, "BolumId", "Ad", secili);
    }

    // ══════════════════════════════════════════════════════
    //  YARDIMCI: benzersizlik ön kontrolü
    // ══════════════════════════════════════════════════════
    private async Task BenzersizlikKontrolAsync(Akademisyen akademisyen, long guncellenenId = 0)
    {
        bool epostaVar = await _db.Akademisyenler
            .AnyAsync(a => a.AkademisyenEposta == akademisyen.AkademisyenEposta
                        && a.AkademisyenId != guncellenenId);

        if (epostaVar)
            ModelState.AddModelError(nameof(Akademisyen.AkademisyenEposta),
                "Bu e-posta adresi başka bir akademisyene kayıtlı.");

        bool tcVar = await _db.Akademisyenler
            .AnyAsync(a => a.AkademisyenTc == akademisyen.AkademisyenTc
                        && a.AkademisyenId != guncellenenId);

        if (tcVar)
            ModelState.AddModelError(nameof(Akademisyen.AkademisyenTc),
                "Bu TC kimlik numarası başka bir akademisyene kayıtlı.");
    }

    // ══════════════════════════════════════════════════════
    //  YARDIMCI: veritabanı hatasını kullanıcı diline çevir
    // ══════════════════════════════════════════════════════
    private void VeritabaniHatasiniIsle(DbUpdateException ex)
    {
        if (ex.InnerException is SqlException sqlEx &&
            (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        {
            string mesaj = sqlEx.Message;

            if (mesaj.Contains("Eposta"))
                ModelState.AddModelError(nameof(Akademisyen.AkademisyenEposta),
                    "Bu e-posta adresi başka bir akademisyene kayıtlı.");
            else if (mesaj.Contains("Tc"))
                ModelState.AddModelError(nameof(Akademisyen.AkademisyenTc),
                    "Bu TC kimlik numarası başka bir akademisyene kayıtlı.");
            else
                ModelState.AddModelError("", "Bu kayıt zaten mevcut.");
        }
        else
        {
            ModelState.AddModelError("",
                "Kayıt sırasında bir sorun oluştu. Lütfen tekrar deneyin.");
        }
    }

    // ══════════════════════════════════════════════════════
    //  1) LİSTELEME
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Index(
        string? arama,
        long? bolumId,
        int? unvan,
        int sayfa = 1)
    {
        const int sayfaBoyutu = 10;

        if (sayfa < 1) sayfa = 1;

        IQueryable<Akademisyen> sorgu = _db.Akademisyenler
            .Include(a => a.Bolum)
                .ThenInclude(b => b!.Fakulte);

        if (!string.IsNullOrWhiteSpace(arama))
        {
            string temizArama = arama.Trim();

            sorgu = sorgu.Where(a =>
                a.AkademisyenAd.Contains(temizArama) ||
                a.AkademisyenSoyad.Contains(temizArama) ||
                a.AkademisyenEposta.Contains(temizArama));
        }

        if (bolumId.HasValue && bolumId.Value > 0)
            sorgu = sorgu.Where(a => a.BolumId == bolumId.Value);

        if (unvan.HasValue && unvan.Value > 0)
            sorgu = sorgu.Where(a => a.Unvan.ToString() == unvan.Value.ToString());

        int toplamKayit = await sorgu.CountAsync();

        var liste = await sorgu
            .OrderBy(a => a.AkademisyenAd)
            .ThenBy(a => a.AkademisyenSoyad)
            .Skip((sayfa - 1) * sayfaBoyutu)
            .Take(sayfaBoyutu)
            .ToListAsync();

        ViewBag.Sayfa       = sayfa;
        ViewBag.ToplamSayfa = (int)Math.Ceiling((double)toplamKayit / sayfaBoyutu);
        ViewBag.ToplamKayit = toplamKayit;

        ViewBag.Arama         = arama;
        ViewBag.SeciliBolum   = bolumId;
        ViewBag.SeciliUnvan   = unvan;

        await BolumListesiniHazirlaAsync(bolumId);

        return View(liste);

    }
        
    // ══════════════════════════════════════════════════════
    //  2) YENİ KAYIT FORMU
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Create()
    {
        await BolumListesiniHazirlaAsync();
        return View();
    }

    // ══════════════════════════════════════════════════════
    //  3) YENİ KAYDI KAYDET
    // ══════════════════════════════════════════════════════
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Akademisyen akademisyen)
    {
        await BenzersizlikKontrolAsync(akademisyen);

        if (!ModelState.IsValid)
        {
            await BolumListesiniHazirlaAsync(akademisyen.BolumId);
            return View(akademisyen);
        }

        akademisyen.CreatedDate = DateTime.Now;
        akademisyen.UpdatedDate = null;
        akademisyen.AktifMi = true;

        try
        {
            _db.Akademisyenler.Add(akademisyen);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            VeritabaniHatasiniIsle(ex);
            await BolumListesiniHazirlaAsync(akademisyen.BolumId);
            return View(akademisyen);
        }

        TempData["Basarili"] = $"{akademisyen.TamAd} kaydedildi.";
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════════════════
    //  4) DÜZENLEME FORMU
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Edit(long id)
    {
        var akademisyen = await _db.Akademisyenler.FindAsync(id);

        if (akademisyen == null)
            return NotFound();

        await BolumListesiniHazirlaAsync(akademisyen.BolumId);
        return View(akademisyen);
    }

    // ══════════════════════════════════════════════════════
    //  5) DÜZENLEMEYİ KAYDET
    // ══════════════════════════════════════════════════════
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Akademisyen akademisyen)
    {
        await BenzersizlikKontrolAsync(akademisyen, akademisyen.AkademisyenId);

        if (!ModelState.IsValid)
        {
            await BolumListesiniHazirlaAsync(akademisyen.BolumId);
            return View(akademisyen);
        }

        var mevcut = await _db.Akademisyenler.FindAsync(akademisyen.AkademisyenId);

        if (mevcut == null)
            return NotFound();

        mevcut.BolumId = akademisyen.BolumId;
        mevcut.AkademisyenAd = akademisyen.AkademisyenAd;
        mevcut.AkademisyenSoyad = akademisyen.AkademisyenSoyad;
        mevcut.Unvan = akademisyen.Unvan;
        mevcut.AkademisyenDogumTarihi = akademisyen.AkademisyenDogumTarihi;
        mevcut.AkademisyenCinsiyet = akademisyen.AkademisyenCinsiyet;
        mevcut.AkademisyenAdres = akademisyen.AkademisyenAdres;
        mevcut.AkademisyenTelefon = akademisyen.AkademisyenTelefon;
        mevcut.AkademisyenEposta = akademisyen.AkademisyenEposta;
        mevcut.AkademisyenTc = akademisyen.AkademisyenTc;
        mevcut.UpdatedDate = DateTime.Now;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            VeritabaniHatasiniIsle(ex);
            await BolumListesiniHazirlaAsync(akademisyen.BolumId);
            return View(akademisyen);
        }

        TempData["Basarili"] = $"{akademisyen.TamAd} güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════════════════
    //  6) SİLME ONAY SAYFASI
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Delete(long id)
    {
        var akademisyen = await _db.Akademisyenler
            .Include(a => a.Bolum)
                .ThenInclude(b => b!.Fakulte)
            .FirstOrDefaultAsync(a => a.AkademisyenId == id);

        if (akademisyen == null)
            return NotFound();

        return View(akademisyen);
    }

    // ══════════════════════════════════════════════════════
    //  7) SİLMEYİ ONAYLA
    // ══════════════════════════════════════════════════════
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var akademisyen = await _db.Akademisyenler.FindAsync(id);

        if (akademisyen == null)
            return NotFound();

        akademisyen.AktifMi = false;
        akademisyen.UpdatedDate = DateTime.Now;
        await _db.SaveChangesAsync();

        TempData["Basarili"] = "Akademisyen silindi.";
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════════════════
    //  8) DETAY
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Details(long id)
    {
        var akademisyen = await _db.Akademisyenler
            .Include(a => a.Bolum)
                .ThenInclude(b => b!.Fakulte)
            .FirstOrDefaultAsync(a => a.AkademisyenId == id);

        if (akademisyen == null)
            return NotFound();

        return View(akademisyen);
    }

    // ══════════════════════════════════════════════════════
    //  9) CSV DIŞA AKTARMA
    //
    //  İki Türkiye'ye özgü detay:
    //    • Türk Excel'i ayraç olarak ; bekler, , değil
    //    • UTF-8 BOM olmadan Türkçe karakterler bozuk açılır
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> CsvIndir()
    {
        var liste = await _db.Akademisyenler
            .Include(a => a.Bolum)
                .ThenInclude(b => b!.Fakulte)
            .OrderBy(a => a.AkademisyenAd)
            .Select(a => new
            {
                a.Unvan,
                a.AkademisyenAd,
                a.AkademisyenSoyad,
                BolumAdi = a.Bolum!.BolumAdi,
                FakulteAd = a.Bolum!.Fakulte!.FakulteAd,
                a.AkademisyenTelefon,
                a.AkademisyenEposta
            })
            .ToListAsync();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Unvan;Ad;Soyad;Bolum;Fakulte;Telefon;Eposta");

        foreach (var a in liste)
        {
            sb.AppendLine($"{a.Unvan};{a.AkademisyenAd};{a.AkademisyenSoyad};" +
                          $"{a.BolumAdi};{a.FakulteAd};" +
                          $"{a.AkademisyenTelefon};{a.AkademisyenEposta}");
        }

        var bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var icerik = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        var dosya = bom.Concat(icerik).ToArray();

        return File(dosya, "text/csv", "akademisyenler.csv");
    }
}
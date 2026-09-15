using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UBYS.Data;
using UBYS.Models;

namespace UBYS.Controllers;

public class OgrenciController : Controller
{
    private readonly UbysDbContext _db;

    public OgrenciController(UbysDbContext db)
    {
        _db = db;
    }

    // ══════════════════════════════════════════════════════
    //  YARDIMCI: bölüm açılır listesi
    //
    //  ⭐ Select ile SADECE ihtiyacımız olan iki alanı çekiyoruz.
    //     Tüm Bolum nesnesini çekmeye gerek yok — açılır listede
    //     yalnızca id ve ad kullanılacak.
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
                // Aynı adlı bölüm farklı fakültelerde olabilir — karışmasın
                Ad = b.BolumAdi + " (" + b.Fakulte!.FakulteAd + ")"
            })
            .ToListAsync();

        ViewBag.Bolumler = new SelectList(bolumler, "BolumId", "Ad", secili);
    }

    // ══════════════════════════════════════════════════════
    //  YARDIMCI: benzersizlik ön kontrolü
    //
    //  guncellenenId: düzenleme yaparken kaydın KENDİSİNİ
    //  çakışma sayma. Yoksa kendi e-postasıyla çakışır.
    // ══════════════════════════════════════════════════════
    private async Task BenzersizlikKontrolAsync(Ogrenci ogrenci, long guncellenenId = 0)
    {
        bool epostaVar = await _db.Ogrenciler
            .AnyAsync(o => o.OgrenciEposta == ogrenci.OgrenciEposta
                        && o.OgrenciId != guncellenenId);

        if (epostaVar)
            ModelState.AddModelError(nameof(Ogrenci.OgrenciEposta),
                "Bu e-posta adresi başka bir öğrenciye kayıtlı.");

        bool tcVar = await _db.Ogrenciler
            .AnyAsync(o => o.OgrenciTc == ogrenci.OgrenciTc
                        && o.OgrenciId != guncellenenId);

        if (tcVar)
            ModelState.AddModelError(nameof(Ogrenci.OgrenciTc),
                "Bu TC kimlik numarası başka bir öğrenciye kayıtlı.");
    }

    // ══════════════════════════════════════════════════════
    //  YARDIMCI: veritabanı hatasını kullanıcı diline çevir
    //
    //  ⚠️ ex.Message'ı OLDUĞU GİBİ GÖSTERME!
    //     Tablo ve indeks adlarını ifşa eder.
    // ══════════════════════════════════════════════════════
    private void VeritabaniHatasiniIsle(DbUpdateException ex)
    {
        if (ex.InnerException is SqlException sqlEx &&
            (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        {
            string mesaj = sqlEx.Message;

            if (mesaj.Contains("Eposta"))
                ModelState.AddModelError(nameof(Ogrenci.OgrenciEposta),
                    "Bu e-posta adresi başka bir öğrenciye kayıtlı.");
            else if (mesaj.Contains("Tc"))
                ModelState.AddModelError(nameof(Ogrenci.OgrenciTc),
                    "Bu TC kimlik numarası başka bir öğrenciye kayıtlı.");
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
    public async Task<IActionResult> Index()
    {
        var ogrenciler = await _db.Ogrenciler
            .Include(o => o.Bolum)                    // bölüm
                .ThenInclude(b => b!.Fakulte)         // ⭐ bölümün fakültesi
            .OrderBy(o => o.OgrenciAd)
            .ThenBy(o => o.OgrenciSoyad)
            .ToListAsync();

        return View(ogrenciler);
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
    public async Task<IActionResult> Create(Ogrenci ogrenci)
    {
        // Önden kontrol — güzel hata mesajı için
        await BenzersizlikKontrolAsync(ogrenci);

        if (!ModelState.IsValid)
        {
            await BolumListesiniHazirlaAsync(ogrenci.BolumId);
            return View(ogrenci);
        }

        ogrenci.CreatedDate = DateTime.Now;
        ogrenci.UpdatedDate = null;
        ogrenci.AktifMi = true;

        try
        {
            _db.Ogrenciler.Add(ogrenci);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Yarış koşulu ağa takıldı — nadir ama mümkün
            VeritabaniHatasiniIsle(ex);
            await BolumListesiniHazirlaAsync(ogrenci.BolumId);
            return View(ogrenci);
        }

        TempData["Basarili"] = $"{ogrenci.TamAd} kaydedildi.";
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════════════════
    //  4) DÜZENLEME FORMU
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Edit(long id)
    {
        var ogrenci = await _db.Ogrenciler.FindAsync(id);

        if (ogrenci == null)
            return NotFound();

        await BolumListesiniHazirlaAsync(ogrenci.BolumId);
        return View(ogrenci);
    }

    // ══════════════════════════════════════════════════════
    //  5) DÜZENLEMEYİ KAYDET
    // ══════════════════════════════════════════════════════
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Ogrenci ogrenci)
    {
        // ⭐ Kendi kaydını çakışma sayma
        await BenzersizlikKontrolAsync(ogrenci, ogrenci.OgrenciId);

        if (!ModelState.IsValid)
        {
            await BolumListesiniHazirlaAsync(ogrenci.BolumId);
            return View(ogrenci);
        }

        var mevcut = await _db.Ogrenciler.FindAsync(ogrenci.OgrenciId);

        if (mevcut == null)
            return NotFound();

        mevcut.BolumId = ogrenci.BolumId;
        mevcut.OgrenciAd = ogrenci.OgrenciAd;
        mevcut.OgrenciSoyad = ogrenci.OgrenciSoyad;
        mevcut.OgrenciSinif = ogrenci.OgrenciSinif;
        mevcut.OgrenciDogumTarihi = ogrenci.OgrenciDogumTarihi;
        mevcut.OgrenciCinsiyet = ogrenci.OgrenciCinsiyet;
        mevcut.OgrenciAdres = ogrenci.OgrenciAdres;
        mevcut.OgrenciTelefon = ogrenci.OgrenciTelefon;
        mevcut.OgrenciEposta = ogrenci.OgrenciEposta;
        mevcut.OgrenciTc = ogrenci.OgrenciTc;
        mevcut.UpdatedDate = DateTime.Now;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            VeritabaniHatasiniIsle(ex);
            await BolumListesiniHazirlaAsync(ogrenci.BolumId);
            return View(ogrenci);
        }

        TempData["Basarili"] = $"{ogrenci.TamAd} güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════════════════
    //  6) SİLME ONAY SAYFASI
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Delete(long id)
    {
        var ogrenci = await _db.Ogrenciler
            .Include(o => o.Bolum)
                .ThenInclude(b => b!.Fakulte)
            .FirstOrDefaultAsync(o => o.OgrenciId == id);

        if (ogrenci == null)
            return NotFound();

        return View(ogrenci);
    }

    // ══════════════════════════════════════════════════════
    //  7) SİLMEYİ ONAYLA
    // ══════════════════════════════════════════════════════
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var ogrenci = await _db.Ogrenciler.FindAsync(id);

        if (ogrenci == null)
            return NotFound();

        ogrenci.AktifMi = false;
        ogrenci.UpdatedDate = DateTime.Now;
        await _db.SaveChangesAsync();

        TempData["Basarili"] = "Öğrenci silindi.";
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════════════════
    //  8) DETAY
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Details(long id)
    {
        var ogrenci = await _db.Ogrenciler
            .Include(o => o.Bolum)
                .ThenInclude(b => b!.Fakulte)
            .FirstOrDefaultAsync(o => o.OgrenciId == id);

        if (ogrenci == null)
            return NotFound();

        return View(ogrenci);
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UBYS.Data;
using UBYS.Models;

namespace UBYS.Controllers;

public class BolumController : Controller
{
    // ⭐ ADO.NET'te İKİ repository enjekte ediyorduk
    //    (BolumRepository + FakulteRepository).
    //    EF'te tek DbContext yeter — içinde tüm tablolar var.
    private readonly UbysDbContext _db;

    public BolumController(UbysDbContext db)
    {
        _db = db;
    }

    // ══════════════════════════════════════════════════════
    //  YARDIMCI: fakülte açılır listesi
    // ══════════════════════════════════════════════════════
    private async Task FakulteListesiniHazirlaAsync(long? secili = null)
    {
        var fakulteler = await _db.Fakulteler
            .OrderBy(f => f.FakulteAd)
            .ToListAsync();

        ViewBag.Fakulteler = new SelectList(fakulteler, "FakulteId", "FakulteAd", secili);
    }

    // ══════════════════════════════════════════════════════
    //  1) LİSTELEME
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Index()
    {
        var bolumler = await _db.Bolumler
            .Include(b => b.Fakulte)              // ⭐ JOIN
            .OrderBy(b => b.Fakulte!.FakulteAd)   // fakülte adına göre
            .ThenBy(b => b.BolumAdi)              // sonra bölüm adına göre
            .ToListAsync();

        return View(bolumler);
    }

    // ══════════════════════════════════════════════════════
    //  2) YENİ KAYIT FORMU
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Create()
    {
        await FakulteListesiniHazirlaAsync();
        return View();
    }

    // ══════════════════════════════════════════════════════
    //  3) YENİ KAYDI KAYDET
    // ══════════════════════════════════════════════════════
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Bolum bolum)
    {
        if (!ModelState.IsValid)
        {
            // ⭐ EN ÇOK UNUTULAN SATIR
            // ViewBag sadece o istek boyunca yaşar. POST yeni bir istektir.
            // Doldurmazsak açılır liste boş gelir ve sayfa çöker.
            await FakulteListesiniHazirlaAsync(bolum.FakulteId);
            return View(bolum);
        }

        bolum.CreatedDate = DateTime.Now;
        bolum.UpdatedDate = null;
        bolum.AktifMi = true;

        // ⚠️ ÖNEMLİ: bolum.Fakulte navigasyon özelliği NULL.
        //    Bu SORUN DEĞİL — EF, FakulteId alanına bakar.
        //    Eğer Fakulte nesnesini de doldursaydık EF onu
        //    YENİ BİR FAKÜLTE sanıp INSERT etmeye çalışabilirdi.
        _db.Bolumler.Add(bolum);
        await _db.SaveChangesAsync();

        TempData["Basarili"] = $"\"{bolum.BolumAdi}\" kaydedildi.";
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════════════════
    //  4) DÜZENLEME FORMU
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Edit(long id)
    {
        var bolum = await _db.Bolumler.FindAsync(id);

        if (bolum == null)
            return NotFound();

        await FakulteListesiniHazirlaAsync(bolum.FakulteId);
        return View(bolum);
    }

    // ══════════════════════════════════════════════════════
    //  5) DÜZENLEMEYİ KAYDET
    // ══════════════════════════════════════════════════════
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Bolum bolum)
    {
        if (!ModelState.IsValid)
        {
            await FakulteListesiniHazirlaAsync(bolum.FakulteId);
            return View(bolum);
        }

        var mevcut = await _db.Bolumler.FindAsync(bolum.BolumId);

        if (mevcut == null)
            return NotFound();

        mevcut.FakulteId = bolum.FakulteId;      // bölüm başka fakülteye taşınabilir
        mevcut.BolumAdi = bolum.BolumAdi;
        mevcut.BolumAdres = bolum.BolumAdres;
        mevcut.BolumTelefon = bolum.BolumTelefon;
        mevcut.BolumEposta = bolum.BolumEposta;
        mevcut.UpdatedDate = DateTime.Now;

        await _db.SaveChangesAsync();

        TempData["Basarili"] = "Bölüm güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════════════════
    //  6) SİLME ONAY SAYFASI
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Delete(long id)
    {
        var bolum = await _db.Bolumler
            .Include(b => b.Fakulte)
            .FirstOrDefaultAsync(b => b.BolumId == id);

        if (bolum == null)
            return NotFound();

        // Bağlı kayıt sayıları — kullanıcıya önceden göster
        ViewBag.OgrenciSayisi = await _db.Ogrenciler.CountAsync(o => o.BolumId == id);
        ViewBag.AkademisyenSayisi = await _db.Akademisyenler.CountAsync(a => a.BolumId == id);

        return View(bolum);
    }

    // ══════════════════════════════════════════════════════
    //  7) SİLMEYİ ONAYLA
    // ══════════════════════════════════════════════════════
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var bolum = await _db.Bolumler.FindAsync(id);

        if (bolum == null)
            return NotFound();

        int ogrenciSayisi = await _db.Ogrenciler.CountAsync(o => o.BolumId == id);
        int akademisyenSayisi = await _db.Akademisyenler.CountAsync(a => a.BolumId == id);

        if (ogrenciSayisi > 0 || akademisyenSayisi > 0)
        {
            TempData["Uyari"] = $"Bu bölümde {ogrenciSayisi} öğrenci ve " +
                                $"{akademisyenSayisi} akademisyen var. Önce onları taşıyın.";
            return RedirectToAction(nameof(Index));
        }

        bolum.AktifMi = false;
        bolum.UpdatedDate = DateTime.Now;
        await _db.SaveChangesAsync();

        TempData["Basarili"] = "Bölüm silindi.";
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════════════════
    //  8) DETAY
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Details(long id)
    {
        // ⭐ Üç seviyeli yükleme: bölüm + fakültesi + öğrencileri + akademisyenleri
        var bolum = await _db.Bolumler
            .Include(b => b.Fakulte)
            .Include(b => b.Ogrenciler)
            .Include(b => b.Akademisyenler)
            .FirstOrDefaultAsync(b => b.BolumId == id);

        if (bolum == null)
            return NotFound();

        return View(bolum);
    }
}
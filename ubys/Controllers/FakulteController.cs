using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UBYS.Data;
using UBYS.Models;

namespace UBYS.Controllers;

public class FakulteController : Controller
{
    private readonly UbysDbContext _db;

    public FakulteController(UbysDbContext db)
    {
        _db = db;
    }

    // ══════════════════════════════════════════════════════
    //  1) LİSTELEME
    //  GET: /Fakulte
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Index()
    {
        // Query filter sayesinde .Where(f => f.AktifMi) yazmıyoruz
        var fakulteler = await _db.Fakulteler
            .OrderBy(f => f.FakulteId)
            .ToListAsync();

        return View(fakulteler);
    }

    // ══════════════════════════════════════════════════════
    //  2) YENİ KAYIT FORMU
    //  GET: /Fakulte/Create
    // ══════════════════════════════════════════════════════
    public IActionResult Create()
    {
        return View();
    }

    // ══════════════════════════════════════════════════════
    //  3) YENİ KAYDI KAYDET
    //  POST: /Fakulte/Create
    // ══════════════════════════════════════════════════════
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Fakulte fakulte)
    {
        if (!ModelState.IsValid)
            return View(fakulte);

        // Sistem alanlarını biz dolduruyoruz — kullanıcıdan almıyoruz
        fakulte.CreatedDate = DateTime.Now;
        fakulte.UpdatedDate = null;
        fakulte.AktifMi = true;

        // ⭐ İKİ SATIRLIK EKLEME
        // ADO.NET'te 15 satırlık INSERT + parametreler vardı.
        _db.Fakulteler.Add(fakulte);        // durumu: Added
        await _db.SaveChangesAsync();       // INSERT çalışır

        // ⭐ Bonus: SaveChanges sonrası fakulte.FakulteId DOLMUŞ olur.
        //    EF, veritabanının ürettiği IDENTITY değerini nesneye geri yazar.
        //    ADO.NET'te bunun için SCOPE_IDENTITY() yazmamız gerekiyordu.

        TempData["Basarili"] = $"\"{fakulte.FakulteAd}\" kaydedildi.";
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════════════════
    //  4) DÜZENLEME FORMU
    //  GET: /Fakulte/Edit/5
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Edit(long id)
    {
        var fakulte = await _db.Fakulteler.FindAsync(id);

        // Kullanıcı adres çubuğuna olmayan bir id yazabilir
        if (fakulte == null)
            return NotFound();

        return View(fakulte);
    }

    // ══════════════════════════════════════════════════════
    //  5) DÜZENLEMEYİ KAYDET
    //  POST: /Fakulte/Edit/5
    // ══════════════════════════════════════════════════════
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Fakulte fakulte)
    {
        if (!ModelState.IsValid)
            return View(fakulte);

        // ⭐ ÖNCE VERİTABANINDAN ÇEK, SONRA DEĞİŞTİR
        //
        // Neden doğrudan _db.Update(fakulte) yazmıyoruz?
        //
        // Çünkü formda olmayan alanlar (CreatedDate, AktifMi) boş gelir.
        // Update() tüm sütunları yazar ve o alanları SIFIRLAR.
        //
        // Bu yöntemde sadece istediğimiz alanları değiştiriyoruz,
        // gerisine dokunmuyoruz. Daha güvenli.
        var mevcut = await _db.Fakulteler.FindAsync(fakulte.FakulteId);

        if (mevcut == null)
            return NotFound();

        mevcut.FakulteAd = fakulte.FakulteAd;
        mevcut.FakulteAdres = fakulte.FakulteAdres;
        mevcut.FakulteTelefon = fakulte.FakulteTelefon;
        mevcut.FakulteEposta = fakulte.FakulteEposta;
        mevcut.UpdatedDate = DateTime.Now;

        // ⭐ Add/Update yok! "mevcut" nesnesi zaten takip ediliyor.
        //    EF neyin değiştiğini biliyor.
        await _db.SaveChangesAsync();

        TempData["Basarili"] = "Fakülte güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════════════════
    //  6) SİLME ONAY SAYFASI
    //  GET: /Fakulte/Delete/5
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Delete(long id)
    {
        var fakulte = await _db.Fakulteler.FindAsync(id);

        if (fakulte == null)
            return NotFound();

        // Silinebilir mi? Kullanıcıya ÖNCEDEN söyleyelim
        ViewBag.BolumSayisi = await _db.Bolumler
            .CountAsync(b => b.FakulteId == id);

        return View(fakulte);
    }

    // ══════════════════════════════════════════════════════
    //  7) SİLMEYİ ONAYLA — soft delete
    //  POST: /Fakulte/Delete/5
    // ══════════════════════════════════════════════════════
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        var fakulte = await _db.Fakulteler.FindAsync(id);

        if (fakulte == null)
            return NotFound();

        // İlişkili kayıt kontrolü
        int bolumSayisi = await _db.Bolumler.CountAsync(b => b.FakulteId == id);

        if (bolumSayisi > 0)
        {
            TempData["Uyari"] = $"Bu fakültede {bolumSayisi} bölüm var. " +
                                 "Önce bölümleri silmelisiniz.";
            return RedirectToAction(nameof(Index));
        }

        // ⭐ SOFT DELETE
        // _db.Remove(fakulte) yazsaydık gerçekten SİLİNİRDİ.
        // Biz sadece işaretliyoruz. Query filter onu listeden gizleyecek.
        fakulte.AktifMi = false;
        fakulte.UpdatedDate = DateTime.Now;

        await _db.SaveChangesAsync();

        TempData["Basarili"] = "Fakülte silindi.";
        return RedirectToAction(nameof(Index));
    }

    // ══════════════════════════════════════════════════════
    //  8) DETAY
    //  GET: /Fakulte/Details/5
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Details(long id)
    {
        // ⭐ Include ile bölümleri de getiriyoruz
        // (Modül 5'te ayrıntılı anlatacağız)
        var fakulte = await _db.Fakulteler
            .Include(f => f.Bolumler)
            .FirstOrDefaultAsync(f => f.FakulteId == id);

        if (fakulte == null)
            return NotFound();

        return View(fakulte);
    }

    // ══════════════════════════════════════════════════════
    //  9) PASİF KAYITLAR
    //  GET: /Fakulte/Pasifler
    // ══════════════════════════════════════════════════════
    public async Task<IActionResult> Pasifler()
    {
        var pasifler = await _db.Fakulteler
            .IgnoreQueryFilters()            // ⭐ soft delete filtresini atla
            .Where(f => !f.AktifMi)
            .OrderBy(f => f.FakulteAd)
            .ToListAsync();

        return View(pasifler);
    }

    // ══════════════════════════════════════════════════════
    //  10) GERİ GETİR
    //  POST: /Fakulte/GeriAl/5
    // ══════════════════════════════════════════════════════
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GeriAl(long id)
    {
        // Pasif kaydı bulmak için filtreyi atlamalıyız,
        // yoksa FindAsync bile onu bulamaz
        var fakulte = await _db.Fakulteler
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(f => f.FakulteId == id);

        if (fakulte == null)
            return NotFound();

        fakulte.AktifMi = true;
        fakulte.UpdatedDate = DateTime.Now;
        await _db.SaveChangesAsync();

        TempData["Basarili"] = "Fakülte geri getirildi.";
        return RedirectToAction(nameof(Pasifler));
    }
}
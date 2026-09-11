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
}
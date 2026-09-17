using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UBYS.Data;
using UBYS.Models;

namespace UBYS.Controllers;

public class HomeController : Controller
{
    private readonly UbysDbContext _db;

    public HomeController(UbysDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel();

        // ══════════════════════════════════════════════════
        //  1) SAYILAR
        //
        //  CountAsync → SQL'de COUNT(*) olur, veri taşınmaz.
        //  Query filter sayesinde WHERE AktifMi = 1 otomatik eklenir.
        // ══════════════════════════════════════════════════
        model.FakulteSayisi = await _db.Fakulteler.CountAsync();
        model.BolumSayisi = await _db.Bolumler.CountAsync();
        model.OgrenciSayisi = await _db.Ogrenciler.CountAsync();
        model.AkademisyenSayisi = await _db.Akademisyenler.CountAsync();

        // ══════════════════════════════════════════════════
        //  2) BÖLÜM DAĞILIMI
        //
        //  ⭐ Select içinde alt sorgu:
        //     b.Ogrenciler.Count() → SQL'de scalar subquery olur.
        //     Include'a GEREK YOK — EF gereken JOIN'i kendisi üretir.
        // ══════════════════════════════════════════════════
        model.BolumDagilimlari = await _db.Bolumler
            .Select(b => new BolumDagilim
            {
                BolumAdi = b.BolumAdi,
                FakulteAd = b.Fakulte!.FakulteAd,
                OgrenciSayisi = b.Ogrenciler.Count(),
                AkademisyenSayisi = b.Akademisyenler.Count()
            })
            .OrderByDescending(x => x.OgrenciSayisi)
            .ThenBy(x => x.BolumAdi)
            .ToListAsync();

        // ══════════════════════════════════════════════════
        //  3) SINIF DAĞILIMI — GroupBy
        //
        //  SQL karşılığı:
        //     SELECT ogrenci_sinif, COUNT(*)
        //     FROM Ogrenciler GROUP BY ogrenci_sinif
        // ══════════════════════════════════════════════════
        model.SinifDagilimlari = await _db.Ogrenciler
            .GroupBy(o => o.OgrenciSinif)        // neye göre grupla
            .Select(g => new SinifDagilim
            {
                Sinif = g.Key,           // ⭐ g.Key = gruplanan değer
                OgrenciSayisi = g.Count()        // o gruptaki satır sayısı
            })
            .OrderBy(x => x.Sinif)
            .ToListAsync();

        // ══════════════════════════════════════════════════
        //  4) CİNSİYET DAĞILIMI — yine GroupBy
        // ══════════════════════════════════════════════════
        model.CinsiyetDagilimlari = await _db.Ogrenciler
            .GroupBy(o => o.OgrenciCinsiyet)
            .Select(g => new CinsiyetDagilim
            {
                Cinsiyet = g.Key,
                OgrenciSayisi = g.Count()
            })
            .OrderByDescending(x => x.OgrenciSayisi)
            .ToListAsync();

        // ══════════════════════════════════════════════════
        //  5) SON EKLENEN ÖĞRENCİLER
        //
        //  Take(5) → SQL'de TOP(5) olur.
        //  ⚠️ SIRA ÖNEMLİ: OrderBy önce, Take sonra.
        //     Ters yazsaydık rastgele 5 kayıt alıp onları sıralardık.
        // ══════════════════════════════════════════════════
        model.SonEklenenOgrenciler = await _db.Ogrenciler
            .OrderByDescending(o => o.CreatedDate)
            .Take(5)
            .Select(o => new SonOgrenci
            {
                OgrenciId = o.OgrenciId,
                TamAd = o.OgrenciAd + " " + o.OgrenciSoyad,
                BolumAdi = o.Bolum!.BolumAdi,
                Sinif = o.OgrenciSinif,
                CreatedDate = o.CreatedDate
            })
            .ToListAsync();

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        ViewBag.HataKodu = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        return View();
    }
}
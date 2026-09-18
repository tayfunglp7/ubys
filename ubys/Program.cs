using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using UBYS.Data;

var builder = WebApplication.CreateBuilder(args);

// ── 1. MVC + "giriş zorunlu" filtresi ───────────────────────
//
// ⭐ Neden global filtre, her controller'a [Authorize] değil?
//    Yarın yeni bir controller yazıp [Authorize] koymayı unutursan
//    o sayfa herkese açık kalır. Global filtreyle varsayılan KAPALI olur.
//    GÜVENLİK İLKESİ: varsayılan hep en kısıtlayıcı seçenek olmalı.
builder.Services.AddControllersWithViews(secenekler =>
{
    var politika = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    secenekler.Filters.Add(new AuthorizeFilter(politika));
});

// ── 2. Çerez tabanlı kimlik doğrulama ───────────────────────
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(secenekler =>
    {
        secenekler.LoginPath = "/Hesap/Giris";
        secenekler.ReturnUrlParameter = "donusUrl";   // controller parametresiyle aynı olmalı!
        secenekler.ExpireTimeSpan = TimeSpan.FromHours(8);
        secenekler.SlidingExpiration = true;
        secenekler.Cookie.HttpOnly = true;             // JS erişemez → XSS koruması
        secenekler.Cookie.SameSite = SameSiteMode.Lax; // CSRF koruması
        secenekler.Cookie.Name = "UBYS.Oturum";
    });

// ══════════════════════════════════════════════════════════
//  ⭐ EF CORE KAYDI
//
//  ADO.NET'te dört satır yazıyorduk:
//      AddScoped<FakulteRepository>();
//      AddScoped<BolumRepository>();
//      AddScoped<OgrenciRepository>();
//      AddScoped<AkademisyenRepository>();
//
//  Şimdi tek satır:
// ══════════════════════════════════════════════════════════
builder.Services.AddDbContext<UbysDbContext>(secenekler =>
{
    secenekler.UseSqlServer(
        builder.Configuration.GetConnectionString("UbysDb"));

    // ⭐ Sadece GELİŞTİRME ortamında: üretilen SQL'i konsola yaz
    if (builder.Environment.IsDevelopment())
    {
        secenekler.LogTo(Console.WriteLine, LogLevel.Information);

        // Parametre DEĞERLERİNİ de göster
        // ⚠️ Canlıda ASLA açma — şifreler, TC numaraları loglara düşer!
        secenekler.EnableSensitiveDataLogging();
    }
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ⭐⭐ SIRA KRİTİK
app.UseAuthentication();   // ÖNCE: "sen kimsin?" (çerezi okur)
app.UseAuthorization();    // SONRA: "girebilir mi?" (filtreyi uygular)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
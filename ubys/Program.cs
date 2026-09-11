using Microsoft.EntityFrameworkCore;
using UBYS.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
using System.Security.Cryptography;
using System.Text;

namespace UBYS.Data;

/// <summary>
/// Şifre hash'leme işlemleri.
///
/// ⚠️ ÖĞRETİM AMAÇLIDIR. Gerçek projede BCrypt/Argon2 kullanın.
/// </summary>
public static class SifreYardimcisi
{
    /// <summary>
    /// Metnin SHA256 özetini büyük harfli onaltılık metin olarak döner.
    ///
    /// Derste çalıştırıp göster:
    ///     SifreYardimcisi.Hashle("ubys123")
    /// Çıktı, veritabanındaki değerle aynı olmalı.
    /// </summary>
    public static string Hashle(string sifre)
    {
        byte[] bayt = Encoding.UTF8.GetBytes(sifre);   // metni bayta çevir
        byte[] hash = SHA256.HashData(bayt);           // özeti hesapla
        return Convert.ToHexString(hash);              // okunabilir metne çevir
    }
}
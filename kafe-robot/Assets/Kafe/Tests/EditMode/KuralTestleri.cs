// Kuralların Edit Mode testleri: oyun çalıştırılmadan, saniyeler içinde koşar.
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Kafe.Testler
{
    // Ders 1.4 — hedefe en çok adım kadar yaklaş
    public class YaklasTestleri
    {
        static void Yakin(Vector2 beklenen, Vector2 gelen)
        {
            Assert.IsFalse(float.IsNaN(gelen.x) || float.IsNaN(gelen.y), "sonuç sayı değil (NaN)");
            Assert.Less((beklenen - gelen).magnitude, 1e-4f, $"beklenen {beklenen}, gelen {gelen}");
        }

        [Test] public void Uzak_Hedefe_Bir_Adim() => Yakin(new Vector2(0.5f, 0f), Hareket.Yaklas(Vector2.zero, new Vector2(3f, 0f), 0.5f));
        [Test] public void Capraz_Yon() => Yakin(new Vector2(0.6f, 0.8f), Hareket.Yaklas(Vector2.zero, new Vector2(3f, 4f), 1f));
        [Test] public void Sifir_Adim_Yerinde_Kalir() => Yakin(new Vector2(1f, 1f), Hareket.Yaklas(new Vector2(1f, 1f), new Vector2(2f, 2f), 0f));
        [Test] public void Yakin_Hedefi_Gecmez() => Yakin(new Vector2(0.2f, 0f), Hareket.Yaklas(Vector2.zero, new Vector2(0.2f, 0f), 0.5f));
        [Test] public void Eksi_Adim_Geri_Gitmez() => Yakin(new Vector2(0f, 0f), Hareket.Yaklas(Vector2.zero, new Vector2(3f, 0f), -0.5f));
        [Test] public void Hedefteyken_Yerinde_Kalir() => Yakin(new Vector2(1f, 1f), Hareket.Yaklas(new Vector2(1f, 1f), new Vector2(1f, 1f), 0.5f));
    }

    // Ders 3.1 — puan kuralı
    public class PuanTestleri
    {
        [TestCase(true, false, 12f, 0, 1360, TestName = "Hizli_Ve_Temiz")]
        [TestCase(true, false, 28f, 0, 1040, TestName = "Yavas_Ama_Temiz")]
        [TestCase(true, false, 12f, 2, 1160, TestName = "Hizli_Iki_Carpma")]
        [TestCase(false, true, 9f, 0, 0, TestName = "Dokulmus")]
        public void Puan_Kurali(bool teslim, bool dokuldu, float sure, int carpma, int beklenen)
            => Assert.AreEqual(beklenen, Puan.Hesapla(teslim, dokuldu, sure, carpma));

        [Test]
        public void Puan_Olcumu()
        {
            Olcum.Yaz("puan", new Dictionary<string, object>
            {
                ["hizli_temiz"] = Puan.Hesapla(true, false, 12f, 0), ["yavas_temiz"] = Puan.Hesapla(true, false, 28f, 0),
                ["hizli_iki_carpma"] = Puan.Hesapla(true, false, 12f, 2), ["dokulmus"] = Puan.Hesapla(false, true, 9f, 0),
                ["teslim_puani"] = Puan.TeslimPuani, ["sure_siniri_s"] = Puan.SureSiniri, ["saniye_odulu"] = Puan.SaniyeOdulu, ["carpma_cezasi"] = Puan.CarpmaCezasi,
            });
        }
    }

    // Ders 3.3 — durum makinesi
    public class DurumTestleri
    {
        [Test]
        public void Menuden_Dogrudan_Teslime_Gecilmez()
        {
            var d = new DurumMakinesi();
            Assert.IsFalse(d.Gec(OyunDurumu.Teslim));
            Assert.AreEqual(OyunDurumu.Menu, d.Simdi);
            Assert.AreEqual(1, d.Reddedilen);
        }

        [Test]
        public void Istek_Dizisi()
        {
            var d = new DurumMakinesi();
            var istek = new[] { OyunDurumu.Teslim, OyunDurumu.Oyun, OyunDurumu.Oyun, OyunDurumu.Dokuldu, OyunDurumu.Teslim, OyunDurumu.Menu, OyunDurumu.Oyun, OyunDurumu.Teslim };
            var adim = new List<object>(); int kabul = 0;
            foreach (var i in istek)
            {
                var once = d.Simdi; bool ok = d.Gec(i); if (ok) kabul++;
                adim.Add($"{once} > {i}: {(ok ? "kabul" : "RED")}");
            }
            Olcum.Yaz("durum", new Dictionary<string, object>
            {
                ["durum_sayisi"] = System.Enum.GetValues(typeof(OyunDurumu)).Length, ["gecis_sayisi"] = DurumMakinesi.GecisSayisi,
                ["istek"] = istek.Length, ["kabul"] = kabul, ["red"] = d.Reddedilen, ["adimlar"] = adim, ["son"] = d.Simdi.ToString(),
            });
            Assert.AreEqual(istek.Length, kabul + d.Reddedilen);
            Assert.AreEqual(OyunDurumu.Teslim, d.Simdi);
        }
    }

    // Ders 3.4 — daire ile kutu değiyor mu
    public class SeviyeDenetimiTestleri
    {
        static readonly Vector2 Kare = new Vector2(0.4f, 0.4f);
        static bool D(float x, float y, Vector2 yari) => SeviyeDenetimi.DaireKutuDegiyor(new Vector2(x, y), 0.25f, Vector2.zero, yari);

        [Test] public void Yandan_Deger() => Assert.IsTrue(D(0.6f, 0f, Kare), "kenara 0,2 m, yarıçap 0,25");
        [Test] public void Uzakta_Degmez() => Assert.IsFalse(D(2f, 0f, Kare), "2 m ötede");
        [Test] public void Icinde_Deger() => Assert.IsTrue(D(0.1f, 0.1f, Kare), "merkez kutunun içinde");
        [Test] public void Kose_Yakini_Degmez() => Assert.IsFalse(D(0.62f, 0.62f, Kare), "köşeye uzaklık 0,31");
        [Test] public void Ince_Kutunun_Ustu_Degmez() => Assert.IsFalse(D(0f, 0.5f, new Vector2(0.8f, 0.1f)), "uzun ince tezgâh, 0,15 m boşluk");
        [Test] public void Koseye_Deger() => Assert.IsTrue(D(0.55f, 0.55f, Kare), "köşeye uzaklık 0,21");

        [Test]
        public void Seviye_Dosyasi_Temiz()
        {
            var json = System.IO.File.ReadAllText(Application.dataPath + "/Kafe/Seviyeler/seviye-1.json");
            var s = SeviyeVerisi.Oku(json);
            var sorun = SeviyeDenetimi.Denetle(s, 0.25f);
            Olcum.Yaz("seviye", new Dictionary<string, object>
            {
                ["masa"] = s.masalar.Length, ["karakter"] = json.Length, ["sorun"] = sorun.Count,
                ["baslangic"] = new[] { s.baslangic.x, s.baslangic.y }, ["teslim"] = new[] { s.teslim.x, s.teslim.y },
            });
            Assert.AreEqual(0, sorun.Count, string.Join(", ", sorun));
        }
    }

    // Ders 4.2 — dp ve güvenli alan
    public class EkranTestleri
    {
        [Test]
        public void Dp_Piksel_Cevirisi()
        {
            Assert.AreEqual(48f, DokunmaAlani.Piksel(48f, 160f), 1e-3f);
            Assert.AreEqual(144f, DokunmaAlani.Piksel(48f, 480f), 1e-3f);
            Olcum.Yaz("ekran-dp", new Dictionary<string, object>
            {
                ["dp"] = DokunmaAlani.OnerilenDp, ["px_160"] = DokunmaAlani.Piksel(48f, 160f), ["px_320"] = DokunmaAlani.Piksel(48f, 320f),
                ["px_480"] = DokunmaAlani.Piksel(48f, 480f), ["mm"] = Olcum.Yuvarla(DokunmaAlani.Milimetre(48f), 2),
            });
        }

        [Test]
        public void Guvenli_Alan_Disindaki_Dugme_Bulunur()
        {
            var guvenli = new Rect(0, 60, 1080, 1920 - 60 - 120);
            Assert.IsFalse(DokunmaAlani.GuvenliAlanda(new Rect(40, 40, 300, 144), guvenli));
            Assert.IsTrue(DokunmaAlani.GuvenliAlanda(new Rect(40, 100, 300, 144), guvenli));
        }
    }

    // Ders 1.3 — dokunma evreleri
    public class EvreTestleri
    {
        [Test]
        public void Dokunma_Evreleri()
        {
            var adlar = System.Enum.GetNames(typeof(TouchPhase));
            Olcum.Yaz("evre", new Dictionary<string, object> { ["evre_sayisi"] = adlar.Length, ["adlar"] = adlar.ToList<object>() });
            CollectionAssert.Contains(adlar, "Began"); CollectionAssert.Contains(adlar, "Canceled");
        }
    }
}

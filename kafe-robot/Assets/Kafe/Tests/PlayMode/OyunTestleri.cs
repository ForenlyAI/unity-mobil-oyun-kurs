// Oyunun bütünü Play modunda, Kafe sahnesinde oynatılır (Ders 1.3, 2.2, 2.3, 2.4, 3.1, 4.1, 4.4).
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Kafe.Testler
{
    public class OyunTestleri
    {
        OyunYoneticisi oyun;

        [TearDown] public void Temizle() { Time.captureFramerate = 0; }

        IEnumerator SahneyiAc(int fps = 60)
        {
            SceneManager.LoadScene("Kafe", LoadSceneMode.Single);
            yield return null; yield return null;
            oyun = Object.FindFirstObjectByType<OyunYoneticisi>();
            Assert.IsNotNull(oyun, "sahnede OyunYoneticisi olmalı");
            Application.targetFrameRate = -1; QualitySettings.vSyncCount = 0;   // testte beklemeden koş
            Time.captureFramerate = fps;
        }

        static DokunusListesi Liste(string ad)
            => DokunusListesi.Oku(Resources.Load<TextAsset>(ad) != null ? Resources.Load<TextAsset>(ad).text : DosyaOku(ad));

        static string DosyaOku(string ad) => System.IO.File.ReadAllText(System.IO.Path.Combine(Application.dataPath, "Kafe/Seviyeler", ad + ".json"));

        IEnumerator Oyna(string kayit, float azamiIvme, int fps, Dictionary<string, object> sonuc)
        {
            yield return SahneyiAc(fps);
            oyun.robot.azamiIvme = azamiIvme;
            oyun.KaydiOynat(Liste(kayit));
            int bekci = 0;
            while (!oyun.Bitti && bekci++ < 20000) yield return null;
            Assert.IsTrue(oyun.Bitti, "oyun bitmeliydi");
            sonuc["durum"] = oyun.Durum.Simdi.ToString();
            sonuc["sure_s"] = Olcum.Yuvarla(oyun.Sure, 2);
            sonuc["kare"] = oyun.Kare;
            sonuc["sabit_adim"] = oyun.SabitAdim;
            sonuc["carpma"] = oyun.robot.Carpma;
            sonuc["en_buyuk_ivme"] = Olcum.Yuvarla(oyun.robot.EnBuyukIvme, 2);
            sonuc["puan"] = oyun.PuanDegeri;
            sonuc["son_x"] = Olcum.Yuvarla(oyun.robot.Konum.x, 2);
            sonuc["son_y"] = Olcum.Yuvarla(oyun.robot.Konum.y, 2);
        }

        [UnityTest]
        public IEnumerator Dokunus_Robotu_Hedefe_Goturur()
        {
            yield return SahneyiAc();
            var rt = new RenderTexture(1080, 1920, 16);
            oyun.kamera.targetTexture = rt;                       // ÖRNEK telefon ekranı: 1080 × 1920
            var px = new Vector2(480f, 900f);
            oyun.Dokun(px, TouchPhase.Began);
            Assert.AreEqual(OyunDurumu.Oyun, oyun.Durum.Simdi, "ilk dokunuş oyunu başlatır");
            var hedef = oyun.robot.Hedef.Value;
            oyun.Dokun(px, TouchPhase.Ended);
            oyun.kamera.targetTexture = null; rt.Release();       // çeviri bitti; büyük dokuya çizmeyi sürdürme
            Assert.IsTrue(oyun.robot.Hedef.HasValue, "parmak kalkınca hedef kalır");
            while (oyun.Sure < 12f && !oyun.Bitti) yield return null;
            float kalan = (oyun.robot.Konum - hedef).magnitude;
            Olcum.Yaz("dokunma-oyun", new Dictionary<string, object>
            {
                ["piksel"] = new[] { px.x, px.y }, ["hedef_x"] = Olcum.Yuvarla(hedef.x, 2), ["hedef_y"] = Olcum.Yuvarla(hedef.y, 2),
                ["sure_s"] = Olcum.Yuvarla(oyun.Sure, 2), ["kalan_m"] = Olcum.Yuvarla(kalan, 2), ["carpma"] = oyun.robot.Carpma,
            });
            Assert.Less(kalan, 0.05f, "robot hedefe varmalı");
        }

        [UnityTest]
        public IEnumerator Ornek_Oyun_Teslim_Eder()
        {
            var s = new Dictionary<string, object>();
            yield return Oyna("ornek-dokunuslar", 1.0f, 60, s);
            Olcum.Yaz("oyun-ornek", s);
            Assert.AreEqual("Teslim", s["durum"]);
            Assert.AreEqual(0, s["carpma"]);
            Assert.Greater((int)s["puan"], 1000);
        }

        [UnityTest]
        public IEnumerator Dogrudan_Yol_Masaya_Carpar()
        {
            var s = new Dictionary<string, object>();
            yield return Oyna("dogrudan-dokunus", 1.0f, 60, s);
            Olcum.Yaz("oyun-dogrudan", s);
            Assert.GreaterOrEqual((int)s["carpma"], 1, "yolda masa var");
        }

        [UnityTest]
        public IEnumerator Sinirsiz_Ivme_Bardagi_Doker()
        {
            var s = new Dictionary<string, object>();
            yield return Oyna("ornek-dokunuslar", 0f, 60, s);
            Olcum.Yaz("oyun-sinirsiz", s);
            Assert.AreEqual("Dokuldu", s["durum"]);
            Assert.AreEqual(0, s["puan"]);
        }

        [UnityTest]
        public IEnumerator Ivme_Siniri_Sureyi_Ve_Riski_Degistirir()
        {
            var o = new Dictionary<string, object>();
            foreach (float a in new[] { 0.5f, 1.0f, 2.0f, 4.0f })
            {
                var s = new Dictionary<string, object>();
                yield return Oyna("ornek-dokunuslar", a, 60, s);
                o[a.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture)] = s;
            }
            Olcum.Yaz("oyun-ivme-sinirlari", o);
            Assert.AreEqual("Teslim", ((Dictionary<string, object>)o["1.0"])["durum"]);
            Assert.AreEqual("Dokuldu", ((Dictionary<string, object>)o["4.0"])["durum"], "eşiğin üstündeki sınır bardağı döker");
        }

        [UnityTest]
        public IEnumerator Ayni_Girdi_Ayni_Sonuc()
        {
            var a = new Dictionary<string, object>(); var b = new Dictionary<string, object>(); var c = new Dictionary<string, object>();
            yield return Oyna("ornek-dokunuslar", 1.0f, 60, a);
            yield return Oyna("ornek-dokunuslar", 1.0f, 60, b);
            yield return Oyna("ornek-dokunuslar", 1.0f, 30, c);
            bool ayni = true, ayni30 = true;
            foreach (var k in a.Keys) { if (!a[k].Equals(b[k])) ayni = false; if (k != "kare" && !a[k].Equals(c[k])) ayni30 = false; }
            Olcum.Yaz("oyun-tekrar", new Dictionary<string, object> { ["a"] = a, ["b"] = b, ["fps30"] = c, ["ayni"] = ayni, ["fps30_ayni"] = ayni30 });
            Assert.IsTrue(ayni, "aynı girdi aynı sonucu vermeli");
            Assert.IsTrue(ayni30, "30 kare/sn'de kare sayısı dışında sonuç aynı olmalı");
        }

        [UnityTest]
        public IEnumerator Kare_Suresi_Butceye_Sigar()
        {
            var olcum = new List<object>();
            foreach (int n in new[] { 1, 10, 100 })
            {
                yield return SahneyiAc();
                oyun.robot.GetComponent<Collider>().enabled = n == 1;
                var robotlar = new List<RobotSurucu> { oyun.robot };
                for (int k = 1; k < n; k++)
                {
                    var kopya = Object.Instantiate(oyun.robot.gameObject, new Vector3(0.4f + k % 10 * 0.4f, 0f, 1.0f + k / 10 * 0.6f), Quaternion.identity);
                    robotlar.Add(kopya.GetComponent<RobotSurucu>());
                }
                yield return null;
                foreach (var r in robotlar) r.HedefVer(new Vector2(4.0f, 7.5f));
                for (int k = 0; k < 60; k++) yield return null;      // ısınma
                var sw = Stopwatch.StartNew(); int kare = 300;
                for (int k = 0; k < kare; k++) yield return null;
                double ms = sw.Elapsed.TotalMilliseconds / kare;
                olcum.Add(new Dictionary<string, object> { ["robot"] = n, ["ms"] = Olcum.Yuvarla(ms, 3) });
            }
            Olcum.Yaz("kare-butcesi", new Dictionary<string, object>
            {
                ["butce60_ms"] = Olcum.Yuvarla(1000.0 / 60, 2), ["butce30_ms"] = Olcum.Yuvarla(1000.0 / 30, 2), ["olcum"] = olcum,
                ["not"] = "editörde Play modu, bu makinede; süre makineye göre değişir",
            });
            Assert.Pass();
        }
    }
}

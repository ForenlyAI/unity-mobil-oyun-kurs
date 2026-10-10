// Sahne, prefab, kamera, fizik ve ayar testleri: Unity Editor'ün kendi API'siyle (Edit Mode).
using System.Collections.Generic;
using System.Linq;
using Kafe.EditorAraclari;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Kafe.Testler
{
    // Ders 1.1 — sahne, nesne, bileşen
    public class SahneTestleri
    {
        [SetUp] public void Ac() => EditorSceneManager.OpenScene(SahneKurucu.SahneYolu, OpenSceneMode.Single);

        [Test]
        public void Sahnede_Nesneler_Ve_Bilesenler()
        {
            var sahne = EditorSceneManager.GetActiveScene();
            var kokler = sahne.GetRootGameObjects();
            var hepsi = kokler.SelectMany(k => k.GetComponentsInChildren<Transform>(true)).ToArray();
            var robot = GameObject.Find("Robot");
            var bilesen = robot.GetComponents<Component>().Select(c => c.GetType().Name).ToList();
            var masalar = Object.FindObjectsByType<MasaSablonu>(FindObjectsSortMode.None);
            int yalnizKonum = hepsi.Count(t => t.GetComponents<Component>().Length == 1);
            Olcum.Yaz("sahne", new Dictionary<string, object>
            {
                ["kok_nesne"] = kokler.Length, ["kokler"] = kokler.Select(k => k.name).ToList<object>(),
                ["toplam_nesne"] = hepsi.Length, ["masa"] = masalar.Length,
                ["robot_bilesen"] = bilesen.Count, ["robot_bilesenler"] = bilesen.ToList<object>(),
                ["robot_parca"] = robot.GetComponentsInChildren<Transform>(true).Length - 1,
                ["masa_bilesenler"] = masalar[0].GetComponents<Component>().Select(c => c.GetType().Name).ToList<object>(),
                ["yalniz_konum"] = yalnizKonum, ["yalniz_konum_adlar"] = hepsi.Where(t => t.GetComponents<Component>().Length == 1).Select(t => t.name).ToList<object>(),
            });
            Assert.AreEqual(4, masalar.Length);
            CollectionAssert.Contains(bilesen, "Rigidbody"); CollectionAssert.Contains(bilesen, "RobotSurucu"); CollectionAssert.Contains(bilesen, "Bardak");
            foreach (var t in hepsi) Assert.IsNotNull(t.GetComponent<Transform>(), "her nesnede Transform vardır");
        }

        // Ders 1.3 — ekran pikseli → kafe zemini
        [Test]
        public void Piksel_Zemine_Cevrilir()
        {
            var kam = Camera.main;
            var rt = new RenderTexture(1080, 1920, 16);
            kam.targetTexture = rt;
            var orta = EkranDunya.Kamerayla(kam, new Vector2(540, 960));
            var nokta = EkranDunya.Kamerayla(kam, new Vector2(480, 900));
            var oranla = EkranDunya.Dunyaya(new Vector2(480, 900), EkranDunya.ReferansEkran);
            Olcum.Yaz("dokunma", new Dictionary<string, object>
            {
                ["ekran_px"] = new[] { kam.pixelWidth, kam.pixelHeight }, ["dunya_m"] = new[] { EkranDunya.KafeBoyu.x, EkranDunya.KafeBoyu.y },
                ["kamera_boyu"] = kam.orthographicSize, ["kamera_yukseklik"] = kam.transform.position.y,
                ["orta_x"] = Olcum.Yuvarla(orta.x, 2), ["orta_y"] = Olcum.Yuvarla(orta.y, 2),
                ["nokta_px"] = new[] { 480, 900 }, ["nokta_x"] = Olcum.Yuvarla(nokta.x, 2), ["nokta_y"] = Olcum.Yuvarla(nokta.y, 2),
                ["metre_basina_px"] = Olcum.Yuvarla(1080 / EkranDunya.KafeBoyu.x, 0),
            });
            kam.targetTexture = null; rt.Release();
            Assert.Less((orta - new Vector2(2.25f, 4f)).magnitude, 1e-3f);
            Assert.Less((nokta - oranla).magnitude, 1e-3f, "kamera çevirisi ile oran aynı sonucu verir");
        }

        // Ders 4.2 — düğmeler güvenli alanda mı (ÖRNEK güvenli alan: üstte 120, altta 60 piksel)
        [Test]
        public void Dugmeler_Guvenli_Alanda()
        {
            var guvenli = new Rect(0, 60, 1080, 1920 - 60 - 120);
            var dugmeler = Object.FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None).OrderBy(b => b.name).ToArray();
            int disarida = 0, panelle = 0; var liste = new List<object>();
            foreach (var b in dugmeler)
            {
                var rt = (RectTransform)b.transform;
                float x = rt.anchorMin.x * 1080 + rt.anchoredPosition.x - rt.pivot.x * rt.sizeDelta.x;
                float y = rt.anchorMin.y * 1920 + rt.anchoredPosition.y - rt.pivot.y * rt.sizeDelta.y;
                var tamEkran = new Rect(x, y, rt.sizeDelta.x, rt.sizeDelta.y);                 // panel bütün ekranı kaplarsa
                float gx = guvenli.xMin + rt.anchorMin.x * guvenli.width + rt.anchoredPosition.x - rt.pivot.x * rt.sizeDelta.x;
                float gy = guvenli.yMin + rt.anchorMin.y * guvenli.height + rt.anchoredPosition.y - rt.pivot.y * rt.sizeDelta.y;
                var panelde = new Rect(gx, gy, rt.sizeDelta.x, rt.sizeDelta.y);                // panel güvenli alana oturunca
                bool ic1 = DokunmaAlani.GuvenliAlanda(tamEkran, guvenli), ic2 = DokunmaAlani.GuvenliAlanda(panelde, guvenli);
                if (!ic1) disarida++; if (!ic2) panelle++;
                liste.Add(new Dictionary<string, object> { ["ad"] = b.name, ["genislik"] = rt.sizeDelta.x, ["yukseklik"] = rt.sizeDelta.y, ["alt_kenar_px"] = y, ["tam_ekranda_icinde"] = ic1, ["guvenli_panelde_icinde"] = ic2 });
                Assert.GreaterOrEqual(rt.sizeDelta.y, DokunmaAlani.Piksel(48f, 480f), $"{b.name} en az 48 dp olmalı (480 dpi'de 144 piksel)");
            }
            Olcum.Yaz("ekran-guvenli", new Dictionary<string, object>
            {
                ["dugme"] = dugmeler.Length, ["centik_px"] = 120, ["alt_cubuk_px"] = 60, ["tam_ekranda_disarida"] = disarida, ["guvenli_panelde_disarida"] = panelle, ["dugmeler"] = liste,
                ["guvenli_alan_bileseni"] = Object.FindFirstObjectByType<GuvenliAlan>() != null,
            });
            Assert.AreEqual(0, panelle, "GuvenliAlan paneliyle bütün düğmeler içeride olmalı");
        }

        // Ders 4.3 — derleme ayarları
        [Test]
        public void Derleme_Ayarlari_Temiz()
        {
            var a = new DerlemeAyarlari
            {
                sahneler = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                gelistirmeSurumu = EditorUserBuildSettings.development,
                dikey = PlayerSettings.defaultInterfaceOrientation == UIOrientation.Portrait,
                hedefKare = Object.FindFirstObjectByType<OyunYoneticisi>().hedefKare,
                sikistirma = PlayerSettings.WebGL.compressionFormat == WebGLCompressionFormat.Disabled ? "Kapalı" : PlayerSettings.WebGL.compressionFormat.ToString(),
                urunAdi = PlayerSettings.productName,
            };
            var sorun = DerlemeDenetimi.Denetle(a);
            var bozuk = a; bozuk.gelistirmeSurumu = true; bozuk.dikey = false;
            var bozukSorun = DerlemeDenetimi.Denetle(bozuk);
            Olcum.Yaz("derleme-ayar", new Dictionary<string, object>
            {
                ["alan"] = DerlemeDenetimi.AlanSayisi, ["sahneler"] = a.sahneler.ToList<object>(), ["gelistirme_surumu"] = a.gelistirmeSurumu, ["dikey"] = a.dikey,
                ["hedef_kare"] = a.hedefKare, ["sikistirma"] = a.sikistirma, ["urun_adi"] = a.urunAdi, ["sorun"] = sorun.Count,
                ["bozuk_sorun"] = bozukSorun.Count, ["bozuk_sorunlar"] = bozukSorun.ToList<object>(),
                ["unity"] = Application.unityVersion, ["web_en"] = PlayerSettings.defaultWebScreenWidth, ["web_boy"] = PlayerSettings.defaultWebScreenHeight,
            });
            Assert.AreEqual(0, sorun.Count, string.Join(", ", sorun));
            Assert.AreEqual(2, bozukSorun.Count);
        }
    }

    // Ders 3.2 — prefab: şablon değişince bütün kopyalar güncellenir
    public class PrefabTestleri
    {
        const string Gecici = "Assets/Kafe/Prefablar/Masa-test.prefab";

        [TearDown] public void Sil() { AssetDatabase.DeleteAsset(Gecici); }

        [Test]
        public void Sablon_Degisince_Kopyalar_Guncellenir()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            Assert.IsTrue(AssetDatabase.CopyAsset(SahneKurucu.PrefabYolu, Gecici));
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Gecici);
            var veri = SeviyeVerisi.Oku(System.IO.File.ReadAllText(Application.dataPath + "/Kafe/Seviyeler/seviye-1.json"));
            var kopyalar = new List<GameObject>();
            foreach (var m in veri.masalar)
            {
                var k = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                k.name = m.ad; k.transform.position = new Vector3(m.x, 0, m.y);
                kopyalar.Add(k);
            }
            float once = kopyalar[0].GetComponent<BoxCollider>().size.x;

            var icerik = PrefabUtility.LoadPrefabContents(Gecici);       // prefab'ı aç
            var s = icerik.GetComponent<MasaSablonu>();
            s.genislik = 1.2f; s.Uygula();                               // şablonu değiştir
            PrefabUtility.SaveAsPrefabAsset(icerik, Gecici);
            PrefabUtility.UnloadPrefabContents(icerik);

            int guncel = kopyalar.Count(k => Mathf.Abs(k.GetComponent<BoxCollider>().size.x - 1.2f) < 1e-4f);
            Olcum.Yaz("prefab", new Dictionary<string, object>
            {
                ["masa"] = kopyalar.Count, ["genislik"] = Olcum.Yuvarla(once, 2), ["yeni_genislik"] = 1.2, ["guncellenen"] = guncel,
                ["prefab_parca"] = prefab.GetComponentsInChildren<Transform>(true).Length,
                ["kopya_mi"] = kopyalar.All(k => PrefabUtility.IsPartOfPrefabInstance(k)),
            });
            Assert.AreEqual(kopyalar.Count, guncel, "bütün kopyalar yeni genişliği almalı");
        }
    }

    // Ders 2.1 ve 2.2 — fizik motoru sabit adımla elle ilerletilir (Physics.Simulate)
    public class FizikTestleri
    {
        SimulationMode onceki;

        [SetUp]
        public void Kur()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            onceki = Physics.simulationMode; Physics.simulationMode = SimulationMode.Script;
        }

        [TearDown] public void Geri() { Physics.simulationMode = onceki; }

        static Rigidbody Govde(string ad, Vector3 yer, float yaricap, CollisionDetectionMode kip)
        {
            var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.name = ad; g.transform.position = yer; g.transform.localScale = Vector3.one * yaricap * 2f;
            var rb = g.AddComponent<Rigidbody>();
            rb.useGravity = false; rb.linearDamping = 0f; rb.mass = 1f; rb.collisionDetectionMode = kip;
            return rb;
        }

        [Test]
        public void Sabit_Kuvvet_Iki_Saniyede_Iki_Metre()
        {
            const float dt = 0.02f; const int adim = 100;
            var rb = Govde("kutu", Vector3.zero, 0.25f, CollisionDetectionMode.Discrete);
            for (int k = 0; k < adim; k++) { rb.AddForce(Vector3.right * 1f); Physics.Simulate(dt); }   // 1 N, 1 kg → 1 m/s²
            float unity = rb.position.x;
            // karşılaştırma: aynı hesap elle, iki Euler sırasıyla
            float xa = 0, va = 0, xy = 0, vy = 0;
            for (int k = 0; k < adim; k++) { xa += va * dt; va += 1f * dt; vy += 1f * dt; xy += vy * dt; }
            Olcum.Yaz("fizik", new Dictionary<string, object>
            {
                ["adim"] = adim, ["dt_s"] = dt, ["tam_m"] = 2.0, ["unity_m"] = Olcum.Yuvarla(unity, 3), ["unity_hiz"] = Olcum.Yuvarla(rb.linearVelocity.x, 3),
                ["acik_euler_m"] = Olcum.Yuvarla(xa, 3), ["yari_kapali_euler_m"] = Olcum.Yuvarla(xy, 3),
            });
            Assert.AreEqual(2.0f, unity, 0.05f, "tam çözüm 2 m; sabit adımın hatası yüzde birkaç");
            Assert.AreEqual(xy, unity, 1e-3f, "Unity'nin fizik motoru yarı kapalı Euler gibi davranır");
        }

        [Test]
        public void Hizli_Top_Ince_Duvardan_Gecer_Surekli_Denetim_Yakalar()
        {
            const float dt = 0.02f, yaricap = 0.05f, duvar = 0.02f;
            var o = new Dictionary<string, object> { ["duvar_m"] = duvar, ["yaricap_m"] = yaricap, ["dt_s"] = dt };
            foreach (float hiz in new[] { 1f, 10f })
                foreach (var kip in new[] { CollisionDetectionMode.Discrete, CollisionDetectionMode.ContinuousDynamic })
                {
                    EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                    var d = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    d.name = "duvar"; d.transform.localScale = new Vector3(duvar, 2f, 2f);
                    var top = Govde("top", new Vector3(-1.1f, 0f, 0f), yaricap, kip);
                    top.linearVelocity = Vector3.right * hiz;
                    Physics.SyncTransforms();
                    for (int k = 0; k < 100; k++) Physics.Simulate(dt);
                    bool gecti = top.position.x > 0.1f;
                    string ad = $"hiz{hiz:0}_{(kip == CollisionDetectionMode.Discrete ? "ayrik" : "surekli")}";
                    o[ad + "_gecti"] = gecti; o[ad + "_son_x"] = Olcum.Yuvarla(top.position.x, 2);
                    o[$"hiz{hiz:0}_adim_m"] = Olcum.Yuvarla(hiz * dt, 2);
                }
            Olcum.Yaz("carpisma", o);
            Assert.IsFalse((bool)o["hiz1_ayrik_gecti"], "yavaş top ayrık denetimde de durur");
            Assert.IsTrue((bool)o["hiz10_ayrik_gecti"], "hızlı top ayrık denetimde duvardan geçer (tünel etkisi)");
            Assert.IsFalse((bool)o["hiz10_surekli_gecti"], "sürekli denetim hızlı topu yakalar");
        }
    }

    // Ders 2.3 — benzetim kaydı
    public class KayitTestleri
    {
        [Test]
        public void Benzetim_Kaydi_Esigin_Altinda()
        {
            var csv = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Kafe/Veri/benzetim_kaydi.csv").text;
            EditorSceneManager.OpenScene(SahneKurucu.SahneYolu, OpenSceneMode.Single);
            float esik = Object.FindFirstObjectByType<Bardak>().esik;
            var r = KayitAnalizi.Coz(csv, esik);
            // hareketin başladığı ve bittiği an: hız en büyüğün onda birini ilk ve son aştığı örnek
            int ilk = System.Array.FindIndex(r.hiz, h => h > r.enBuyukHiz * 0.1), son = System.Array.FindLastIndex(r.hiz, h => h > r.enBuyukHiz * 0.1);
            Olcum.Yaz("kayit", new Dictionary<string, object>
            {
                ["ornek"] = r.ornek, ["sure_s"] = Olcum.Yuvarla(r.sure, 1), ["yol_m"] = Olcum.Yuvarla(r.yol, 2), ["en_buyuk_hiz"] = Olcum.Yuvarla(r.enBuyukHiz, 2),
                ["en_buyuk_ivme"] = Olcum.Yuvarla(r.enBuyukIvme, 2), ["esik"] = esik, ["esik_ustu"] = r.esikUstu,
                ["hareket_basi_s"] = Olcum.Yuvarla(r.zaman[ilk], 1), ["hareket_sonu_s"] = Olcum.Yuvarla(r.zaman[son], 1), ["ornekleme_hz"] = 50,
            });
            System.IO.File.WriteAllLines(System.IO.Path.Combine(Olcum.Klasor, "kayit_ivme.csv"),
                Enumerable.Range(0, r.ornek).Where(i => i % 5 == 0).Select(i => System.FormattableString.Invariant($"{r.zaman[i]:0.00},{r.hiz[i]:0.0000},{r.ivme[i]:0.0000}")));
            Assert.AreEqual(0, r.esikUstu, "kayıtta bardak eşiği hiç aşmıyor");
            Assert.Less(r.enBuyukIvme, esik);
        }
    }
}

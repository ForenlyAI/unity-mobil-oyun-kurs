// Ders ekranları için Play modunda Unity'nin çizdiği kareleri PNG olarak kaydeder (Olcumler/gorsel).
// [Explicit]: olağan test koşumuna girmez; yalnız adıyla istenince koşar:
//   unity test . --mode PlayMode --filter "Kafe.Testler.GorselKayit"
using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Kafe.Testler
{
    [Explicit, Category("Gorsel")]
    public class GorselKayit
    {
        OyunYoneticisi oyun;
        Camera yanKamera;

        static string Klasor(string ad) { var k = Path.Combine(Olcum.Klasor, "gorsel", ad); Directory.CreateDirectory(k); return k; }

        [TearDown] public void Temizle() { Time.captureFramerate = 0; }

        static void Kaydet(Camera kam, int w, int h, string yol)
        {
            var rt = RenderTexture.GetTemporary(w, h, 24, RenderTextureFormat.ARGB32);
            var onceki = kam.targetTexture;
            kam.targetTexture = rt; kam.Render();
            RenderTexture.active = rt;
            var t = new Texture2D(w, h, TextureFormat.RGB24, false);
            t.ReadPixels(new Rect(0, 0, w, h), 0, 0); t.Apply();
            RenderTexture.active = null; kam.targetTexture = onceki;
            File.WriteAllBytes(yol, t.EncodeToPNG());
            Object.Destroy(t); RenderTexture.ReleaseTemporary(rt);
        }

        IEnumerator SahneyiAc(int fps = 60)
        {
            SceneManager.LoadScene("Kafe", LoadSceneMode.Single);
            yield return null; yield return null;
            oyun = Object.FindFirstObjectByType<OyunYoneticisi>();
            Application.targetFrameRate = -1; QualitySettings.vSyncCount = 0;
            Time.captureFramerate = fps;
            // arayüz kameraya bağlanır ki kaydedilen karede yazılar da görünsün
            var tuval = Object.FindFirstObjectByType<Canvas>();
            tuval.renderMode = RenderMode.ScreenSpaceCamera; tuval.worldCamera = oyun.kamera; tuval.planeDistance = 1f;
            var g = new GameObject("YanKamera");
            yanKamera = g.AddComponent<Camera>();
            yanKamera.fieldOfView = 42f; yanKamera.clearFlags = CameraClearFlags.SolidColor; yanKamera.backgroundColor = new Color(0.10f, 0.11f, 0.14f);
            yanKamera.enabled = false;
            yield return null;
        }

        void YanKamerayiKoy(Vector3 bakilan, float uzaklik = 4.2f)
        {
            yanKamera.transform.position = bakilan + new Vector3(-uzaklik * 0.45f, uzaklik * 0.62f, -uzaklik * 0.75f);
            yanKamera.transform.LookAt(bakilan + Vector3.up * 0.7f);
        }

        IEnumerator Klip(string ad, string kayit, float azamiIvme, int her, float sondaBekle = 1.0f)
        {
            yield return SahneyiAc();
            string k = Klasor(ad);
            oyun.robot.azamiIvme = azamiIvme;
            oyun.KaydiOynat(DokunusListesi.Oku(File.ReadAllText(Path.Combine(Application.dataPath, "Kafe/Seviyeler", kayit + ".json"))));
            int kare = 0, yazilan = 0, sonKareler = -1;
            while (true)
            {
                if (kare % her == 0)
                {
                    var r = oyun.robot.transform.position;
                    YanKamerayiKoy(new Vector3(Mathf.Lerp(r.x, 2.25f, 0.5f), 0f, r.z + 0.6f));
                    Kaydet(oyun.kamera, 608, 1080, Path.Combine(k, $"ust_{yazilan:0000}.png"));
                    Kaydet(yanKamera, 1312, 1080, Path.Combine(k, $"yan_{yazilan:0000}.png"));
                    yazilan++;
                }
                kare++;
                yield return null;
                if (oyun.Bitti && sonKareler < 0) sonKareler = Mathf.RoundToInt(sondaBekle * 60f);
                if (sonKareler >= 0 && sonKareler-- == 0) break;
                if (kare > 4000) break;
            }
            File.WriteAllText(Path.Combine(k, "bilgi.txt"), $"{yazilan} kare, her {her} oyun karesinde bir, 60 kare/sn oyun; sonuc {oyun.Durum.Simdi} sure {oyun.Sure:0.00} puan {oyun.PuanDegeri}\n");
        }

        [UnityTest, Timeout(1800000)] public IEnumerator Klip_Ornek_Oyun() { yield return Klip("oyun-ornek", "ornek-dokunuslar", 1.0f, 3); }
        [UnityTest, Timeout(1800000)] public IEnumerator Klip_Dogrudan_Carpma() { yield return Klip("oyun-dogrudan", "dogrudan-dokunus", 1.0f, 1, 1.5f); }
        [UnityTest, Timeout(1800000)] public IEnumerator Klip_Sinirsiz_Ivme() { yield return Klip("oyun-sinirsiz", "ornek-dokunuslar", 0f, 1, 1.0f); }

        [UnityTest, Timeout(1800000)]
        public IEnumerator Durgun_Kareler()
        {
            yield return SahneyiAc();
            string k = Klasor("durgun");
            // sahnenin bütünü, üstten ve yandan
            yanKamera.fieldOfView = 50f;
            yanKamera.transform.position = new Vector3(-1.6f, 6.2f, -3.4f); yanKamera.transform.LookAt(new Vector3(2.4f, 0f, 3.6f));
            Kaydet(yanKamera, 1920, 1080, Path.Combine(k, "sahne-yandan.png"));
            Kaydet(oyun.kamera, 1080, 1920, Path.Combine(k, "oyun-ekrani-menu.png"));
            // robot yakından
            var r = oyun.robot.transform.position;
            yanKamera.fieldOfView = 35f;
            yanKamera.transform.position = r + new Vector3(1.5f, 1.7f, 2.2f); yanKamera.transform.LookAt(r + Vector3.up * 0.95f);
            Kaydet(yanKamera, 1920, 1080, Path.Combine(k, "robot-yakin.png"));
            // prefab: masa genişliği 1,2 m olunca (yalnız görüntü için, çalışırken)
            foreach (var m in Object.FindObjectsByType<MasaSablonu>(FindObjectsSortMode.None)) { m.genislik = 1.2f; m.Uygula(); }
            yield return null;
            yanKamera.fieldOfView = 50f;
            yanKamera.transform.position = new Vector3(-1.6f, 6.2f, -3.4f); yanKamera.transform.LookAt(new Vector3(2.4f, 0f, 3.6f));
            Kaydet(yanKamera, 1920, 1080, Path.Combine(k, "masalar-genis.png"));
        }

        [UnityTest, Timeout(1800000)]
        public IEnumerator Klip_Tunel_Etkisi()
        {
            SceneManager.LoadScene("Kafe", LoadSceneMode.Single);
            yield return null; yield return null;
            foreach (var kok in SceneManager.GetActiveScene().GetRootGameObjects())
                if (kok.GetComponent<Camera>() == null && kok.GetComponent<Light>() == null) Object.Destroy(kok);
            Application.targetFrameRate = -1; QualitySettings.vSyncCount = 0; Time.captureFramerate = 50;
            yield return null;
            string k = Klasor("tunel");
            var kam = Camera.main; kam.orthographic = true; kam.orthographicSize = 0.75f;
            kam.transform.position = new Vector3(0f, 5f, 0f); kam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            var zemin = GameObject.CreatePrimitive(PrimitiveType.Plane); Object.Destroy(zemin.GetComponent<Collider>());
            zemin.transform.position = new Vector3(0, -0.06f, 0);
            var duvar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            duvar.transform.localScale = new Vector3(0.02f, 0.3f, 1.4f);
            duvar.GetComponent<Renderer>().material.color = new Color(0.2f, 0.25f, 0.3f);
            Rigidbody Top(float z, CollisionDetectionMode kip, Color renk)
            {
                var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                g.transform.position = new Vector3(-1.1f, 0f, z); g.transform.localScale = Vector3.one * 0.1f;
                g.GetComponent<Renderer>().material.color = renk;
                var rb = g.AddComponent<Rigidbody>(); rb.useGravity = false; rb.linearDamping = 0f; rb.collisionDetectionMode = kip;
                return rb;
            }
            var ayrik = Top(0.3f, CollisionDetectionMode.Discrete, new Color(0.85f, 0.2f, 0.2f));
            var surekli = Top(-0.3f, CollisionDetectionMode.ContinuousDynamic, new Color(0.2f, 0.65f, 0.3f));
            Physics.SyncTransforms();
            yield return new WaitForFixedUpdate();
            int n = 0;
            for (int i = 0; i < 10; i++) { Kaydet(kam, 1920, 1080, Path.Combine(k, $"f_{n++:0000}.png")); }
            ayrik.linearVelocity = Vector3.right * 10f; surekli.linearVelocity = Vector3.right * 10f;
            for (int i = 0; i < 14; i++)
            {
                yield return new WaitForFixedUpdate();
                Kaydet(kam, 1920, 1080, Path.Combine(k, $"f_{n++:0000}.png"));
            }
            for (int i = 0; i < 20; i++) { Kaydet(kam, 1920, 1080, Path.Combine(k, $"f_{n++:0000}.png")); }
            File.WriteAllText(Path.Combine(k, "bilgi.txt"), $"kirmizi top ayrik denetim son x {ayrik.position.x:0.00}; yesil top surekli denetim son x {surekli.position.x:0.00}; 10 m/s, adim 0.02 s\n");
        }

        [UnityTest, Timeout(1800000)]
        public IEnumerator Durgun_Yuz_Robot()
        {
            yield return SahneyiAc();
            string k = Klasor("durgun");
            oyun.robot.GetComponent<Collider>().enabled = false;
            for (int i = 1; i < 100; i++)
                Object.Instantiate(oyun.robot.gameObject, new Vector3(0.4f + i % 10 * 0.4f, 0f, 1.0f + i / 10 * 0.6f), Quaternion.identity);
            yield return null;
            foreach (var r in Object.FindObjectsByType<RobotSurucu>(FindObjectsSortMode.None)) r.HedefVer(new Vector2(4.0f, 7.5f));
            for (int i = 0; i < 90; i++) yield return null;
            yanKamera.fieldOfView = 50f;
            yanKamera.transform.position = new Vector3(-1.6f, 6.2f, -3.4f); yanKamera.transform.LookAt(new Vector3(2.4f, 0f, 3.6f));
            Kaydet(yanKamera, 1920, 1080, Path.Combine(k, "yuz-robot.png"));
        }
    }
}

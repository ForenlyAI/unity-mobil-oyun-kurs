// Ders 1.2 ve 2.1 — Update, Time.deltaTime ve FixedUpdate gerçek oyun döngüsünde ölçülür.
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Kafe.Testler
{
    public class KareTestleri
    {
        [TearDown] public void Temizle() { Time.captureFramerate = 0; }

        [UnityTest]
        public IEnumerator DeltaTime_Ile_Yol_Kare_Hizindan_Bagimsiz()
        {
            var o = new Dictionary<string, object>();
            foreach (int fps in new[] { 30, 60 })
            {
                Application.targetFrameRate = -1; QualitySettings.vSyncCount = 0;
                Time.captureFramerate = fps;      // her kare tam 1/fps saniye sayılır
                yield return null;
                var dogru = new GameObject("dogru").AddComponent<DeltaIleHareket>();
                var yanlis = new GameObject("yanlis").AddComponent<KareBasinaHareket>();
                float dt = 0f;
                while (dogru.KareSayisi < fps * 2) { yield return null; dt = Time.deltaTime; }
                o[$"fps{fps}_kare"] = dogru.KareSayisi;
                o[$"fps{fps}_dt_ms"] = Olcum.Yuvarla(dt * 1000.0, 2);
                o[$"fps{fps}_dogru_m"] = Olcum.Yuvarla(dogru.transform.position.x, 2);
                o[$"fps{fps}_yanlis_m"] = Olcum.Yuvarla(yanlis.transform.position.x, 2);
                Assert.AreEqual(1.0f, dogru.transform.position.x, 0.01f, "deltaTime ile iki saniyede bir metre");
                Object.Destroy(dogru.gameObject); Object.Destroy(yanlis.gameObject);
            }
            Assert.AreNotEqual(o["fps30_yanlis_m"], o["fps60_yanlis_m"], "kare başına sabit adım kare hızına bağlı kalır");
            Olcum.Yaz("kare", o);
        }

        [UnityTest]
        public IEnumerator FixedUpdate_Sayisi_Kare_Hizindan_Bagimsiz()
        {
            var o = new Dictionary<string, object> { ["sabit_adim_s"] = Olcum.Yuvarla(Time.fixedDeltaTime, 3) };
            var sabit = new List<int>();
            foreach (int fps in new[] { 30, 60, 120 })
            {
                Application.targetFrameRate = -1; QualitySettings.vSyncCount = 0;
                Time.captureFramerate = fps;
                yield return null;
                var s = new GameObject("sayac").AddComponent<SayacBileseni>();
                while (s.UpdateSayisi < fps * 2) yield return null;
                o[$"fps{fps}_update"] = s.UpdateSayisi; o[$"fps{fps}_fixed"] = s.FixedUpdateSayisi;
                sabit.Add(s.FixedUpdateSayisi);
                Object.Destroy(s.gameObject);
            }
            foreach (int f in sabit) Assert.That(f, Is.InRange(99, 101), "iki saniyede yaklaşık yüz sabit adım");
            Olcum.Yaz("sabit-adim", o);
        }
    }
}

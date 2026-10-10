// Ders 3.1–3.3, 4.4 · Oyunun yöneticisi: durum makinesi, puan, süre ve kaydedilmiş dokunuşları yeniden oynatma.
// Teslim ve dökülme kararı FixedUpdate'te verilir: sonuç kare hızından bağımsız olur.
using System.Collections.Generic;
using UnityEngine;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Kafe
{
    [DefaultExecutionOrder(100)]
    public class OyunYoneticisi : MonoBehaviour
    {
        public Camera kamera;
        public RobotSurucu robot;
        public Bardak bardak;
        public SeviyeKurucu seviye;
        public OyunArayuzu arayuz;
        [Tooltip("Kaydedilmiş dokunuşlar (Assets/Kafe/Seviyeler)")] public TextAsset dokunusDosyasi;
        [Tooltip("Açılışta kaydı kendiliğinden oynat")] public bool otomatikOyna;
        [Tooltip("s — süre dolunca oyun biter")] public float azamiSure = 40f;
        [Tooltip("m — teslim yarıçapı")] public float teslimYaricapi = 0.3f;
        [Tooltip("Hedef kare hızı (telefonda varsayılan 30)")] public int hedefKare = 60;

        public readonly DurumMakinesi Durum = new DurumMakinesi();
        public float Sure { get; private set; }
        public int Kare { get; private set; }
        public int SabitAdim { get; private set; }
        public int PuanDegeri { get; private set; }
        public bool Bitti { get; private set; }

        List<KayitliDokunus> oynatilan;
        int sira;

        void Awake() { Application.targetFrameRate = hedefKare; }

        void Start()
        {
            seviye.Kur();
            robot.Sifirla(seviye.Veri.baslangic);
            bardak.Sifirla();
            if (arayuz) arayuz.Goster(this);
            if (otomatikOyna && dokunusDosyasi) KaydiOynat(DokunusListesi.Oku(dokunusDosyasi.text));
        }

        public void Basla()
        {
            if (Durum.Simdi != OyunDurumu.Menu) Durum.Gec(OyunDurumu.Menu);
            robot.Sifirla(seviye.Veri.baslangic);
            bardak.Sifirla();
            Sure = 0; Kare = 0; SabitAdim = 0; PuanDegeri = 0; Bitti = false; sira = 0;
            Durum.Gec(OyunDurumu.Oyun);
            Dagit();
            if (arayuz) arayuz.Goster(this);
        }

        public void KaydiOynat(DokunusListesi liste)
        {
            oynatilan = new List<KayitliDokunus>(liste.dokunuslar);
            Basla();
        }

        // Ekrandan gelen dokunuş: menüdeyse oyunu başlatır, oyundaysa robota hedef verir.
        public void Dokun(Vector2 ekranPikseli, TouchPhase faz)
        {
            if (Durum.Simdi == OyunDurumu.Menu) { if (faz == TouchPhase.Began) { oynatilan = null; Basla(); } else return; }
            if (Durum.Simdi != OyunDurumu.Oyun) return;
            if (faz == TouchPhase.Began || faz == TouchPhase.Moved) robot.HedefVer(EkranDunya.Kamerayla(kamera, ekranPikseli));
            else if (faz == TouchPhase.Canceled) robot.HedefVer(null);
            // Ended / Stationary: robot son hedefe gitmeyi sürdürür
        }

        void Dagit()
        {
            if (oynatilan == null) return;
            while (sira < oynatilan.Count && oynatilan[sira].t <= Sure + 1e-4f)
            {
                var d = oynatilan[sira++];
                var faz = (TouchPhase)System.Enum.Parse(typeof(TouchPhase), d.faz);
                // kayıt 1080 × 1920 referans ekranında: kamera yerine oranla çevrilir (her ekranda aynı sonuç)
                var zemin = EkranDunya.Dunyaya(new Vector2(d.px, d.py), EkranDunya.ReferansEkran);
                if (faz == TouchPhase.Began || faz == TouchPhase.Moved) robot.HedefVer(zemin);
                else if (faz == TouchPhase.Canceled) robot.HedefVer(null);
            }
        }

        void FixedUpdate()
        {
            if (Durum.Simdi != OyunDurumu.Oyun) return;
            SabitAdim++;
            Sure = SabitAdim * Time.fixedDeltaTime;
            if (bardak.Dokuldu) Bitir(OyunDurumu.Dokuldu);
            else if ((robot.Konum - seviye.Veri.teslim).magnitude < teslimYaricapi && robot.Hiz.magnitude < 0.05f) Bitir(OyunDurumu.Teslim);
            else if (Sure >= azamiSure) Bitir(OyunDurumu.Menu);
            else Dagit();
        }

        void Update()
        {
            if (Durum.Simdi == OyunDurumu.Oyun) Kare++;
            if (arayuz) arayuz.Goster(this);
        }

        void Bitir(OyunDurumu son)
        {
            Durum.Gec(son);
            robot.HedefVer(null);
            PuanDegeri = Puan.Hesapla(son == OyunDurumu.Teslim, son == OyunDurumu.Dokuldu, Sure, robot.Carpma);
            Bitti = true;
            Debug.Log($"[Kafe] sonuç: {son} · süre {Sure:0.00} s · {Kare} kare · {SabitAdim} sabit adım · çarpma {robot.Carpma} · en büyük ivme {robot.EnBuyukIvme:0.00} m/s² · puan {PuanDegeri}");
        }

        public void BaslaDugmesi() { oynatilan = null; Basla(); }

        public void YenidenOynatDugmesi()
        {
            if (dokunusDosyasi) KaydiOynat(DokunusListesi.Oku(dokunusDosyasi.text));
        }
    }
}

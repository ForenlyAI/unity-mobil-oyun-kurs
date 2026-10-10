// Kafe sahnesini, masa prefab'ını ve malzemeleri baştan kurar. Menü: Kafe > Sahneyi Kur
// Komut satırından: unity command eval "Kafe.EditorAraclari.SahneKurucu.Kur();"
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Kafe.EditorAraclari
{
    public static class SahneKurucu
    {
        public const string Kok = "Assets/Kafe";
        public const string SahneYolu = Kok + "/Sahneler/Kafe.unity";
        public const string PrefabYolu = Kok + "/Prefablar/Masa.prefab";

        static Material Malzeme(string ad, Color renk, float puruz = 0.5f)
        {
            string yol = $"{Kok}/Malzemeler/{ad}.mat";
            var m = AssetDatabase.LoadAssetAtPath<Material>(yol);
            if (m == null)
            {
                m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(m, yol);
            }
            m.SetColor("_BaseColor", renk);
            m.SetFloat("_Smoothness", puruz);
            EditorUtility.SetDirty(m);
            return m;
        }

        static GameObject Parca(PrimitiveType tur, string ad, Transform ust, Vector3 yer, Vector3 olcek, Material m, bool carpisma = false)
        {
            var g = GameObject.CreatePrimitive(tur);
            g.name = ad;
            g.transform.SetParent(ust, false);
            g.transform.localPosition = yer;
            g.transform.localScale = olcek;
            g.GetComponent<Renderer>().sharedMaterial = m;
            if (!carpisma) Object.DestroyImmediate(g.GetComponent<Collider>());
            return g;
        }

        public static MasaSablonu MasaPrefabiKur(Material masa, Material ayak)
        {
            var kok = new GameObject("Masa");
            kok.AddComponent<BoxCollider>();
            var s = kok.AddComponent<MasaSablonu>();
            s.tabla = Parca(PrimitiveType.Cube, "Tabla", kok.transform, Vector3.zero, Vector3.one, masa).transform;
            Parca(PrimitiveType.Cylinder, "Ayak", kok.transform, new Vector3(0, 0.36f, 0), new Vector3(0.12f, 0.36f, 0.12f), ayak);
            s.Uygula();
            var p = PrefabUtility.SaveAsPrefabAsset(kok, PrefabYolu);
            Object.DestroyImmediate(kok);
            return p.GetComponent<MasaSablonu>();
        }

        static Text Yazi(string ad, Transform ust, Vector2 capaMin, Vector2 capaMax, Vector2 konum, Vector2 boyut, int punto, TextAnchor hiza)
        {
            var g = new GameObject(ad, typeof(RectTransform));
            g.transform.SetParent(ust, false);
            var rt = (RectTransform)g.transform;
            rt.anchorMin = capaMin; rt.anchorMax = capaMax; rt.anchoredPosition = konum; rt.sizeDelta = boyut;
            var t = g.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = punto; t.alignment = hiza; t.color = Color.white; t.raycastTarget = false;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            var golge = g.AddComponent<Outline>(); golge.effectColor = new Color(0, 0, 0, 0.8f); golge.effectDistance = new Vector2(2, -2);
            return t;
        }

        static Button Dugme(string ad, string yazi, Transform ust, Vector2 capa, Vector2 konum, Vector2 boyut)
        {
            var g = new GameObject(ad, typeof(RectTransform));
            g.transform.SetParent(ust, false);
            var rt = (RectTransform)g.transform;
            rt.anchorMin = rt.anchorMax = capa; rt.pivot = capa; rt.anchoredPosition = konum; rt.sizeDelta = boyut;
            var im = g.AddComponent<Image>(); im.color = new Color(0.11f, 0.13f, 0.18f, 0.85f);
            var b = g.AddComponent<Button>();
            var t = Yazi("Yazi", g.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 44, TextAnchor.MiddleCenter);
            t.text = yazi;
            return b;
        }

        [MenuItem("Kafe/Sahneyi Kur")]
        public static void Kur()
        {
            foreach (var k in new[] { "Malzemeler", "Prefablar", "Sahneler" }) Directory.CreateDirectory($"{Kok}/{k}");
            foreach (var eski in new[] { "Assets/TutorialInfo", "Assets/Readme.asset", "Assets/Scenes" })
                if (AssetDatabase.IsValidFolder(eski) || File.Exists(eski)) AssetDatabase.DeleteAsset(eski);
            AssetDatabase.Refresh();

            var sahne = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var mZemin = Malzeme("zemin", new Color(0.78f, 0.66f, 0.50f), 0.2f);
            var mDuvar = Malzeme("duvar", new Color(0.90f, 0.88f, 0.84f), 0.1f);
            var mMasa = Malzeme("masa", new Color(0.42f, 0.27f, 0.16f), 0.4f);
            var mAyak = Malzeme("masa-ayak", new Color(0.15f, 0.15f, 0.17f), 0.6f);
            var mTezgah = Malzeme("tezgah", new Color(0.22f, 0.30f, 0.36f), 0.5f);
            var mGovde = Malzeme("robot-govde", new Color(0.93f, 0.94f, 0.96f), 0.7f);
            var mEklem = Malzeme("robot-eklem", new Color(0.18f, 0.20f, 0.24f), 0.6f);
            var mTepsi = Malzeme("tepsi", new Color(0.70f, 0.72f, 0.75f), 0.85f);
            var mBardak = Malzeme("bardak", new Color(0.98f, 0.98f, 1.0f), 0.9f);
            var mCay = Malzeme("cay", new Color(0.55f, 0.20f, 0.08f), 0.8f);
            var mTeslim = Malzeme("teslim", new Color(0.20f, 0.72f, 0.38f), 0.3f);
            var mHedef = Malzeme("hedef", new Color(0.98f, 0.80f, 0.15f), 0.3f);

            var masaPrefab = MasaPrefabiKur(mMasa, mAyak);
            var seviyeDosyasi = AssetDatabase.LoadAssetAtPath<TextAsset>($"{Kok}/Seviyeler/seviye-1.json");
            var dokunusDosyasi = AssetDatabase.LoadAssetAtPath<TextAsset>($"{Kok}/Seviyeler/ornek-dokunuslar.json");
            var veri = SeviyeVerisi.Oku(seviyeDosyasi.text);

            // Kamera: yukarıdan dik bakar, 4,5 × 8,0 m'lik alanı gösterir (dikey 9:16 ekranda)
            var kam = Camera.main;
            kam.orthographic = true; kam.orthographicSize = EkranDunya.KafeBoyu.y / 2f;
            kam.transform.position = new Vector3(EkranDunya.KafeBoyu.x / 2f, 10f, EkranDunya.KafeBoyu.y / 2f);
            kam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            kam.clearFlags = CameraClearFlags.SolidColor; kam.backgroundColor = new Color(0.10f, 0.11f, 0.14f);
            kam.nearClipPlane = 0.1f; kam.farClipPlane = 30f;
            var isik = Object.FindFirstObjectByType<Light>();
            isik.transform.rotation = Quaternion.Euler(55f, -35f, 0f); isik.intensity = 1.2f;

            var kafe = new GameObject("Kafe").transform;
            var zemin = Parca(PrimitiveType.Plane, "Zemin", kafe, new Vector3(2.25f, 0f, 4f), new Vector3(0.45f, 1f, 0.8f), mZemin);
            Parca(PrimitiveType.Cube, "Duvar-sol", kafe, new Vector3(-0.05f, 0.5f, 4f), new Vector3(0.1f, 1f, 8.2f), mDuvar, true);
            Parca(PrimitiveType.Cube, "Duvar-sag", kafe, new Vector3(4.55f, 0.5f, 4f), new Vector3(0.1f, 1f, 8.2f), mDuvar, true);
            Parca(PrimitiveType.Cube, "Duvar-alt", kafe, new Vector3(2.25f, 0.5f, -0.05f), new Vector3(4.5f, 1f, 0.1f), mDuvar, true);
            Parca(PrimitiveType.Cube, "Duvar-ust", kafe, new Vector3(2.25f, 0.5f, 8.05f), new Vector3(4.5f, 1f, 0.1f), mDuvar, true);
            Parca(PrimitiveType.Cube, "Tezgah", kafe, new Vector3(0.9f, 0.45f, 0.3f), new Vector3(1.6f, 0.9f, 0.5f), mTezgah, true);

            var masalar = new GameObject("Masalar").transform;
            foreach (var m in veri.masalar)
            {
                var k = (GameObject)PrefabUtility.InstantiatePrefab(masaPrefab.gameObject, masalar);
                k.name = m.ad; k.transform.position = new Vector3(m.x, 0f, m.y);
            }

            var teslim = Parca(PrimitiveType.Cylinder, "TeslimNoktasi", null, new Vector3(veri.teslim.x, 0.01f, veri.teslim.y), new Vector3(0.6f, 0.005f, 0.6f), mTeslim);
            var hedef = Parca(PrimitiveType.Cylinder, "HedefIsareti", null, new Vector3(veri.baslangic.x, 0.02f, veri.baslangic.y), new Vector3(0.2f, 0.005f, 0.2f), mHedef);

            // Robot: insansı gövdenin basit parçalarla temsili (yalnız görüntü); fizik kökteki kapsülde
            var robot = new GameObject("Robot");
            robot.transform.position = new Vector3(veri.baslangic.x, 0f, veri.baslangic.y);
            var rb = robot.AddComponent<Rigidbody>();
            rb.mass = 30f; rb.useGravity = false; rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            var kapsul = robot.AddComponent<CapsuleCollider>();
            kapsul.radius = 0.25f; kapsul.height = 1.6f; kapsul.center = new Vector3(0f, 0.8f, 0f);
            var surucu = robot.AddComponent<RobotSurucu>();
            var bardak = robot.AddComponent<Bardak>();
            var gorsel = new GameObject("Gorsel").transform; gorsel.SetParent(robot.transform, false);
            Parca(PrimitiveType.Cylinder, "Bacak-sol", gorsel, new Vector3(-0.1f, 0.4f, 0f), new Vector3(0.12f, 0.4f, 0.12f), mEklem);
            Parca(PrimitiveType.Cylinder, "Bacak-sag", gorsel, new Vector3(0.1f, 0.4f, 0f), new Vector3(0.12f, 0.4f, 0.12f), mEklem);
            Parca(PrimitiveType.Cube, "Govde", gorsel, new Vector3(0f, 1.08f, 0f), new Vector3(0.42f, 0.56f, 0.24f), mGovde);
            Parca(PrimitiveType.Sphere, "Bas", gorsel, new Vector3(0f, 1.52f, 0f), new Vector3(0.24f, 0.24f, 0.24f), mGovde);
            Parca(PrimitiveType.Cube, "Kol-sol", gorsel, new Vector3(-0.2f, 1.0f, 0.2f), new Vector3(0.08f, 0.08f, 0.42f), mEklem);
            Parca(PrimitiveType.Cube, "Kol-sag", gorsel, new Vector3(0.2f, 1.0f, 0.2f), new Vector3(0.08f, 0.08f, 0.42f), mEklem);
            var tepsi = new GameObject("Tepsi").transform; tepsi.SetParent(gorsel, false); tepsi.localPosition = new Vector3(0f, 1.05f, 0.42f);
            Parca(PrimitiveType.Cube, "Tepsi-yuzey", tepsi, Vector3.zero, new Vector3(0.44f, 0.02f, 0.3f), mTepsi);
            var bardakG = Parca(PrimitiveType.Cylinder, "Bardak", tepsi, new Vector3(0f, 0.07f, 0f), new Vector3(0.09f, 0.06f, 0.09f), mBardak);
            Parca(PrimitiveType.Cylinder, "Cay", bardakG.transform, new Vector3(0f, 0.9f, 0f), new Vector3(0.85f, 0.12f, 0.85f), mCay);
            var dokulen = Parca(PrimitiveType.Cylinder, "DokulenCay", gorsel, new Vector3(0f, 0.012f, 0.45f), new Vector3(0.55f, 0.004f, 0.55f), mCay);
            dokulen.SetActive(false);
            surucu.gorsel = gorsel;
            bardak.robot = surucu; bardak.bardakGorseli = bardakG.transform; bardak.dokulenCay = dokulen;
            hedef.AddComponent<HedefIsareti>().robot = surucu;
            hedef.GetComponent<HedefIsareti>().gorsel = hedef.GetComponent<Renderer>();

            // Arayüz
            var tuval = new GameObject("Arayuz", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            tuval.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var olcek = tuval.GetComponent<CanvasScaler>();
            olcek.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; olcek.referenceResolution = EkranDunya.ReferansEkran; olcek.matchWidthOrHeight = 0.5f;
            var guvenli = new GameObject("GuvenliAlan", typeof(RectTransform), typeof(GuvenliAlan));
            guvenli.transform.SetParent(tuval.transform, false);
            var grt = (RectTransform)guvenli.transform; grt.anchorMin = Vector2.zero; grt.anchorMax = Vector2.one; grt.offsetMin = grt.offsetMax = Vector2.zero;
            var arayuz = tuval.AddComponent<OyunArayuzu>();
            arayuz.ustYazi = Yazi("UstYazi", guvenli.transform, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -70), new Vector2(0, 80), 46, TextAnchor.MiddleCenter);
            arayuz.ortaYazi = Yazi("OrtaYazi", guvenli.transform, new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(0, 520), new Vector2(0, 100), 60, TextAnchor.MiddleCenter);
            arayuz.basla = Dugme("Dugme-Oyna", "Oyna", guvenli.transform, new Vector2(0, 0), new Vector2(40, 40), new Vector2(300, 144));
            arayuz.tekrar = Dugme("Dugme-KaydiOynat", "Kaydı oynat", guvenli.transform, new Vector2(1, 0), new Vector2(-40, 40), new Vector2(360, 144));
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));

            var yon = new GameObject("OyunYoneticisi");
            var sk = yon.AddComponent<SeviyeKurucu>();
            sk.seviyeDosyasi = seviyeDosyasi; sk.masaPrefab = masaPrefab; sk.masalarKoku = masalar; sk.teslimNoktasi = teslim.transform;
            var oy = yon.AddComponent<OyunYoneticisi>();
            oy.kamera = kam; oy.robot = surucu; oy.bardak = bardak; oy.seviye = sk; oy.arayuz = arayuz; oy.dokunusDosyasi = dokunusDosyasi;
            yon.AddComponent<DokunmaGirdisi>().yonetici = oy;
            UnityEditor.Events.UnityEventTools.AddPersistentListener(arayuz.basla.onClick, oy.BaslaDugmesi);
            UnityEditor.Events.UnityEventTools.AddPersistentListener(arayuz.tekrar.onClick, oy.YenidenOynatDugmesi);

            EditorSceneManager.SaveScene(sahne, SahneYolu);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(SahneYolu, true) };
            ProjeAyarlari();
            AssetDatabase.SaveAssets();
            Debug.Log($"[Kafe] sahne kuruldu: {SahneYolu} · {veri.masalar.Length} masa");
        }

        // Derleme ayarları (Ders 4.3): dikey ekran, Web'de sıkıştırma kapalı (her durağan dosya sunucusunda açılır)
        public static void ProjeAyarlari()
        {
            PlayerSettings.productName = "Kafe Robotu";
            PlayerSettings.companyName = "Forenly Academy";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            PlayerSettings.WebGL.decompressionFallback = false;
            PlayerSettings.defaultWebScreenWidth = 540;
            PlayerSettings.defaultWebScreenHeight = 960;
            PlayerSettings.runInBackground = true;
            EditorUserBuildSettings.development = false;
        }
    }
}

// Ders 1.3 · Dokunarak Yönlendirme — ekran pikseli ile kafe zemini (metre) arasında çeviri.
// Unity'de ekranın (0, 0) noktası sol alt köşedir. Kamera yukarıdan dik bakar (ortografik).
using UnityEngine;

namespace Kafe
{
    public static class EkranDunya
    {
        // ÖRNEK telefon: dikey 1080 × 1920 piksel; kamera kafenin 4,5 × 8,0 m'lik bölümünü gösterir.
        public static readonly Vector2 ReferansEkran = new Vector2(1080f, 1920f);
        public static readonly Vector2 KafeBoyu = new Vector2(4.5f, 8.0f);

        // Kamera olmadan, oranla: piksel → zemin (x, z) metre
        public static Vector2 Dunyaya(Vector2 piksel, Vector2 ekran)
            => new Vector2(piksel.x / ekran.x * KafeBoyu.x, piksel.y / ekran.y * KafeBoyu.y);

        public static Vector2 Ekrana(Vector2 zemin, Vector2 ekran)
            => new Vector2(zemin.x / KafeBoyu.x * ekran.x, zemin.y / KafeBoyu.y * ekran.y);

        // Kamerayla: Unity'nin kendi çevirisi (Camera.ScreenToWorldPoint)
        public static Vector2 Kamerayla(Camera kamera, Vector2 piksel)
        {
            var w = kamera.ScreenToWorldPoint(new Vector3(piksel.x, piksel.y, kamera.transform.position.y));
            return new Vector2(w.x, w.z);
        }
    }
}

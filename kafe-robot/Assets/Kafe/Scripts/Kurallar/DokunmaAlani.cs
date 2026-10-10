// Ders 4.2 · Ekran ve Dokunma Alanı — yoğunluktan bağımsız birim (dp) ve güvenli alan.
using UnityEngine;

namespace Kafe
{
    public static class DokunmaAlani
    {
        public const float OnerilenDp = 48f;        // Android erişilebilirlik önerisi: en az 48 dp
        public static float Piksel(float dp, float dpi) => dp * dpi / 160f;
        public static float Milimetre(float dp) => dp / 160f * 25.4f;

        // Düğme dikdörtgeni güvenli alanın tamamen içinde mi? (ikisi de ekran pikseli, sol alt köşe başlangıç)
        public static bool GuvenliAlanda(Rect dugme, Rect guvenli)
            => dugme.xMin >= guvenli.xMin && dugme.xMax <= guvenli.xMax && dugme.yMin >= guvenli.yMin && dugme.yMax <= guvenli.yMax;
    }
}

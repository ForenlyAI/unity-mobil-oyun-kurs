// Ders 1.4 · Yapay Zekâyla Kod ve Test — "hedefe en çok adım kadar yaklaş".
// İlk sürümü Claude Code yazdı; altı testin beşini geçti. Kalan test (eksi adım) istekte söylenmeyen
// bir durumu gösterdi: adım sıfır ya da eksi ise yerinde kal. Düzeltme aşağıdaki ilk satır.
using UnityEngine;

namespace Kafe
{
    public static class Hareket
    {
        public static Vector2 Yaklas(Vector2 konum, Vector2 hedef, float adim)
        {
            if (adim <= 0f) return konum;

            Vector2 fark = hedef - konum;
            float mesafe = fark.magnitude;

            if (mesafe <= adim || mesafe <= Mathf.Epsilon)
            {
                return hedef;
            }

            return konum + fark / mesafe * adim;
        }
    }
}

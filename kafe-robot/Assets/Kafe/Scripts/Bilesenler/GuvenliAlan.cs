// Ders 4.2 · Ekran ve Dokunma Alanı — arayüz panelini telefonun güvenli alanına (Screen.safeArea) sığdırır.
using UnityEngine;

namespace Kafe
{
    [RequireComponent(typeof(RectTransform))]
    public class GuvenliAlan : MonoBehaviour
    {
        Rect son;
        void Update()
        {
            Rect g = Screen.safeArea;
            if (g == son || Screen.width == 0) return;
            son = g;
            var rt = (RectTransform)transform;
            rt.anchorMin = new Vector2(g.xMin / Screen.width, g.yMin / Screen.height);
            rt.anchorMax = new Vector2(g.xMax / Screen.width, g.yMax / Screen.height);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
        }
    }
}

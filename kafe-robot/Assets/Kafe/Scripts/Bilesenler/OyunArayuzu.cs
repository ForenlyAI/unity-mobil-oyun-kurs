// Oyunun ekran yazıları ve düğmeleri (uGUI).
using UnityEngine;
using UnityEngine.UI;

namespace Kafe
{
    public class OyunArayuzu : MonoBehaviour
    {
        public Text ustYazi;
        public Text ortaYazi;
        public Button basla;
        public Button tekrar;

        public void Goster(OyunYoneticisi y)
        {
            if (ustYazi) ustYazi.text = $"Süre {y.Sure:0.0} s   Çarpma {y.robot.Carpma}   Puan {y.PuanDegeri}";
            string orta = y.Durum.Simdi switch
            {
                OyunDurumu.Menu => "Başlamak için dokun",
                OyunDurumu.Teslim => $"Teslim edildi · {y.PuanDegeri} puan",
                OyunDurumu.Dokuldu => "Bardak döküldü",
                _ => ""
            };
            if (ortaYazi) ortaYazi.text = orta;
            if (tekrar) tekrar.gameObject.SetActive(y.Durum.Simdi != OyunDurumu.Oyun);
            if (basla) basla.gameObject.SetActive(y.Durum.Simdi != OyunDurumu.Oyun);
        }
    }
}

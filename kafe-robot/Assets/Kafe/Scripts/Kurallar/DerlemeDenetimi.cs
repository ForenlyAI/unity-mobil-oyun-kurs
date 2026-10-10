// Ders 4.3 · Derleme Ayarları — teslimden önce derleme ayarlarını kurallarla denetle.
using System.Collections.Generic;

namespace Kafe
{
    public struct DerlemeAyarlari
    {
        public string[] sahneler;
        public bool gelistirmeSurumu;
        public bool dikey;
        public int hedefKare;
        public string sikistirma;
        public string urunAdi;
    }

    public static class DerlemeDenetimi
    {
        public const int AlanSayisi = 6;

        public static List<string> Denetle(DerlemeAyarlari a)
        {
            var sorun = new List<string>();
            if (a.sahneler == null || a.sahneler.Length == 0) sorun.Add("sahne listesi boş");
            else if (!a.sahneler[0].EndsWith("Kafe.unity")) sorun.Add("ilk sahne Kafe değil");
            if (a.gelistirmeSurumu) sorun.Add("geliştirme sürümü açık");
            if (!a.dikey) sorun.Add("ekran yönü dikey değil");
            if (a.hedefKare < 60) sorun.Add("hedef kare hızı 60'ın altında");
            if (a.sikistirma != "Kapalı") sorun.Add("sunucusuz açılış için sıkıştırma kapalı olmalı");
            if (string.IsNullOrEmpty(a.urunAdi)) sorun.Add("ürün adı boş");
            return sorun;
        }
    }
}

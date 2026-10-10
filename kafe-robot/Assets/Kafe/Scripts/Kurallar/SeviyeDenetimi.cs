// Ders 3.4 · Yapay Zekâyla Hata Ayıklama — daire (robot) ile kutu (masa) değiyor mu?
// Seviye dosyası yüklenirken robotun başlangıç ve teslim yeri bir masanın içinde mi diye bakılır.
using System.Collections.Generic;
using UnityEngine;

namespace Kafe
{
    public static class SeviyeDenetimi
    {
        // Bu işlevi Claude Code yazdı (istek: daire kutuya değiyorsa true dönsün); altı testin altısını geçti.
        // Dairenin merkezinden kutuya olan uzaklık, eksen eksen bulunur ve yarıçapla karşılaştırılır.
        public static bool DaireKutuDegiyor(Vector2 merkez, float yaricap, Vector2 kutuMerkezi, Vector2 kutuYariBoyu)
        {
            Vector2 fark = merkez - kutuMerkezi;
            float dx = Mathf.Max(Mathf.Abs(fark.x) - kutuYariBoyu.x, 0f);
            float dy = Mathf.Max(Mathf.Abs(fark.y) - kutuYariBoyu.y, 0f);
            return dx * dx + dy * dy <= yaricap * yaricap;
        }

        public static List<string> Denetle(SeviyeVerisi s, float robotYaricapi)
        {
            var sorun = new List<string>();
            var yari = new Vector2(s.masaGenislik / 2f, s.masaDerinlik / 2f);
            foreach (var m in s.masalar)
            {
                var k = new Vector2(m.x, m.y);
                if (DaireKutuDegiyor(s.baslangic, robotYaricapi, k, yari)) sorun.Add($"başlangıç {m.ad} ile değiyor");
                if (DaireKutuDegiyor(s.teslim, robotYaricapi, k, yari)) sorun.Add($"teslim yeri {m.ad} ile değiyor");
            }
            return sorun;
        }
    }
}

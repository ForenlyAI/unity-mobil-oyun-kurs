// Ders 3.2 · Prefab ve Seviye — seviye dosyasındaki masaları masa prefab'ından üretir.
using System.Collections.Generic;
using UnityEngine;

namespace Kafe
{
    public class SeviyeKurucu : MonoBehaviour
    {
        public TextAsset seviyeDosyasi;
        public MasaSablonu masaPrefab;
        public Transform masalarKoku;
        public Transform teslimNoktasi;

        public SeviyeVerisi Veri { get; private set; }
        public readonly List<MasaSablonu> Masalar = new List<MasaSablonu>();

        public SeviyeVerisi Yukle()
        {
            Veri = SeviyeVerisi.Oku(seviyeDosyasi.text);
            return Veri;
        }

        // Oyun başlarken: sahnede masa yoksa seviye dosyasından üret (sahnede varsa onları kullan).
        public void Kur()
        {
            Yukle();
            Masalar.Clear();
            if (masalarKoku != null) Masalar.AddRange(masalarKoku.GetComponentsInChildren<MasaSablonu>());
            if (Masalar.Count == 0 && masaPrefab != null)
            {
                foreach (var m in Veri.masalar)
                {
                    var yeni = Instantiate(masaPrefab, new Vector3(m.x, 0f, m.y), Quaternion.identity, masalarKoku);
                    yeni.name = m.ad;
                    Masalar.Add(yeni);
                }
            }
            if (teslimNoktasi) teslimNoktasi.position = new Vector3(Veri.teslim.x, 0.01f, Veri.teslim.y);
        }
    }
}

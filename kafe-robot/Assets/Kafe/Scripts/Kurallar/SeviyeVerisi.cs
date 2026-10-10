// Ders 3.2 · Prefab ve Seviye — seviye, koddan ayrı bir JSON dosyasında (Assets/Kafe/Seviyeler).
using System;
using UnityEngine;

namespace Kafe
{
    [Serializable]
    public class MasaYeri { public string ad; public float x; public float y; }

    [Serializable]
    public class SeviyeVerisi
    {
        public string ad;
        public Vector2 baslangic;      // tezgâhın önü (zemin x, z)
        public Vector2 teslim;         // teslim edilecek masanın önü
        public float masaGenislik = 0.8f;
        public float masaDerinlik = 0.8f;
        public MasaYeri[] masalar;

        public static SeviyeVerisi Oku(string json) => JsonUtility.FromJson<SeviyeVerisi>(json);
    }

    [Serializable]
    public class KayitliDokunus { public float t; public string faz; public float px; public float py; }

    [Serializable]
    public class DokunusListesi
    {
        public string not;
        public KayitliDokunus[] dokunuslar;
        public static DokunusListesi Oku(string json) => JsonUtility.FromJson<DokunusListesi>(json);
    }
}

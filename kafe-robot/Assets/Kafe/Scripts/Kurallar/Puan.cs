// Ders 3.1 · Puan ve Süre — puan kuralı tek bir işlevde.
// ÖRNEK kural: teslim 1000 · otuz saniyenin altında kalan her saniyeye 20 · her çarpma -100 · dökülürse 0
using UnityEngine;

namespace Kafe
{
    public static class Puan
    {
        public const int TeslimPuani = 1000;
        public const float SureSiniri = 30f;
        public const int SaniyeOdulu = 20;
        public const int CarpmaCezasi = 100;

        public static int Hesapla(bool teslim, bool dokuldu, float sure, int carpma)
        {
            if (dokuldu || !teslim) return 0;
            int odul = Mathf.RoundToInt(Mathf.Max(0f, SureSiniri - sure) * SaniyeOdulu);
            return Mathf.Max(0, TeslimPuani + odul - CarpmaCezasi * carpma);
        }
    }
}

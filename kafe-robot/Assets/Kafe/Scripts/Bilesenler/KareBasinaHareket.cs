// Ders 1.2 · Update ve Kare Süresi — yanlış yazım: her karede sabit adım (kare hızına bağlı)
using UnityEngine;

namespace Kafe
{
    public class KareBasinaHareket : MonoBehaviour
    {
        [Tooltip("m/kare — ÖRNEK")] public float adim = 0.01f;
        public int KareSayisi { get; private set; }
        void Update() { transform.position += Vector3.right * adim; KareSayisi++; }
    }
}

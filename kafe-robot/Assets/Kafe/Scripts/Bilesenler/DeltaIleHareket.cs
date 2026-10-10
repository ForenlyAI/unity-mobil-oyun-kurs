// Ders 1.2 · Update ve Kare Süresi — doğru yazım: yol = hız × Time.deltaTime
using UnityEngine;

namespace Kafe
{
    public class DeltaIleHareket : MonoBehaviour
    {
        [Tooltip("m/s — ÖRNEK")] public float hiz = 0.5f;
        public int KareSayisi { get; private set; }
        void Update() { transform.position += Vector3.right * hiz * Time.deltaTime; KareSayisi++; }
    }
}

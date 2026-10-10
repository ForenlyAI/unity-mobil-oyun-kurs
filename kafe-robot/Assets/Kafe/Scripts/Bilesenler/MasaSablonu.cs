// Ders 3.2 · Prefab ve Seviye — masa şablonu. Prefab'daki ölçü değişince bütün kopyalar güncellenir.
using UnityEngine;

namespace Kafe
{
    [ExecuteAlways]
    [RequireComponent(typeof(BoxCollider))]
    public class MasaSablonu : MonoBehaviour
    {
        [Tooltip("m — ÖRNEK")] public float genislik = 0.8f;
        [Tooltip("m — ÖRNEK")] public float derinlik = 0.8f;
        public float yukseklik = 0.75f;
        public Transform tabla;

        public void Uygula()
        {
            var k = GetComponent<BoxCollider>();
            k.size = new Vector3(genislik, yukseklik, derinlik);
            k.center = new Vector3(0f, yukseklik / 2f, 0f);
            if (tabla)
            {
                tabla.localScale = new Vector3(genislik, 0.05f, derinlik);
                tabla.localPosition = new Vector3(0f, yukseklik, 0f);
            }
        }

        void OnValidate() => Uygula();
    }
}

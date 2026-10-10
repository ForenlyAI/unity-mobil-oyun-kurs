// Ders 2.1 · Gövde ve Sabit Adım — Update ve FixedUpdate kaç kez çağrılıyor?
using UnityEngine;

namespace Kafe
{
    public class SayacBileseni : MonoBehaviour
    {
        public int UpdateSayisi, FixedUpdateSayisi;
        void Update() => UpdateSayisi++;
        void FixedUpdate() => FixedUpdateSayisi++;
    }
}

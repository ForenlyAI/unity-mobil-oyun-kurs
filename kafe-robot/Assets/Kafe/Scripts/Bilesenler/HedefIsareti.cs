// Ders 1.2 · Update ve Kare Süresi — hedef işareti her karede Time.deltaTime ile kayar.
using UnityEngine;

namespace Kafe
{
    public class HedefIsareti : MonoBehaviour
    {
        public RobotSurucu robot;
        [Tooltip("m/s — ÖRNEK")] public float hiz = 6f;
        public Renderer gorsel;

        void Update()
        {
            if (robot == null) return;
            bool var = robot.Hedef.HasValue;
            if (gorsel) gorsel.enabled = var;
            if (!var) return;
            Vector2 simdi = new Vector2(transform.position.x, transform.position.z);
            Vector2 yeni = Hareket.Yaklas(simdi, robot.Hedef.Value, hiz * Time.deltaTime);
            transform.position = new Vector3(yeni.x, transform.position.y, yeni.y);
        }
    }
}

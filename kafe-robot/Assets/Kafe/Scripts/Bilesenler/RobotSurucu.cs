// Ders 2.1–2.4 · Robotun sürüşü: FixedUpdate'te (sabit fizik adımı) hız, ivme sınırıyla değiştirilir.
// Rigidbody (katı gövde) hareketi ve çarpışmayı Unity'nin fizik motoru çözer.
using UnityEngine;

namespace Kafe
{
    [RequireComponent(typeof(Rigidbody))]
    [DefaultExecutionOrder(0)]
    public class RobotSurucu : MonoBehaviour
    {
        [Tooltip("m/s — benzetim kaydındaki en büyük hıza yakın")] public float azamiHiz = 0.66f;
        [Tooltip("m/s² — ÖRNEK; 0 ya da eksi = sınırsız")] public float azamiIvme = 1.0f;
        [Tooltip("ÖRNEK gövde yarıçapı (m)")] public float yaricap = 0.25f;
        [Tooltip("Robotun görünen parçaları: gidiş yönüne döner (yalnız görüntü)")] public Transform gorsel;

        public Vector2? Hedef { get; private set; }
        public float SonIvme { get; private set; }
        public float EnBuyukIvme { get; private set; }
        public int Carpma { get; private set; }
        public Vector2 Konum => new Vector2(rb.position.x, rb.position.z);
        public Vector2 Hiz => new Vector2(rb.linearVelocity.x, rb.linearVelocity.z);

        Rigidbody rb;
        Vector3 oncekiHiz;
        bool ilkAdim = true;

        void Awake() { rb = GetComponent<Rigidbody>(); }

        public void HedefVer(Vector2? hedef) => Hedef = hedef;

        public void Sifirla(Vector2 konum)
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            rb.position = new Vector3(konum.x, 0f, konum.y);
            transform.position = rb.position;
            rb.linearVelocity = Vector3.zero;
            oncekiHiz = Vector3.zero; ilkAdim = true;
            Hedef = null; SonIvme = 0; EnBuyukIvme = 0; Carpma = 0;
        }

        void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            Vector3 v = rb.linearVelocity;                 // fizik motorunun son adımdan sonraki hızı
            SonIvme = ilkAdim ? 0f : (v - oncekiHiz).magnitude / dt;   // son adımdaki gerçek ivme (çarpma dahil)
            ilkAdim = false;
            EnBuyukIvme = Mathf.Max(EnBuyukIvme, SonIvme);

            Vector2 simdi = new Vector2(v.x, v.z);
            Vector2 istenen = Vector2.zero;
            bool sinirsiz = azamiIvme <= 0f;
            if (Hedef is Vector2 h)
            {
                Vector2 fark = h - Konum;
                float uz = fark.magnitude;
                float frenli = sinirsiz ? azamiHiz : Mathf.Sqrt(2f * azamiIvme * uz);   // fren mesafesi
                if (uz > 1e-6f) istenen = fark / uz * Mathf.Min(azamiHiz, Mathf.Min(frenli, uz / dt));
            }
            Vector2 dv = istenen - simdi;
            if (!sinirsiz) dv = Vector2.ClampMagnitude(dv, azamiIvme * dt);            // ivme sınırı
            Vector2 yeni = simdi + dv;
            rb.linearVelocity = new Vector3(yeni.x, 0f, yeni.y);
            oncekiHiz = v;
        }

        void Update()
        {
            if (gorsel == null) return;
            Vector2 v = Hiz;
            if (v.sqrMagnitude < 0.0025f) return;
            var hedefYon = Quaternion.LookRotation(new Vector3(v.x, 0f, v.y), Vector3.up);
            gorsel.rotation = Quaternion.RotateTowards(gorsel.rotation, hedefYon, 360f * Time.deltaTime);
        }

        void OnCollisionEnter(Collision c) { Carpma++; }
    }
}

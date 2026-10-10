// Ders 2.3 · Bardağın Dökülmesi — robotun ivmesi eşiği aşarsa bardak dökülür.
using UnityEngine;

namespace Kafe
{
    [DefaultExecutionOrder(10)]
    public class Bardak : MonoBehaviour
    {
        [Tooltip("m/s² — ÖRNEK dökülme eşiği")] public float esik = 2.5f;
        public RobotSurucu robot;
        public Transform bardakGorseli;
        public GameObject dokulenCay;

        public bool Dokuldu { get; private set; }
        public float DokulmeAni { get; private set; } = -1f;
        float zaman;

        public void Sifirla()
        {
            Dokuldu = false; DokulmeAni = -1f; zaman = 0f;
            if (bardakGorseli) bardakGorseli.localRotation = Quaternion.identity;
            if (dokulenCay) dokulenCay.SetActive(false);
        }

        void FixedUpdate()
        {
            zaman += Time.fixedDeltaTime;
            if (!Dokuldu && robot != null && robot.SonIvme > esik)
            {
                Dokuldu = true; DokulmeAni = zaman;
                if (bardakGorseli) bardakGorseli.localRotation = Quaternion.Euler(0f, 0f, 80f);
                if (dokulenCay) dokulenCay.SetActive(true);
            }
        }
    }
}

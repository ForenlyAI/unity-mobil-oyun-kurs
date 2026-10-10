// Ders 1.3 · Dokunarak Yönlendirme — Input System ile dokunuş (telefonda) ve fare (bilgisayarda/Web'de).
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Dokunus = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Kafe
{
    public class DokunmaGirdisi : MonoBehaviour
    {
        public OyunYoneticisi yonetici;

        void OnEnable() { EnhancedTouchSupport.Enable(); }
        void OnDisable() { EnhancedTouchSupport.Disable(); }

        void Update()
        {
            if (yonetici == null) return;
            foreach (var d in Dokunus.activeTouches)
            {
                yonetici.Dokun(d.screenPosition, d.phase);
                return;   // tek parmak yeter
            }
            var fare = Mouse.current;
            if (fare == null) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;   // düğmeye basıldı
            var p = fare.position.ReadValue();
            if (fare.leftButton.wasPressedThisFrame) yonetici.Dokun(p, TouchPhase.Began);
            else if (fare.leftButton.isPressed && fare.delta.ReadValue().sqrMagnitude > 0f) yonetici.Dokun(p, TouchPhase.Moved);
            else if (fare.leftButton.wasReleasedThisFrame) yonetici.Dokun(p, TouchPhase.Ended);
        }
    }
}

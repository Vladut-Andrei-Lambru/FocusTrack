using UnityEngine;
using UnityEngine.InputSystem;
using Unity.FPS.Gameplay;

namespace FPSProgression
{
    // Vladut Andrei - distance-based dash (feels consistent)
    public sealed class DashAbility : MonoBehaviour
    {
        [Header("Input (drag Player/Dash here)")]
        [SerializeField] private InputActionReference dashAction;

        [Header("Dash Tuning")]
        [SerializeField] private float dashDistance = 6f;     // meters
        [SerializeField] private float dashTime = 0.12f;      // seconds
        [SerializeField] private float dashCooldown = 1.0f; // seconds
        [SerializeField] private Camera cam;
        [SerializeField] private float dashFovKick = 8f;
        [SerializeField] private float fovReturnSpeed = 12f;
        float _baseFov;

        private PlayerCharacterController _cc;
        private PlayerInputHandler _input;

        private bool _unlocked;
        private float _nextDashTime;
        private float _dashEndTime;
        private Vector3 _dashVelocity;

        void Awake()
        {
            _cc = GetComponent<PlayerCharacterController>();
            _input = GetComponent<PlayerInputHandler>();
            enabled = false; // locked
            if (!cam) cam = GetComponent<PlayerCharacterController>().PlayerCamera;
            if (cam) _baseFov = cam.fieldOfView;

        }

        void OnEnable()
        {
            if (dashAction == null || dashAction.action == null)
            {
                Debug.LogError("[Vladut Andrei] DashAbility: dashAction NOT assigned.");
                return;
            }

            dashAction.action.Enable();
            dashAction.action.performed += OnDashPerformed;
        }

        void OnDisable()
        {
            if (dashAction != null && dashAction.action != null)
                dashAction.action.performed -= OnDashPerformed;
        }

        public void UnlockDash()
        {
            _unlocked = true;
            enabled = true;
            Debug.Log("[Vladut Andrei] Dash unlocked");
        }

        private void OnDashPerformed(InputAction.CallbackContext ctx)
        {
            if (!_unlocked) return;

            float now = Time.unscaledTime;
            if (now < _nextDashTime) return;

            _nextDashTime = now + dashCooldown;
            _dashEndTime = now + dashTime;

            Vector3 move = _input != null ? _input.GetMoveInput() : Vector3.zero;
            Vector3 dir = move.sqrMagnitude > 0.01f ? transform.TransformVector(move).normalized : transform.forward;

            // distance-based => velocity = distance / time
            float dashSpeed = (dashTime <= 0.001f) ? dashDistance : (dashDistance / dashTime);
            _dashVelocity = dir * dashSpeed;
            
            if (cam) cam.fieldOfView = _baseFov + dashFovKick;


            Debug.Log("[Vladut Andrei] DASH START");
        }

        void Update()
        {
            if (!_unlocked) return;
            
            if (cam) cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, _baseFov, fovReturnSpeed * Time.deltaTime);


            // apply dash while active
            if (Time.unscaledTime < _dashEndTime)
            {
                var v = _cc.CharacterVelocity;
                v.x = _dashVelocity.x;
                v.z = _dashVelocity.z;
                _cc.CharacterVelocity = v;
            }
        }
    }
}

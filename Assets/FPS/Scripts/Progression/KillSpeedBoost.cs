using System.Collections;
using UnityEngine;
using Unity.FPS.Gameplay;

namespace FPSProgression
{
    // Vladut Andrei - obvious speed boost after kill
    public sealed class KillSpeedBoostOnKill : MonoBehaviour
    {
        [SerializeField] private float speedMultiplier = 1.5f;
        [SerializeField] private float duration = 3.0f;

        private PlayerCharacterController _cc;
        private bool _unlocked;

        private float _baseMaxSpeedOnGround;
        private Coroutine _routine;

        void Awake()
        {
            _cc = GetComponent<PlayerCharacterController>();
            _baseMaxSpeedOnGround = _cc.MaxSpeedOnGround;
            enabled = false;
        }

        public void UnlockAdrenaline()
        {
            _unlocked = true;
            enabled = true;
            Debug.Log("[Vladut Andrei] Adrenaline unlocked");
        }

        // Call this from your kill/Xp system when an enemy dies
        public void NotifyKill()
        {
            if (!_unlocked) return;

            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(BoostRoutine());
        }

        private IEnumerator BoostRoutine()
        {
            _cc.MaxSpeedOnGround = _baseMaxSpeedOnGround * speedMultiplier;
            Debug.Log("[Vladut Andrei] ADRENALINE ON");

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                yield return null;
            }

            _cc.MaxSpeedOnGround = _baseMaxSpeedOnGround;
            Debug.Log("[Vladut Andrei] ADRENALINE OFF");
        }
    }
}
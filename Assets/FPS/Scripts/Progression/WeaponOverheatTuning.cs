using UnityEngine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;

namespace FPSProgression
{
    // Vladut Andrei - fixed: always tune the ACTIVE weapon instance + safe clamps
    public sealed class WeaponOverheatTuning : MonoBehaviour
    {
        [Header("Optional: assign if you want. Otherwise it auto-finds active weapon.")]
        [SerializeField] private WeaponController weapon;

        [Header("Tuning multipliers")]
        [SerializeField] private float tier1ReloadRateMultiplier = 1.5f;
        [SerializeField] private float tier2ReloadDelayMultiplier = 0.6f;

        // safety clamps (prevents breaking reload)
        [SerializeField] private float minReloadDelay = 0.05f;
        [SerializeField] private float minReloadRate = 0.1f;

        private PlayerWeaponsManager _weaponsManager;

        private float _baseReloadRate;
        private float _baseReloadDelay;
        private bool _baseCaptured;

        void Awake()
        {
            // Find the player's weapons manager (active runtime weapon lives there)
            _weaponsManager = FindFirstObjectByType<PlayerWeaponsManager>();
        }

        private WeaponController GetTargetWeapon()
        {
            // Prefer active weapon from weapons manager
            if (_weaponsManager != null && _weaponsManager.GetActiveWeapon() != null)
                return _weaponsManager.GetActiveWeapon();

            // fallback to manually assigned weapon / same object
            if (weapon != null) return weapon;
            return GetComponent<WeaponController>();
        }

        private void CaptureBaseIfNeeded(WeaponController w)
        {
            if (_baseCaptured || w == null) return;
            _baseReloadRate = w.AmmoReloadRate;
            _baseReloadDelay = w.AmmoReloadDelay;
            _baseCaptured = true;
        }

        public void ApplyTier(int tier)
        {
            var w = GetTargetWeapon();
            if (w == null)
            {
                Debug.LogWarning("[Vladut Andrei] WeaponOverheatTuning: no WeaponController found.");
                return;
            }

            CaptureBaseIfNeeded(w);

            // Reset to base
            w.AmmoReloadRate = _baseReloadRate;
            w.AmmoReloadDelay = _baseReloadDelay;

            if (tier >= 1)
                w.AmmoReloadRate = _baseReloadRate * tier1ReloadRateMultiplier;

            if (tier >= 2)
                w.AmmoReloadDelay = _baseReloadDelay * tier2ReloadDelayMultiplier;

            // Safety clamps
            w.AmmoReloadRate = Mathf.Max(minReloadRate, w.AmmoReloadRate);
            w.AmmoReloadDelay = Mathf.Max(minReloadDelay, w.AmmoReloadDelay);

            Debug.Log($"[Vladut Andrei] Overheat tier {tier} applied to '{w.WeaponName}' | Rate={w.AmmoReloadRate:0.00} Delay={w.AmmoReloadDelay:0.00}");
        }
    }
}

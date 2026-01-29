using System;
using UnityEngine;

namespace FPSProgression
{
    // Vladut Andrei - crit logic + event for UI feedback
    public sealed class CritProvider : MonoBehaviour
    {
        public static event Action<Vector3> CritHappened;

        [Header("Crit Enabled")]
        [SerializeField] private bool critEnabled = false; // Vladut Andrei (master switch)

        [Header("Crit Tuning (editable in Inspector)")]
        [Range(0f, 1f)]
        [SerializeField] private float chance = 0f;          // Vladut Andrei: default OFF
        [Min(1f)]
        [SerializeField] private float multiplier = 1f;      // Vladut Andrei: default OFF

        public void SetCritEnabled(bool enabled) // Vladut Andrei
        {
            critEnabled = enabled;
        }

        public void SetCrit(float newChance, float newMultiplier)
        {
            chance = Mathf.Clamp01(newChance);
            multiplier = Mathf.Max(1f, newMultiplier);
        }

        // Optional: lets you tweak chance or multiplier independently from upgrades
        public void SetChance(float newChance) => chance = Mathf.Clamp01(newChance); // Vladut Andrei
        public void SetMultiplier(float newMultiplier) => multiplier = Mathf.Max(1f, newMultiplier); // Vladut Andrei

        public void TryApplyCrit(ref float damage, Vector3 hitPos)
        {
            // Vladut Andrei: real OFF switch
            if (!critEnabled) return;
            if (chance <= 0f || multiplier <= 1f) return;

            if (UnityEngine.Random.value < chance)
            {
                damage *= multiplier;
                CritHappened?.Invoke(hitPos);
            }
        }
    }
}
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace FPSProgression
{
    /// <summary>
    /// Awards XP when this object dies.
    /// Designed to work with Unity Learn FPS Microgame Health variants without modifying template code.
    /// Attach to turret/enemy root that has a Health component.
    /// </summary>
    public sealed class GrantXPOnDeath : MonoBehaviour
    {
        [SerializeField] private int xpAmount = 5;

        // Optional: if you later want spawners to know a turret died
        public event Action<GrantXPOnDeath> Died;

        private Component _health;
        private bool _awarded;

        // Reflection caches
        private Func<float> _readCurrentHealth;
        private bool _hasDeathHook;

        // Vladut Andrei - cache reference once (no Find spam)
        private KillSpeedBoostOnKill _killSpeedBoost;

        private void Awake()
        {
            // FPS Microgame usually has a component named "Health"
            _health = GetComponent("Health") as Component;
            if (_health == null)
                _health = GetComponentInChildren(Type.GetType("Health")) as Component;

            // Vladut Andrei - find player kill boost once
            _killSpeedBoost = FindFirstObjectByType<KillSpeedBoostOnKill>();
        }

        private void OnEnable()
        {
            _awarded = false;

            if (_health == null)
            {
                // Nothing to hook into; do nothing.
                return;
            }

            TryHookDeathEvent();
            if (!_hasDeathHook)
            {
                // Fallback: try to read current health so we can detect <= 0 in Update
                _readCurrentHealth = BuildHealthReader(_health);
            }
        }

        private void OnDisable()
        {
            // If object is disabled due to death (pooling), Update may stop.
            // We only award if we can confirm it was dead.
            if (!_awarded && _readCurrentHealth != null)
            {
                if (_readCurrentHealth.Invoke() <= 0f)
                    AwardOnce();
            }
        }

        private void Update()
        {
            if (_awarded) return;
            if (_readCurrentHealth == null) return;

            if (_readCurrentHealth.Invoke() <= 0f)
                AwardOnce();
        }

        private void TryHookDeathEvent()
        {
            _hasDeathHook = false;

            // Common UnityEvent names in templates
            string[] candidateNames =
            {
                "onDie", "OnDie",
                "onDeath", "OnDeath",
                "dieEvent", "DieEvent",
                "deathEvent", "DeathEvent"
            };

            var t = _health.GetType();

            foreach (var name in candidateNames)
            {
                // Field
                var f = t.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (f != null && typeof(UnityEvent).IsAssignableFrom(f.FieldType))
                {
                    var evt = f.GetValue(_health) as UnityEvent;
                    if (evt != null)
                    {
                        evt.AddListener(AwardOnce);
                        _hasDeathHook = true;
                        return;
                    }
                }

                // Property
                var p = t.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (p != null && typeof(UnityEvent).IsAssignableFrom(p.PropertyType))
                {
                    var evt = p.GetValue(_health) as UnityEvent;
                    if (evt != null)
                    {
                        evt.AddListener(AwardOnce);
                        _hasDeathHook = true;
                        return;
                    }
                }
            }
        }

        private static Func<float> BuildHealthReader(Component health)
        {
            var t = health.GetType();

            // Common field/property names
            string[] names =
            {
                "CurrentHealth", "currentHealth",
                "Health", "health",
                "m_CurrentHealth", "_currentHealth"
            };

            // Properties first
            foreach (var n in names)
            {
                var p = t.GetProperty(n, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (p != null && (p.PropertyType == typeof(float) || p.PropertyType == typeof(int)))
                {
                    return () =>
                    {
                        object v = p.GetValue(health);
                        return v is int i ? i : (float)v;
                    };
                }
            }

            // Fields next
            foreach (var n in names)
            {
                var f = t.GetField(n, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (f != null && (f.FieldType == typeof(float) || f.FieldType == typeof(int)))
                {
                    return () =>
                    {
                        object v = f.GetValue(health);
                        return v is int i ? i : (float)v;
                    };
                }
            }

            // If we can't read health, we can't safely poll.
            return null;
        }

        private void AwardOnce()
        {
            if (_awarded) return;
            _awarded = true;

            if (ProgressionManager.Instance != null)
                ProgressionManager.Instance.AddXP(xpAmount);

            // Vladut Andrei - adrenaline / kill speed boost trigger
            _killSpeedBoost?.NotifyKill();

            Died?.Invoke(this);
        }

        // Convenience for inspector
        public void SetXPAmount(int value) => xpAmount = Mathf.Max(0, value);
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FPSProgression
{
    /// <summary>
    /// Minimal XP + Level system.
    /// - XP gained from kills
    /// - Level up every xpPerLevel XP
    /// - Updates a UI slider (and optional texts)
    /// </summary>
    public sealed class ProgressionManager : MonoBehaviour
    {
        public static ProgressionManager Instance { get; private set; }

        [Header("Tuning")]
        [SerializeField] private int xpPerLevel = 30;

        [Header("UI")]
        [SerializeField] private Slider xpSlider;
        [SerializeField] private TMP_Text xpText;
        [SerializeField] private TMP_Text levelText;

        public int Level { get; private set; } = 1;
        public int CurrentXP { get; private set; } = 0;

        public event Action<int, int> XPChanged; // (current, required)
        public event Action<int> LevelUp;        // (new level)

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            // Optional: persist across scene reloads
            // DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            RefreshUI();
        }

        public void AddXP(int amount)
        {
            if (amount <= 0) return;

            CurrentXP += amount;

            while (CurrentXP >= xpPerLevel)
            {
                CurrentXP -= xpPerLevel;
                Level++;
                LevelUp?.Invoke(Level);
            }

            XPChanged?.Invoke(CurrentXP, xpPerLevel);
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (xpSlider != null)
            {
                xpSlider.minValue = 0;
                xpSlider.maxValue = xpPerLevel;
                xpSlider.value = CurrentXP;
            }

            if (xpText != null)
                xpText.text = $"{CurrentXP} / {xpPerLevel} XP";

            if (levelText != null)
                levelText.text = $"Lv. {Level}";
        }
    }
}

using System.Collections;
using TMPro;
using UnityEngine;

namespace FPSProgression
{
    // Vladut Andrei - simple HUD: upgrades list + toast + CRIT toast
    public sealed class ProgressionHUDSimple : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private UpgradeState state;

        [Header("UI")]
        [SerializeField] private TMP_Text upgradesText;
        [SerializeField] private TMP_Text toastText;

        [Header("Toast Timing")]
        [SerializeField] private float toastSeconds = 1.0f;

        Coroutine _toastRoutine;

        private void Awake()
        {
            CritProvider.CritHappened += OnCritHappened;
        }

        private void OnDestroy()
        {
            CritProvider.CritHappened -= OnCritHappened;
        }

        private void Start()
        {
            RefreshUpgrades();
            HideToastImmediate();
        }

        public void OnUpgradeApplied(UpgradeId id)
        {
            RefreshUpgrades();
            ShowToast("Unlocked: " + Friendly(id));
        }

        public void RefreshUpgrades()
        {
            if (!upgradesText || !state) return;

            string s = "Upgrades: ";
            bool any = false;

            if (state.dashUnlocked) { s += "Dash, "; any = true; }
            if (state.doubleJumpUnlocked) { s += "DoubleJump, "; any = true; }
            if (state.killSpeedBoostUnlocked) { s += "Adrenaline, "; any = true; }

            if (state.overheatTier >= 1) { s += "Overheat I, "; any = true; }
            if (state.overheatTier >= 2) { s += "Overheat II, "; any = true; }

            if (state.crit10Unlocked) { s += "Crit 10%, "; any = true; }

            if (!any) s += "-";
            else s = s.TrimEnd(' ', ',');

            upgradesText.text = s;
        }

        void OnCritHappened(Vector3 hitPos)
        {
            // Vladut Andrei: don't show crit UI unless unlocked
            if (state == null || !state.crit10Unlocked) return;

            ShowToast("CRIT!");
        }

        void ShowToast(string msg)
        {
            if (!toastText) return;

            toastText.gameObject.SetActive(true);
            toastText.text = msg;

            if (_toastRoutine != null) StopCoroutine(_toastRoutine);
            _toastRoutine = StartCoroutine(HideToastAfter());
        }

        IEnumerator HideToastAfter()
        {
            float t = 0f;
            while (t < toastSeconds)
            {
                t += Time.unscaledDeltaTime; // works even if timeScale=0
                yield return null;
            }
            HideToastImmediate();
        }

        void HideToastImmediate()
        {
            if (toastText) toastText.gameObject.SetActive(false);
        }

        static string Friendly(UpgradeId id)
        {
            return id switch
            {
                UpgradeId.Dash => "Dash",
                UpgradeId.DoubleJump => "Double Jump",
                UpgradeId.KillSpeedBoost => "Adrenaline Rush",
                UpgradeId.OverheatTier1 => "Overheat I",
                UpgradeId.OverheatTier2 => "Overheat II",
                UpgradeId.CritChance10 => "Crit +10%",
                _ => id.ToString()
            };
        }
    }
}

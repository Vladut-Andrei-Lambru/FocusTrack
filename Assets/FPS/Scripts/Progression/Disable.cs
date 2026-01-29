using UnityEngine;

namespace FPSProgression
{
    /// <summary>
    /// Vladut Andrei:
    /// Disables the existing InGameMenu UI while the upgrade menu is open,
    /// so it can't steal pointer focus / cursor logic / raycasts.
    ///
    /// Attach this to the SAME GameObject as LevelUpMenu (UpgradePanel).
    /// </summary>
    public sealed class InGameMenuDisablerWhileUpgradeOpen : MonoBehaviour
    {
        [Header("Assign the existing InGameMenu root here (the one that breaks clicks)")]
        [SerializeField] private GameObject inGameMenuRoot;

        [Tooltip("If true, we restore the previous active state on close.")]
        [SerializeField] private bool restorePreviousState = true;

        private bool _cachedWasActive;

        private void OnEnable()
        {
            if (inGameMenuRoot == null)
            {
                return;
            }

            _cachedWasActive = inGameMenuRoot.activeSelf;

            // Disable it while upgrade menu is open
            inGameMenuRoot.SetActive(false);
        }

        private void OnDisable()
        {
            if (inGameMenuRoot == null) return;

            if (restorePreviousState)
            {
                // Restore exactly how it was before we opened the upgrade menu
                inGameMenuRoot.SetActive(_cachedWasActive);
            }
            else
            {
                // Or just always re-enable it when upgrade menu closes
                inGameMenuRoot.SetActive(true);
            }
        }
    }
}
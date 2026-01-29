using System.Collections;
using UnityEngine;

namespace FPSProgression
{
    public sealed class RankUpgradeSystem : MonoBehaviour
    {
        [SerializeField] private RankDefinition[] ranks;
        [SerializeField] private LevelUpMenu menu;
        [SerializeField] private UpgradeApplier applier;

        Coroutine m_BindRoutine;

        void Start()
        {
            // Bind after all Awake() calls have run
            m_BindRoutine = StartCoroutine(BindWhenReady());
        }

        IEnumerator BindWhenReady()
        {
            while (ProgressionManager.Instance == null)
                yield return null;

            ProgressionManager.Instance.LevelUp += OnLevelUp;
            Debug.Log("[Vladut] RankUpgradeSystem bound to ProgressionManager.LevelUp");
        }

        void OnDestroy()
        {
            if (ProgressionManager.Instance != null)
                ProgressionManager.Instance.LevelUp -= OnLevelUp;

            if (m_BindRoutine != null)
                StopCoroutine(m_BindRoutine);
        }

        void OnLevelUp(int newLevel)
        {
            Debug.Log($"[Vladut] OnLevelUp received: {newLevel}");

            // Level starts at 1. First unlock is when reaching Level 2.
            int rankIndex = newLevel - 2;

            if (rankIndex < 0 || rankIndex >= ranks.Length)
            {
                Debug.Log($"[Vladut] Rank index out of range: {rankIndex}");
                return;
            }

            if (menu == null)
            {
                Debug.LogError("[Vladut] Menu reference is missing in RankUpgradeSystem!");
                return;
            }

            menu.Show(ranks[rankIndex], OnPickUpgrade);
        }

        void OnPickUpgrade(UpgradeId id)
        {
            Debug.Log($"[Vladut] Picked upgrade: {id}");

            if (applier == null)
            {
                Debug.LogError("[Vladut] Applier reference is missing in RankUpgradeSystem!");
                return;
            }

            applier.Apply(id);
        }
    }
}

using UnityEngine;

namespace FPSProgression
{
    // Vladut Andrei - enemy-side: listens to GrantXPOnDeath and tells manager to respawn
    public sealed class RespawnMeOnDeath : MonoBehaviour
    {
        [SerializeField] private NavMeshRespawnManager.SpawnType type;
        [SerializeField] private float respawnDelay = 0.5f;

        [Header("After N kills, spawn 2 instead of 1")]
        [SerializeField] private int duplicateAfterKills = 5;
        [SerializeField] private int spawnCountAfterDuplicate = 2;

        private static int s_TotalKills;

        private GrantXPOnDeath _grant;
        private Vector3 _deathPos;
        private int _spawnCount;

        private void Awake()
        {
            _grant = GetComponent<GrantXPOnDeath>();
            if (_grant == null) _grant = GetComponentInChildren<GrantXPOnDeath>();
        }

        private void OnEnable()
        {
            if (_grant != null) _grant.Died += OnDied;
            else Debug.LogWarning("[Vladut Andrei] RespawnMeOnDeath: missing GrantXPOnDeath on " + name);
        }

        private void OnDisable()
        {
            if (_grant != null) _grant.Died -= OnDied;
        }

        private void OnDied(GrantXPOnDeath dead)
        {
            s_TotalKills++;
            _deathPos = transform.position;

            _spawnCount = (s_TotalKills >= duplicateAfterKills)
                ? Mathf.Max(1, spawnCountAfterDuplicate)
                : 1;

            Debug.Log($"[Vladut Andrei] OnDied: {type} totalKills={s_TotalKills} spawnCount={_spawnCount}");

            if (NavMeshRespawnManager.Instance == null)
            {
                Debug.LogError("[Vladut Andrei] RespawnMeOnDeath: No NavMeshRespawnManager in scene!");
                return;
            }

            // IMPORTANT: manager handles the delay (enemy can be disabled/destroyed)
            NavMeshRespawnManager.Instance.RequestRespawn(type, _deathPos, _spawnCount, respawnDelay);
        }
    }
}
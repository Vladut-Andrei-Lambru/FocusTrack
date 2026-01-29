using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace FPSProgression
{
    // Vladut Andrei - reliable respawn manager (not affected by enemy destruction or Time.timeScale)
    public sealed class NavMeshRespawnManager : MonoBehaviour
    {
        public static NavMeshRespawnManager Instance { get; private set; }

        public enum SpawnType { HoverBot, Turret }

        [Header("Prefabs")]
        [SerializeField] private GameObject hoverBotPrefab;
        [SerializeField] private GameObject turretPrefab;

        [Header("NavMesh Spawn")]
        [SerializeField] private Transform spawnCenter; // optional
        [SerializeField] private float spawnRadius = 40f;
        [SerializeField] private int maxTries = 30;
        [SerializeField] private float sampleMaxDistance = 8f;
        [SerializeField] private int areaMask = NavMesh.AllAreas;

        [Header("Avoid Player")]
        [SerializeField] private float minDistanceFromPlayer = 10f;

        private Transform _player;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            var pcc = FindFirstObjectByType<Unity.FPS.Gameplay.PlayerCharacterController>();
            if (pcc != null) _player = pcc.transform;
        }

        public void RequestRespawn(SpawnType type, Vector3 aroundPos, int count, float delay)
        {
            Debug.Log($"[Vladut Andrei] RequestRespawn: {type} x{count} delay={delay:0.00} at {aroundPos}");
            StartCoroutine(RespawnRoutine(type, aroundPos, count, delay));
        }

        private IEnumerator RespawnRoutine(SpawnType type, Vector3 aroundPos, int count, float delay)
        {
            if (delay > 0f)
                yield return new WaitForSecondsRealtime(delay); // IMPORTANT: works even when paused

            var prefab = GetPrefab(type);
            if (prefab == null)
            {
                Debug.LogError($"[Vladut Andrei] RespawnManager prefab missing for {type}");
                yield break;
            }

            count = Mathf.Max(1, count);

            for (int i = 0; i < count; i++)
            {
                Vector3 center = spawnCenter ? spawnCenter.position : aroundPos;
                Vector3 spawnPos = FindNavMeshPoint(center, aroundPos);

                Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                Instantiate(prefab, spawnPos, rot);

                // tiny stagger so double spawn doesn't overlap perfectly
                yield return new WaitForSecondsRealtime(0.05f);
            }
        }

        private GameObject GetPrefab(SpawnType type)
        {
            return type switch
            {
                SpawnType.HoverBot => hoverBotPrefab,
                SpawnType.Turret => turretPrefab,
                _ => null
            };
        }

        private Vector3 FindNavMeshPoint(Vector3 center, Vector3 fallback)
        {
            for (int i = 0; i < maxTries; i++)
            {
                Vector2 r = Random.insideUnitCircle * spawnRadius;
                Vector3 candidate = center + new Vector3(r.x, 0f, r.y);

                if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, sampleMaxDistance, areaMask))
                {
                    if (_player != null && Vector3.Distance(hit.position, _player.position) < minDistanceFromPlayer)
                        continue;

                    return hit.position;
                }
            }

            // last fallback: try to snap fallback position itself
            if (NavMesh.SamplePosition(fallback, out NavMeshHit hit2, sampleMaxDistance, areaMask))
                return hit2.position;

            return fallback;
        }
    }
}

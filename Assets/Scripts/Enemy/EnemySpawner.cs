using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    public GameObject enemyPrefab;

    [Header("Spawn Settings")]
    public int enemyCount = 3;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    private void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner: Enemy Prefab is not assigned!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("EnemySpawner: No spawn points assigned!");
            return;
        }

        for (int i = 0; i < enemyCount; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];

            Instantiate(
                enemyPrefab,
                spawnPoint.position,
                Quaternion.identity
            );
        }
    }
}

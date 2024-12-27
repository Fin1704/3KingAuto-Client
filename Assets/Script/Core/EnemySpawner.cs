using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private Vector2 spawnAreaMin;
    [SerializeField] private Vector2 spawnAreaMax;
    [SerializeField] private int maxEnemies = 10;

    [Header("Debug Settings")]
    [SerializeField] private Color gizmoColor = Color.green;

    private bool isBossActive;
    private int currentEnemyCount;
    private readonly List<GameObject> activeEnemies = new List<GameObject>();

    #region Unity Lifecycle Methods

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void Start()
    {
        InitializeEnemies();
    }

    private void Update()
    {
        if (!isBossActive)
        {
            ManageEnemySpawning();
            CleanupDeadEnemies();
        }
    }

    private void OnDrawGizmos()
    {
        DrawSpawnArea();
    }

    #endregion

    #region Event Management

    private void SubscribeToEvents()
    {
        EventManager.StartListening("OnCallBoss", HandleBossSpawn);
        EventManager.StartListening("OnBossEnd", HandleBossEnd);
    }

    private void UnsubscribeFromEvents()
    {
        EventManager.StopListening("OnCallBoss", HandleBossSpawn);
        EventManager.StopListening("OnBossEnd", HandleBossEnd);
    }

    private void HandleBossSpawn(object[] parameters)
    {
        isBossActive = true;
        DespawnAllEnemies();
    }

    private void HandleBossEnd(object[] parameters)
    {
        isBossActive = false;
    }

    #endregion

    #region Enemy Management

    private void InitializeEnemies()
    {
        ValidateEnemyPrefabs();
        SpawnInitialEnemies();
    }

    private void ValidateEnemyPrefabs()
    {
        if (enemyPrefabs == null || enemyPrefabs.Count == 0)
        {
            Debug.LogError($"[{nameof(EnemySpawner)}] Enemy prefab list is empty or null!");
            enabled = false;
        }
    }

    private void SpawnInitialEnemies()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    private void ManageEnemySpawning()
    {
        if (currentEnemyCount < maxEnemies)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (!CanSpawnEnemy()) return;

        GameObject enemyPrefab = GetRandomEnemyPrefab();
        Vector2 spawnPosition = GenerateRandomSpawnPosition();
        GameObject newEnemy = InstantiateEnemy(enemyPrefab, spawnPosition);
        Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
        enemyComponent.SetDataByCharacter(GetRandomCharacterData());
        TrackEnemy(newEnemy);
    }

    private Character GetRandomCharacterData()
    {
        return new Character
        {
            hp = Random.Range(5, 50),
            attackMin = Random.Range(5, 10),
            attackMax = Random.Range(10, 30),
            moveSpeed = Random.Range(1, 2),
            attackSpeed = Random.Range(1, 3)
        };
    }

    private bool CanSpawnEnemy()
    {
        return enemyPrefabs != null && enemyPrefabs.Count > 0;
    }

    private GameObject GetRandomEnemyPrefab()
    {
        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        return enemyPrefabs[randomIndex];
    }

    private Vector2 GenerateRandomSpawnPosition()
    {
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        return new Vector2(randomX, randomY);
    }

    private GameObject InstantiateEnemy(GameObject prefab, Vector2 position)
    {
        return Instantiate(prefab, position, Quaternion.identity);
    }

    private void TrackEnemy(GameObject enemy)
    {
        activeEnemies.Add(enemy);
        currentEnemyCount++;
    }

    private void CleanupDeadEnemies()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] == null)
            {
                activeEnemies.RemoveAt(i);
                currentEnemyCount--;
            }
        }
    }

    private void DespawnAllEnemies()
    {
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        activeEnemies.Clear();
        currentEnemyCount = 0;
    }

    #endregion

    #region Gizmos

    private void DrawSpawnArea()
    {
        Gizmos.color = gizmoColor;
        Vector2[] corners = new Vector2[]
        {
            new Vector2(spawnAreaMin.x, spawnAreaMin.y),
            new Vector2(spawnAreaMax.x, spawnAreaMin.y),
            new Vector2(spawnAreaMax.x, spawnAreaMax.y),
            new Vector2(spawnAreaMin.x, spawnAreaMax.y)
        };

        for (int i = 0; i < corners.Length; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % corners.Length]);
        }
    }

    #endregion
}

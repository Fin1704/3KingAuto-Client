using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // Danh sách các prefab của quái vật
    public Vector2 spawnAreaMin; // Điểm bắt đầu của phạm vi spawn
    public Vector2 spawnAreaMax; // Điểm kết thúc của phạm vi spawn
    public int maxEnemies = 10; // Số lượng quái vật tối đa trong game
    private bool isSummonBoss = false;
    private int currentEnemyCount = 0; // Số lượng quái vật hiện tại
    private List<GameObject> activeEnemies = new List<GameObject>(); // Danh sách quái vật đang tồn tại
    void OnEnable()
    {
        EventManager.StartListening("OnCallBoss", OnCallBoss);
        EventManager.StartListening("OnBossEnd", OnBossEnd);

        
    }

    private void OnBossEnd(object[] obj)
    {
       isSummonBoss = false;
    }

    private void OnCallBoss(object[] parameters)
    {
        isSummonBoss = true;
        DispawnAll();
        
    }

    void OnDisable()
    {
        EventManager.StopListening("OnCallBoss", OnCallBoss);
        EventManager.StopListening("OnBossEnd", OnBossEnd);
    }

    void Start()
    {
        // Khởi tạo và spawn 10 quái vật
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    void Update()
    {
        if (!isSummonBoss)
        {
            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }

            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                if (activeEnemies[i] == null)
                {
                    activeEnemies.RemoveAt(i);
                    currentEnemyCount--;
                }
            }
        }

    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogError("Enemy prefab list is empty! Cannot spawn enemies.");
            return;
        }

        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        GameObject selectedEnemyPrefab = enemyPrefabs[randomIndex];

        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        Vector2 spawnPosition = new Vector2(randomX, randomY);

        GameObject newEnemy = Instantiate(selectedEnemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(newEnemy);
        currentEnemyCount++;
    }
    private void DispawnAll()
    {

        foreach (GameObject enemy in activeEnemies)
        {
            Destroy(enemy);
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(spawnAreaMin.x, spawnAreaMin.y), new Vector2(spawnAreaMax.x, spawnAreaMin.y)); // Bottom
        Gizmos.DrawLine(new Vector2(spawnAreaMax.x, spawnAreaMin.y), new Vector2(spawnAreaMax.x, spawnAreaMax.y)); // Right
        Gizmos.DrawLine(new Vector2(spawnAreaMax.x, spawnAreaMax.y), new Vector2(spawnAreaMin.x, spawnAreaMax.y)); // Top
        Gizmos.DrawLine(new Vector2(spawnAreaMin.x, spawnAreaMax.y), new Vector2(spawnAreaMin.x, spawnAreaMin.y)); // Left
    }



}
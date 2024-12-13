using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // Danh sách các prefab của quái vật
    public Vector2 spawnAreaMin; // Điểm bắt đầu của phạm vi spawn
    public Vector2 spawnAreaMax; // Điểm kết thúc của phạm vi spawn
    public int maxEnemies = 10; // Số lượng quái vật tối đa trong game

    private int currentEnemyCount = 0; // Số lượng quái vật hiện tại
    private List<GameObject> activeEnemies = new List<GameObject>(); // Danh sách quái vật đang tồn tại

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
        // Kiểm tra nếu số lượng quái vật dưới mức tối đa
        if (currentEnemyCount < maxEnemies)
        {
            SpawnEnemy(); // Spawn một quái vật mới
        }

        // Kiểm tra các quái vật đã chết và loại bỏ khỏi danh sách
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] == null)
            {
                activeEnemies.RemoveAt(i); // Xóa quái vật đã chết khỏi danh sách
                currentEnemyCount--; // Giảm số lượng quái vật
            }
        }
    }

    private void SpawnEnemy()
    {
        // Chọn ngẫu nhiên một prefab từ danh sách
        if (enemyPrefabs.Count == 0)
        {
            Debug.LogError("Enemy prefab list is empty! Cannot spawn enemies.");
            return;
        }

        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        GameObject selectedEnemyPrefab = enemyPrefabs[randomIndex];

        // Tính toán vị trí spawn ngẫu nhiên trong phạm vi
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        Vector2 spawnPosition = new Vector2(randomX, randomY);

        // Spawn quái vật tại vị trí ngẫu nhiên
        GameObject newEnemy = Instantiate(selectedEnemyPrefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(newEnemy); // Thêm quái vật vào danh sách
        currentEnemyCount++; // Tăng số lượng quái vật hiện tại
    }

    // Vẽ phạm vi spawn lên Scene view trong Unity
    private void OnDrawGizmos()
    {
        // Vẽ hình chữ nhật đại diện cho phạm vi spawn
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(spawnAreaMin.x, spawnAreaMin.y), new Vector2(spawnAreaMax.x, spawnAreaMin.y)); // Bottom
        Gizmos.DrawLine(new Vector2(spawnAreaMax.x, spawnAreaMin.y), new Vector2(spawnAreaMax.x, spawnAreaMax.y)); // Right
        Gizmos.DrawLine(new Vector2(spawnAreaMax.x, spawnAreaMax.y), new Vector2(spawnAreaMin.x, spawnAreaMax.y)); // Top
        Gizmos.DrawLine(new Vector2(spawnAreaMin.x, spawnAreaMax.y), new Vector2(spawnAreaMin.x, spawnAreaMin.y)); // Left
    }
}
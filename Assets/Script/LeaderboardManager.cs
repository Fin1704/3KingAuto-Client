using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardPanel; // Panel chứa bảng xếp hạng
    [SerializeField] private Transform leaderboardContent; // Parent object chứa các dòng của bảng xếp hạng
    [SerializeField] private GameObject leaderboardRowPrefab; // Prefab của mỗi dòng
    [SerializeField] private float updateInterval = 600f; // 10 phút (600 giây)
    [SerializeField] private Button openLeaderboardButton;
    private string apiUrl;

    private bool isUpdating = false;

    private class PlayerData
    {
        public int id;
        public string username;
        public int gems;
        public int rank;
    }

    private class ApiResponse
    {
        public bool success;
        public string message;
        public List<PlayerData> topPlayers;
    }

    // Khởi chạy cập nhật định kỳ
    private void Start()
    {
        apiUrl=DataManager.Instance.SERVER_URL+"/api/game/top";
        StartCoroutine(PeriodicUpdateLeaderboard());
        openLeaderboardButton.onClick.AddListener(() => leaderboardPanel.SetActive(!leaderboardPanel.activeSelf));
    }

    private IEnumerator PeriodicUpdateLeaderboard()
    {
        while (true)
        {
            yield return StartCoroutine(GetLeaderboardData());
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private IEnumerator GetLeaderboardData()
    {
        if (isUpdating)
            yield break;

        isUpdating = true;

        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(apiUrl))
        {
            request.SetRequestHeader("Authorization", "Bearer "+DataManager.Instance.Get<string>("token"));
            // Gửi request
            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError ||
                request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error fetching leaderboard data: {request.error}");
            }
            else
            {
                // Parse JSON response
                var jsonResponse =JsonConvert.DeserializeObject<ApiResponse>(request.downloadHandler.text);

                if (jsonResponse.success)
                {
                    UpdateLeaderboardUI(jsonResponse.topPlayers);
                }
                else
                {
                    Debug.LogError($"Error in response: {jsonResponse.message}");
                }
            }
        }

        isUpdating = false;
    }

    private void UpdateLeaderboardUI(List<PlayerData> players)
    {
        // Xóa các dòng cũ
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }

        // Thêm dữ liệu mới vào bảng
        foreach (var player in players)
        {
            GameObject row = Instantiate(leaderboardRowPrefab, leaderboardContent);
            TMP_Text[] rowTexts = row.GetComponentsInChildren<TMP_Text>();

            // Cập nhật các cột (thứ hạng, tên, gems)
            rowTexts[0].text = player.rank.ToString(); // Cột thứ hạng
            rowTexts[1].text = player.username; // Cột tên
            rowTexts[2].text = player.gems.ToString(); // Cột gems
        }

        // Hiển thị bảng xếp hạng
        
    }
}

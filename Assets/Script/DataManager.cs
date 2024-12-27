using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
    // Đối tượng Singleton
    public static DataManager Instance;

    // Dictionary để lưu trữ dữ liệu
    private Dictionary<string, object> data = new Dictionary<string, object>();
    public string SERVER_URL;
    public bool isDev; // New public variable

    // URL của file JSON
    private string serverConfigURL = "https://devmini.com/server.json";

    // Kiểm tra nếu đối tượng đã tồn tại hay chưa
    void Awake()
    {
        if (Instance == null)
        {
            // Nếu chưa, gán this làm instance duy nhất
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Giữ đối tượng này khi chuyển scene
            StartCoroutine(LoadServerConfig());
        }
        else
        {
            // Nếu đối tượng đã tồn tại, hủy bỏ đối tượng mới
            Destroy(gameObject);
        }
    }

    // Phương thức để tải file JSON
    private IEnumerator LoadServerConfig()
    {
        if (isDev)
        {
            SERVER_URL = "http://localhost:3000";
            Debug.Log("Development mode: SERVER_URL set to " + SERVER_URL);
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(serverConfigURL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    // Deserialize JSON để lấy SERVER_URL
                    ServerConfig config = JsonUtility.FromJson<ServerConfig>(request.downloadHandler.text);
                    SERVER_URL = config.serverUrl;
                    Debug.Log("SERVER_URL loaded: " + SERVER_URL);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error parsing server.json: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("Failed to load server.json: " + request.error);
            }
        }
    }

    // Class để map dữ liệu JSON
    [Serializable]
    private class ServerConfig
    {
        public string serverUrl; // Match key in JSON file
    }

    // Phương thức Set để lưu dữ liệu vào Dictionary
    public void Set(string key, object value)
    {
        if (data.ContainsKey(key))
        {
            data[key] = value;  // Cập nhật giá trị nếu key đã tồn tại
        }
        else
        {
            data.Add(key, value);  // Thêm mới nếu key chưa tồn tại
        }
    }

    // Phương thức Get để lấy dữ liệu từ Dictionary
    public T Get<T>(string key)
    {
        if (data.ContainsKey(key))
        {
            return (T)data[key];  // Trả về giá trị kiểu T nếu key tồn tại
        }
        else
        {
            Debug.LogWarning("Key not found: " + key);
            return default(T);  // Trả về giá trị mặc định của kiểu T nếu key không tồn tại
        }
    }

    // Phương thức Reset để xóa tất cả dữ liệu
    public void ResetData()
    {
        data.Clear();
    }
}
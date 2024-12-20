using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    // Instance tĩnh của PlayerDataManager (Singleton)
    public static PlayerDataManager Instance { get; private set; }

    // Đối tượng PlayerData đã tạo sẵn từ ScriptableObject
    public PlayerData playerData; 

    // Kiểm tra và đảm bảo chỉ có một instance
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Nếu chưa có instance nào, gán instance này
            DontDestroyOnLoad(gameObject); // Đảm bảo đối tượng không bị hủy khi scene đổi
        }
        else
        {
            Destroy(gameObject); // Nếu đã có instance, hủy đi đối tượng này
        }
    }

    
}

using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }
    public PlayerData playerData; 
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
    private List<Rune> getListRunes()
    {
        return playerData.runes;
    }
    
}

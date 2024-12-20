using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
public class GameDataFetcher : MonoBehaviour
{
    private string url = "https://devmini.com/characters.json";  
    public PlayerData playerData; 
    void Start()
    {
        StartCoroutine(GetUserData());
    }

    IEnumerator GetUserData()
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("Data received: " + jsonResponse);
            ProcessUserData(jsonResponse);
        }
        else
        {
            Debug.LogError("Error fetching data: " + request.error);
        }
    }

   void ProcessUserData(string json)
    {
        UserData userData = JsonConvert.DeserializeObject<UserData>(json);
        playerData.is_get=true;
        playerData.userName = userData.userName;
        playerData.gold = userData.gold;
        playerData.gem = userData.gem;
        foreach (var rune in userData.runes)
        {
            playerData.AddRune(rune);
        }
        foreach (var character in userData.characters)
        {
            playerData.AddCharacter(character);
        }

        
    }
}

[System.Serializable]
public class UserData
{
    public string userName;
    public int gold;
    public int gem;
    public Character[] characters;
    public Rune[] runes;
}
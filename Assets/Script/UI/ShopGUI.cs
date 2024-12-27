using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShopGUI : MonoBehaviour
{
    public Button button;
    public GameObject GUI;
    public Button buyHero2;

private void Start()
    {
        button.onClick.AddListener(openGUI);
     
        GUI.SetActive(false);
        buyHero2.onClick.AddListener(buyHero2Button);
       
    }
 public class HeroResponse
    {
        public bool success { get; set; }
        public Character hero { get; set; }
        public string message { get; set; }
    }

 
    private void buyHero2Button()
    {
         StartCoroutine(SendPurchaseRequest(2));
    }

    private void openGUI()
    {
                EventManager.FireEvent("OnOpenUI", "ShopGUI");

        GUI.SetActive(!GUI.activeSelf);
    }
     private IEnumerator SendPurchaseRequest(int heroId)
    {
        string url = DataManager.Instance.SERVER_URL+"/api/game/buy-hero"; // Thay URL server của bạn
        var requestData = new { idHero = heroId }; // Tạo data gửi đi

        // Convert object sang JSON
        string jsonData = JsonConvert.SerializeObject(requestData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                        request.SetRequestHeader("Authorization", $"Bearer {DataManager.Instance.Get<string>("token")}");

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Gửi request và chờ phản hồi
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Deserialize JSON response
                try
                {
                    HeroResponse response = JsonConvert.DeserializeObject<HeroResponse>(request.downloadHandler.text);
                    if (response != null && response.success)
                    {
                        Debug.Log("Hero purchased successfully!");
                        Debug.Log($"Hero ID: {response.hero.id}, Level: {response.hero.level}");
                       ToastManager.Instance.ShowToast("Hero purchased successfully!");
                        PlayerDataManager.Instance.playerData.AddCharacter(response.hero);

                    }
                    else
                    {
                        ToastManager.Instance.ShowToast(response.message);
                        Debug.LogError("Error in response: " + response.message);
                    }
                }
                catch (System.Exception e)
                {
                      ToastManager.Instance.ShowToast(e.Message);
                    Debug.LogError("Failed to parse response: " + e.Message);
                }
            }
            else
            {
                ToastManager.Instance.ShowToast("Request failed: " + request.error);
               
                Debug.LogError("Request failed: " + request.error);
            }
        }
    }
}


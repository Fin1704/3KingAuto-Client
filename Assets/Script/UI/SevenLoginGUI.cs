using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SevenLoginGUI : MonoBehaviour
{
    public Button button;
    public GameObject GUI;
    public GameObject DaNhan;
    public GameObject ChuaNhan;
    public GameObject GUIReward;
    public TMP_Text goldText;
    public Image runeImage;
    public RuneMasterData runeMasterData;
    public GameObject rewardPanel;
    private void Start()
    {
        button.onClick.AddListener(openGUI);
        ChuaNhan.GetComponent<Button>().onClick.AddListener(claimReward);
        GUI.SetActive(false);

    }

    private void claimReward()
    {
        rewardPanel.SetActive(false);
        StartCoroutine(MineMinerals());
    }

    public static bool IsToday(string lastDailyReward)
    {
        if (DateTime.TryParse(lastDailyReward, out DateTime rewardDate))
        {
            return rewardDate.Date == DateTime.Today;
        }
        else
        {
            return false;
        }
    }

    private void openGUI()
    {
        if (IsToday(PlayerDataManager.Instance.playerData.lastDailyReward))
        {
            Debug.Log("Da nhan");
            DaNhan.SetActive(true);
            ChuaNhan.SetActive(false);
        }
        else
        {
            Debug.Log("Chua nhan");

            DaNhan.SetActive(false);
            ChuaNhan.SetActive(true);
        }
        EventManager.FireEvent("OnOpenUI", "SevenLoginGUI");
        GUI.SetActive(!GUI.activeSelf);
    }

    public IEnumerator MineMinerals()
    {

        string ApiUrl = DataManager.Instance.SERVER_URL + "/api/game/daily-reward";
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(ApiUrl))
        {
            request.SetRequestHeader("Authorization", "Bearer " + DataManager.Instance.Get<string>("token"));
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
                var jsonResponse = JsonConvert.DeserializeObject<DailyReward>(request.downloadHandler.text);
                if (jsonResponse.success)
                {
                    GUIReward.SetActive(true);
                    runeImage.sprite = runeMasterData.GetImageById(jsonResponse.newRune.id);
                    goldText.text = jsonResponse.gems.ToString();
                    PlayerDataManager.Instance.playerData.AddCoins(jsonResponse.gems);
                    PlayerDataManager.Instance.playerData.AddRune(jsonResponse.newRune);

                    yield return new WaitForSeconds(4);

                    GUIReward.SetActive(false);
                    rewardPanel.SetActive(true);
                    GUI.SetActive(false);
                    PlayerDataManager.Instance.playerData.UpdateRewardDate();

                }
                else
                {
                    ToastManager.Instance.ShowToast(jsonResponse.message);
                }
            }

        }

    }
}

public class DailyReward
{
    public bool success;
    public Rune newRune;
    public int gems;
    public string message;
}
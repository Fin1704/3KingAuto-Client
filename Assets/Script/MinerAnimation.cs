using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinerAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private SkeletonAnimation mineAnimation;
    [SerializeField] private Button tapButton;
    [SerializeField] private Image imageReward;
    [SerializeField] private TMP_Text price;
    [SerializeField] private TMP_Text name;
    [SerializeField] private GameObject panel;


    [SerializeField] private OreMasterData oreMasterData;


    private void Start()
    {
        tapButton.onClick.AddListener(PlayMiningAnimation);
    }

    private async void PlayMiningAnimation()
    {
        try
        {
            if (PlayerDataManager.Instance.playerData.gold < 100)
            {
                ToastManager.Instance.ShowToast("Not enough gold to mine. Need 100 Gold!");
                return;
            }

            PlayerDataManager.Instance.playerData.UseGold(100);

            tapButton.interactable = false;
            mineAnimation.gameObject.SetActive(true);
            mineAnimation.AnimationState.SetAnimation(0, "animation", true);


    Debug.Log("Mining animation started");
            StartCoroutine(MineMinerals());

        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during mining animation: {ex.Message}");
            // Ensure the animation object is disabled even if there's an error
            mineAnimation.gameObject.SetActive(false);
        }
    }

    public IEnumerator MineMinerals()
    {
    Debug.Log("Mining animation started 1"  );

        yield return new WaitForSeconds(3);
        mineAnimation.gameObject.SetActive(false);
        string ApiUrl = DataManager.Instance.SERVER_URL + "/api/game/miner";
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(ApiUrl))
        {
            request.SetRequestHeader("Authorization", "Bearer " + DataManager.Instance.Get<string>("token"));
            // Gửi request
            yield return request.SendWebRequest();
            Debug.Log(request.downloadHandler.text);
            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError ||
                request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error fetching leaderboard data: {request.error}");
            }
            else
            {
                // Parse JSON response
                var jsonResponse = JsonConvert.DeserializeObject<MiningResponse>(request.downloadHandler.text);

                if (jsonResponse.success)
                {
                    panel.SetActive(true);
                    OreData oreData = oreMasterData.GetPrefabById(jsonResponse.selectedId);
                    name.text = oreData.name;
                    price.text = oreData.sellingPrice.ToString();
                    PlayerDataManager.Instance.playerData.AddCoins(oreData.sellingPrice);
                    imageReward.sprite = oreData.sprite;
                    yield return new WaitForSeconds(3);

                    panel.SetActive(false);
                    tapButton.interactable = true;
                }
                else
                {
                    ToastManager.Instance.ShowToast(jsonResponse.message);
                }
            }

        }

    }
}
[Serializable]
public class MiningResponse
{
    public bool success;
    public int selectedId;
    public int rewardGems;
    public string message;
}



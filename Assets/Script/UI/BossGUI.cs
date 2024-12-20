using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BossGUI : MonoBehaviour
{
    public Button button;
    public GameObject GUI;
    public Button buttonCall;
    public FadeWarning fadeWarning;
    public int priceSummon = 3;
    public GameObject summonBossPanel;
    public GameObject bossExistsPanel;

    // Tham chiếu đến Boss Prefab
    public GameObject bossPrefab;
    public Vector2 spawnAreaMin; // Điểm bắt đầu của phạm vi spawn
    public Vector2 spawnAreaMax; // Điểm kết thúc của phạm vi spawn
    private bool isHaveBoss = false;

    private void Start()
    {
        button.onClick.AddListener(openGUI);
        buttonCall.onClick.AddListener(callBoss);
        GUI.SetActive(false);
    }
 void OnEnable()
    {
        EventManager.StartListening("OnBossDeath", BossEnded);

        
    }
       void OnDisable()
    {
        EventManager.StopListening("OnBossDeath", BossEnded);
    }

    private void callBoss()
    {
        if (PlayerDataManager.Instance.playerData.gem < priceSummon)
        {
            ToastManager.Instance.ShowToast("Not enough gems");

        }
        if (isHaveBoss)
        {
            ToastManager.Instance.ShowToast("Boss already exists");

        }

        isHaveBoss = true;
        // Trigger warning and fire event
        fadeWarning.TriggerWarning();
        EventManager.FireEvent("OnCallBoss", Time.time);
        GUI.SetActive(false);

        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        Vector2 spawnPosition = new Vector2(randomX, randomY);
        GameObject bossobject = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        Boss boss = bossobject.GetComponent<Boss>();
        boss.PlayAppearEffect();


    }
    private void BossEnded(object[] obj){
        
        isHaveBoss = false;
        EventManager.FireEvent("OnBossEnd", Time.time);
        fadeWarning.TriggerStopWarning();
    }
    private void openGUI()
    {
        EventManager.FireEvent("OnOpenUI", "BossGUI");
        if (isHaveBoss)
        {
            bossExistsPanel.SetActive(true);
            summonBossPanel.SetActive(false);
        }
        else
        {
            bossExistsPanel.SetActive(false);
            summonBossPanel.SetActive(true);
        }
        GUI.SetActive(!GUI.activeSelf);


    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(new Vector2(spawnAreaMin.x, spawnAreaMin.y), new Vector2(spawnAreaMax.x, spawnAreaMin.y)); // Bottom
        Gizmos.DrawLine(new Vector2(spawnAreaMax.x, spawnAreaMin.y), new Vector2(spawnAreaMax.x, spawnAreaMax.y)); // Right
        Gizmos.DrawLine(new Vector2(spawnAreaMax.x, spawnAreaMax.y), new Vector2(spawnAreaMin.x, spawnAreaMax.y)); // Top
        Gizmos.DrawLine(new Vector2(spawnAreaMin.x, spawnAreaMax.y), new Vector2(spawnAreaMin.x, spawnAreaMin.y)); // Left
    }
}

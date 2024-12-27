using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BossGUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button summonButton;
    [SerializeField] private GameObject mainGUI;
    [SerializeField] private Button callButton;
    [SerializeField] private FadeWarning fadeWarning;
    [SerializeField] private GameObject summonBossPanel;
    [SerializeField] private GameObject bossExistsPanel;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Vector2 spawnAreaMin;
    [SerializeField] private Vector2 spawnAreaMax;
    [SerializeField] private int priceSummon = 200;
    private GameObject bossObject;
    // Cached components and values
    private Transform cachedTransform;
    private bool isHaveBoss;
    private static readonly Vector2[] spawnAreaCorners = new Vector2[4];
    private static readonly WaitForSeconds bossSpawnDelay = new WaitForSeconds(0.5f);
    private readonly Vector3 defaultRotation = Quaternion.identity.eulerAngles;

    // Object pooling for boss instances
    private static readonly ObjectPool<GameObject> bossPool = new ObjectPool<GameObject>();

    private void Awake()
    {
        InitializeComponents();
        InitializeSpawnArea();
    }

    private void Start()
    {
        InitializeEventListeners();
        SetInitialState();
    }

    private void OnEnable()
    {
        EventManager.StartListening("OnBossDeath", OnBossDeath);
    }

    private void OnDisable()
    {
        EventManager.StopListening("OnBossDeath", OnBossDeath);
    }

    private void InitializeComponents()
    {
        cachedTransform = transform;
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (!summonButton || !mainGUI || !callButton || !fadeWarning ||
            !summonBossPanel || !bossExistsPanel || !bossPrefab)
        {
            Debug.LogError($"[{nameof(BossGUI)}] Missing required components!");
            enabled = false;
        }
    }

    private void InitializeEventListeners()
    {
        summonButton.onClick.AddListener(OpenGUI);
        callButton.onClick.AddListener(CallBoss);
    }

    private void SetInitialState()
    {
        mainGUI.SetActive(false);
        isHaveBoss = false;
    }

    private void InitializeSpawnArea()
    {
        spawnAreaCorners[0] = new Vector2(spawnAreaMin.x, spawnAreaMin.y);
        spawnAreaCorners[1] = new Vector2(spawnAreaMax.x, spawnAreaMin.y);
        spawnAreaCorners[2] = new Vector2(spawnAreaMax.x, spawnAreaMax.y);
        spawnAreaCorners[3] = new Vector2(spawnAreaMin.x, spawnAreaMax.y);
    }

    private void CallBoss()
    {
        if (!ValidateBossSpawn()) return;
        StartCoroutine(SummonBossRequest());

        PlayerDataManager.Instance.playerData.UseGold(priceSummon);
        SpawnBoss();

        UpdateUIState();
        TriggerBossEvent();
    }

    private bool ValidateBossSpawn()
    {
        if (PlayerDataManager.Instance.playerData.gold < priceSummon)
        {
            ToastManager.Instance.ShowToast("Not enough gold");
            return false;
        }

        if (isHaveBoss)
        {
            ToastManager.Instance.ShowToast("Boss already exists");
            return false;
        }

        return true;
    }
public class SummonBossResponse  {
    public string code;
}
    private IEnumerator SummonBossRequest()
    {
        string url = $"{DataManager.Instance.SERVER_URL}/api/game/summon-boss";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            // Set headers
            request.SetRequestHeader("Authorization", $"Bearer {DataManager.Instance.Get<string>("token")}");

            yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success)
                {
                    SummonBossResponse summonBossResponse = JsonConvert.DeserializeObject<SummonBossResponse>(request.downloadHandler.text);
                    DataManager.Instance.Set("bossCode", summonBossResponse.code);
                }
        }
    }
    private void SpawnBoss()
    {
        Vector2 spawnPosition = GenerateSpawnPosition();
        GameObject bossObject = GetBossFromPool(spawnPosition);
        if (bossObject.TryGetComponent<Boss>(out var boss))
        {
            boss.PlayAppearEffect();
        }

        isHaveBoss = true;
    }

    private Vector2 GenerateSpawnPosition()
    {
        return new Vector2(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y)
        );
    }

    private GameObject GetBossFromPool(Vector2 position)
    {
        bossObject = bossPool.Get(() => Instantiate(bossPrefab));
        bossObject.transform.position = position;
        bossObject.transform.rotation = Quaternion.Euler(defaultRotation);
        bossObject.SetActive(true);
        return bossObject;
    }

    private void UpdateUIState()
    {
        mainGUI.SetActive(false);
        fadeWarning.TriggerWarning();
    }

    private void TriggerBossEvent()
    {
        EventManager.FireEvent("OnCallBoss", Time.time);
    }

    private void OnBossDeath(object[] obj)
    {

        isHaveBoss = false;
        fadeWarning.TriggerStopWarning();

        EventManager.FireEvent("OnBossEnd", Time.time);
        Destroy(bossObject);


    }

    private void OpenGUI()
    {
        EventManager.FireEvent("OnOpenUI", "BossGUI");
        UpdatePanelVisibility();
        ToggleMainGUI();
    }

    private void UpdatePanelVisibility()
    {
        bossExistsPanel.SetActive(isHaveBoss);
        summonBossPanel.SetActive(!isHaveBoss);
    }

    private void ToggleMainGUI()
    {
        mainGUI.SetActive(!mainGUI.activeSelf);
    }

    private void OnDrawGizmos()
    {
        DrawSpawnArea();
    }

    private void DrawSpawnArea()
    {
        Gizmos.color = Color.black;
        for (int i = 0; i < spawnAreaCorners.Length; i++)
        {
            int nextIndex = (i + 1) % spawnAreaCorners.Length;
            Gizmos.DrawLine(spawnAreaCorners[i], spawnAreaCorners[nextIndex]);
        }
    }
}

// Object Pool implementation
public class ObjectPool<T> where T : class
{
    private readonly Queue<T> pool = new Queue<T>();

    public T Get(System.Func<T> createFunc)
    {
        return pool.Count > 0 ? pool.Dequeue() : createFunc();
    }

    public void Return(T item)
    {
        if (item != null)
        {
            pool.Enqueue(item);
        }
    }
}

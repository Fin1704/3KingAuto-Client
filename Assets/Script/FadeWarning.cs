using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FadeWarning : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject warningUI;
    [SerializeField] private GameObject playTimePanel;
    [SerializeField] private TMP_Text timeText;

    [Header("Warning Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float totalDuration = 3f;
    [SerializeField] private int countdownDuration = 120; // 5 minutes in seconds

    private readonly WaitForSeconds fadeWait;
    private readonly WaitForSeconds oneSecond;
    private readonly System.Text.StringBuilder stringBuilder;
    private Coroutine activeWarningCoroutine;

    public FadeWarning()
    {
        fadeWait = new WaitForSeconds(fadeDuration);
        oneSecond = new WaitForSeconds(1);
        stringBuilder = new System.Text.StringBuilder(5);
    }

    private void Awake()
    {
        InitializeComponents();
    }
private void OnEnable()
    {
        EventManager.StartListening("OnBossManagerEnd", TurnOffTime);


    }

    private void TurnOffTime(object[] obj)
    {
        playTimePanel.SetActive(false);
    }

    private void Start()
    {
        SetInitialState();
    }

    private void InitializeComponents()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (timeText == null) timeText = GetComponent<TMP_Text>();
        
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (canvasGroup == null) Debug.LogError($"{nameof(canvasGroup)} not assigned!");
        if (warningUI == null) Debug.LogError($"{nameof(warningUI)} not assigned!");
        if (playTimePanel == null) Debug.LogError($"{nameof(playTimePanel)} not assigned!");
        if (timeText == null) Debug.LogError($"{nameof(timeText)} not assigned!");
    }

    private void SetInitialState()
    {
        canvasGroup.alpha = 0f;
        warningUI.SetActive(false);
        playTimePanel.SetActive(false);
    }

    public void TriggerWarning()
    {
        StopActiveWarning();
        warningUI.SetActive(true);
        activeWarningCoroutine = StartCoroutine(FlickerEffect());
    }

    public void TriggerStopWarning()
    {
        StopActiveWarning();
        warningUI.SetActive(false);
    }

    private void StopActiveWarning()
    {
        if (activeWarningCoroutine != null)
        {
            StopCoroutine(activeWarningCoroutine);
            activeWarningCoroutine = null;
        }
    }

    private IEnumerator FlickerEffect()
    {
        float endTime = Time.time + totalDuration;

        while (Time.time < endTime)
        {
            yield return FadeSequence();
        }

        TransitionToCountdown();
    }

    private IEnumerator FadeSequence()
    {
        yield return StartCoroutine(Fade(0f, 1f));
        yield return fadeWait;
        yield return StartCoroutine(Fade(1f, 0f));
        yield return fadeWait;
    }

    private void TransitionToCountdown()
    {
        warningUI.SetActive(false);
        playTimePanel.SetActive(true);
        StartCoroutine(StartCountdown(countdownDuration));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        float inverseTime = 1f / fadeDuration;

        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed * inverseTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }

    private IEnumerator StartCountdown(int totalSeconds)
    {
        EventManager.FireEvent("OnBossManagerStart", totalSeconds);
        while (totalSeconds > 0)
        {
            UpdateCountdownDisplay(totalSeconds);
            yield return oneSecond;
            totalSeconds--;
        }

        UpdateCountdownDisplay(0);
       
        EventManager.FireEvent("OnBossManagerTimeUp", totalSeconds);
         playTimePanel.SetActive(false);
    }

    private void UpdateCountdownDisplay(int seconds)
    {
        timeText.text = FormatTime(seconds);
    }

    private string FormatTime(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        
        stringBuilder.Clear()
            .Append(minutes.ToString("D2"))
            .Append(':')
            .Append(seconds.ToString("D2"));
            
        return stringBuilder.ToString();
    }
}

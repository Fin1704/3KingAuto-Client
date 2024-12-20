using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeWarning : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.5f; // Thời gian fade in/out cho mỗi lần chớp
    public float totalDuration = 3f; // Tổng thời gian chớp tắt liên tục
    public GameObject warningui;

    public GameObject PlayTimePanel;
    public TMP_Text timeText;
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    public void TriggerWarning()
    {
        warningui.SetActive(true);
        StartCoroutine(FlickerEffect());
    }
 public void TriggerStopWarning()
    {
        warningui.SetActive(false);
    }
    private IEnumerator FlickerEffect()
    {
        float elapsedTime = 0f;

        while (elapsedTime < totalDuration)
        {
            yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
            yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
            elapsedTime += fadeDuration * 2;
        }
        warningui.SetActive(false);
        PlayTimePanel.SetActive(true);
        StartCoroutine(StartCountdown(300));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;


    }
        private IEnumerator StartCountdown(int totalSeconds)
    {
        while (totalSeconds > 0)
        {
            // Chuyển đổi giây sang định dạng mm:ss
            string timeFormatted = FormatTime(totalSeconds);
            timeText.text = timeFormatted;

            yield return new WaitForSeconds(1); // Chờ 1 giây
            totalSeconds--; // Giảm số giây còn lại
        }

        // Khi kết thúc đếm ngược
        timeText.text = "00:00";
    }

    private string FormatTime(int totalSeconds)
    {
        int minutes = totalSeconds / 60; // Lấy số phút
        int seconds = totalSeconds % 60; // Lấy số giây còn lại
        return $"{minutes:D2}:{seconds:D2}"; // Định dạng thành mm:ss
    }
}

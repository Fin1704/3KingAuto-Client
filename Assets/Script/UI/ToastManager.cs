using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance { get; private set; }  // Singleton Instance

    public GameObject toastPrefab;  // Prefab của Toast
    public Transform toastContainer;  // Container để chứa Toasts

    private Queue<GameObject> activeToasts = new Queue<GameObject>();  // Queue để quản lý các Toasts

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Hàm để hiển thị Toast
    public void ShowToast(string message)
    {
        GameObject newToast = Instantiate(toastPrefab, toastContainer);
        TMP_Text toastText = newToast.GetComponentInChildren<TMP_Text>();
        toastText.text = message;

        activeToasts.Enqueue(newToast);
        StartCoroutine(AnimateToast(newToast));
    }

    private IEnumerator AnimateToast(GameObject toast)
    {
        float duration = 4f;  
        Vector3 startPos = toast.transform.position;
        Vector3 endPos = startPos + new Vector3(0, 2, 0); 

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            toast.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(toast);
    }
}

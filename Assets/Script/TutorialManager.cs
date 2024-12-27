using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Button _button1;
    [SerializeField] private Button _button2;
    [SerializeField] private GameObject _panel1;
    [SerializeField] private GameObject _panel2;

    private void Awake()
    {
        InitializeButtons();
        CheckNewPlayerStatus();
    }

    private void OnEnable()
    {
        if (_button1 != null) _button1.onClick.AddListener(OnButton1Click);
        if (_button2 != null) _button2.onClick.AddListener(OnButton2Click);
    }

    private void OnDisable()
    {
        if (_button1 != null) _button1.onClick.RemoveListener(OnButton1Click);
        if (_button2 != null) _button2.onClick.RemoveListener(OnButton2Click);
    }

    private void InitializeButtons()
    {
        // Validate required components
        if (_button1 == null || _button2 == null || _panel1 == null || _panel2 == null)
        {
            Debug.LogError("Missing required components in TutorialManager");
            enabled = false;
            return;
        }
    }

    private void CheckNewPlayerStatus()
    {
        try
        {
            bool isNewPlayer = DataManager.Instance.Get<bool>("new_player");
            if (isNewPlayer)
            {
                SetPanelStates(true, false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking player status: {e.Message}");
            gameObject.SetActive(false);
        }
    }

    private void OnButton1Click()
    {
        SetPanelStates(false, true);
    }

    private void OnButton2Click()
    {
        gameObject.SetActive(false);
    }

    private void SetPanelStates(bool panel1Active, bool panel2Active)
    {
        if (_panel1 != null) _panel1.SetActive(panel1Active);
        if (_panel2 != null) _panel2.SetActive(panel2Active);
    }
}

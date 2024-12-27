using UnityEngine;
using UnityEngine.UI;

public class SwitchPanel : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private Button switchLoginBtn;
    [SerializeField] private Button switchRegisterBtn;

    private void Awake()
    {
        // Add button listeners on Awake
        if (switchLoginBtn != null)
            switchLoginBtn.onClick.AddListener(SwitchToLogin);
        if (switchRegisterBtn != null)
            switchRegisterBtn.onClick.AddListener(SwitchToRegister);
    }

    private void OnDestroy()
    {
        // Clean up listeners when object is destroyed
        if (switchLoginBtn != null)
            switchLoginBtn.onClick.RemoveListener(SwitchToLogin);
        if (switchRegisterBtn != null)
            switchRegisterBtn.onClick.RemoveListener(SwitchToRegister);
    }

    private void SwitchToRegister()
    {
        SetPanelStates(false, true);
    }

    private void SwitchToLogin()
    {
        SetPanelStates(true, false);
    }

    // Combine panel state changes into one method to reduce code duplication
    private void SetPanelStates(bool loginActive, bool registerActive)
    {
        if (loginPanel != null)
            loginPanel.SetActive(loginActive);
        if (registerPanel != null)
            registerPanel.SetActive(registerActive);
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Newtonsoft.Json;

public class LoginManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Text errorText;
    [SerializeField] private CanvasGroup transitionCanvas;
    [SerializeField] private GameObject loading;

    public PlayerData playerData; 
 
private string serverUrl;

 public void ShowError(string message)
    {
        errorText.text = message;
        errorText.gameObject.SetActive(true);
        StartCoroutine(HideAfterSeconds(2f));
    }

    private IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        errorText.gameObject.SetActive(false);
    }
        private void Start()
    {
        // Add click listener to login button
        loginButton.onClick.AddListener(HandleLogin);
        serverUrl=DataManager.Instance.SERVER_URL;
    }
  private IEnumerator TransitionToMainScene()
    {
        // Fade in transition canvas
        transitionCanvas.gameObject.SetActive(true);
        transitionCanvas.DOFade(1f, 1);

        // Wait for fade to complete
        yield return new WaitForSeconds(1);

        // Load main scene
        SceneManager.LoadScene("Main");
    }
    private void HandleLogin()
    {
        if (ValidateInputs())
        {
            StartCoroutine(SendLoginRequest());
        }
        else
        {
            Debug.LogWarning("Please fill in all fields!");
        }
    }

    private bool ValidateInputs()
    {
        return !string.IsNullOrEmpty(usernameInput.text) && 
               !string.IsNullOrEmpty(passwordInput.text);
    }

    private IEnumerator SendLoginRequest()
    {
        // Create login data object
                loading.SetActive(true);

        LoginData loginData = new LoginData
        {
            username = usernameInput.text,
            password = passwordInput.text
        };

        // Convert to JSON
        string jsonData = JsonUtility.ToJson(loginData);

        // Create request
        using (UnityWebRequest request = new UnityWebRequest(serverUrl+"/api/auth/login", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Login successful!");
                HandleSuccessfulLogin(request.downloadHandler.text);
                StartCoroutine(TransitionToMainScene());
            }
            else 
            {
                HandleLoginError(request.downloadHandler.text);
            }
                    loading.SetActive(false);

        }
    }

    private void HandleSuccessfulLogin(string responseData)
    {
        errorText.gameObject.SetActive(false);
        LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseData);
        if (loginResponse.success==false){
            HandleLoginError(loginResponse.message);
            return;
        };
        playerData.is_get=true;
        playerData.gold=loginResponse.player.gems;
        playerData.userName=loginResponse.player.username;
        DataManager.Instance.Set("token",loginResponse.token);
                 DataManager.Instance.Set("new_player",false);

        foreach (var rune in loginResponse.runes)
        {
            playerData.AddRune(rune);
        }
        foreach (var character in loginResponse.characters)
        {
            playerData.AddCharacter(character);
        }
    }
     private void HandleLoginError(string error)
    {
        Debug.Log(error);
        ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(error);
       ShowError(errorResponse.message);
    }
}

[System.Serializable]
public class LoginData
{
    public string username;
    public string password;
}

[System.Serializable]
public class LoginResponse
{
    public bool success { get; set; }
    public string message { get; set; }
    public string token { get; set; }
    public PlayerServer player { get; set; }
    public Character[] characters;
    public Rune[] runes;
}

public class PlayerServer
{
    public int id { get; set; }
    public string username { get; set; }
    public int gems { get; set; }
}
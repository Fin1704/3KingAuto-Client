using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Newtonsoft.Json;

public class RegisterManager : MonoBehaviour
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
    private void Start()
    {
        // Add click listener to login button
        loginButton.onClick.AddListener(HandleLogin);
        serverUrl=DataManager.Instance.SERVER_URL;
        Debug.Log($"Server URL: {serverUrl}");
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
            StartCoroutine(SendRegisterRequest());
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

    private IEnumerator SendRegisterRequest()
    {
        loading.SetActive(true);
        // Create login data object
        LoginData loginData = new LoginData
        {
            username = usernameInput.text,
            password = passwordInput.text
        };

        // Convert to JSON
        string jsonData = JsonUtility.ToJson(loginData);

        // Create request
        using (UnityWebRequest request = new UnityWebRequest(serverUrl+"/api/auth/register", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Send request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Register successful!");
                HandleSuccessfulRegister(request.downloadHandler.text);
                StartCoroutine(TransitionToMainScene());
            }
            else
            {
                HandleRegisterError(request.downloadHandler.text);
            }
            loading.SetActive(false);

        }
    }

    private void HandleSuccessfulRegister(string responseData)
    {
        errorText.gameObject.SetActive(false);
       
        LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseData);
        playerData.is_get=true;
        playerData.gold=loginResponse.player.gems;
        playerData.userName=loginResponse.player.username;
                playerData.lastDailyReward=loginResponse.player.lastDailyReward;

        DataManager.Instance.Set("token",loginResponse.token);
         DataManager.Instance.Set("new_player",true);
        foreach (var rune in loginResponse.runes)
        {
            playerData.AddRune(rune);
        }
        foreach (var character in loginResponse.characters)
        {
            playerData.AddCharacter(character);
        }
    }
    private void HandleRegisterError(string error)
    {
        Debug.Log("Register failed: " + error);
        ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(error);
       errorText.gameObject.SetActive(true);
       errorText.text=errorResponse.message;
    }
}

public class ErrorResponse
{
      public bool success;
        public string message ;
        public ArrayList error;
  
}
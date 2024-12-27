using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RuneGUI : MonoBehaviour
{
    public Button button;
    public GameObject GUI;
    public PlayerData playerData;
    public GameObject noRuneLabel;
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;
    public GameObject cellRune;
    public Transform listRuneParent;
    public Button buttonEquip;
    public GameObject listButtonCell;
    private Rune selectedRune;
    private TMP_Text textBtnEquip;
public TMP_Text text_description;
    private void Start()
    {
        button.onClick.AddListener(openGUI);
        buttonEquip.onClick.AddListener(ToggleEquipStatus);  // Equip/Unequip button listener
        GUI.SetActive(false);
        textBtnEquip = buttonEquip.GetComponentInChildren<TMP_Text>();
    }

    private void openGUI()
    {
        EventManager.FireEvent("OnOpenUI", "RuneGUI");

        GUI.SetActive(!GUI.activeSelf);
        if (GUI.activeSelf)
        {
            UpdateSlot();
            UpdateRuneList();
        }
    }

    private void UpdateSlot()
    {
        Dictionary<int, GameObject> slotMapping = new Dictionary<int, GameObject>
        {
            { 1, slot1 },
            { 2, slot2 },
            { 3, slot3 }
        };
        if (playerData.runes.Count == 0)
        {
            noRuneLabel.SetActive(true);
        }else{
            noRuneLabel.SetActive(false);
        
        }
        // Loop through all runes
        foreach (Rune rune in playerData.runes)
        {
            // Check if the rune index exists in the dictionary
            if (slotMapping.ContainsKey(rune.index))
            {
                // Find the "Icon_Rune" child and update its sprite
                Transform iconRuneTransform = slotMapping[rune.index].transform.Find("Icon_Rune");

                if (iconRuneTransform != null)
                {
                    iconRuneTransform.GetComponent<Image>().sprite = rune.runeData.icon;
                    slotMapping[rune.index].GetComponentInChildren<TMP_Text>().text = rune.runeData.description;
                    iconRuneTransform.gameObject.SetActive(true);
                }
            }
        }

        foreach (var entry in slotMapping)
        {
            if (!playerData.runes.Exists(rune => rune.index == entry.Key))
            {
                Transform iconRuneTransform = entry.Value.transform.Find("Icon_Rune");
                if (iconRuneTransform != null)
                {
                    iconRuneTransform.gameObject.SetActive(false);
                    entry.Value.GetComponentInChildren<TMP_Text>().text = "";

                }
            }
        }
    }
    private void UpdateRuneList()
    {
        listButtonCell.SetActive(false);

        // Clear the list before adding new runes
        foreach (Transform child in listRuneParent)
        {
            Destroy(child.gameObject);
        }

        // Loop through all runes in playerData.runes
        foreach (Rune rune in playerData.runes)
        {
            GameObject newRuneCell = Instantiate(cellRune, listRuneParent);

            // Update the icon
            Transform iconTransform = newRuneCell.transform.Find("Icon");
            if (iconTransform != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = rune.runeData.icon;
                }
            }

            // Update the cell background color based on equip status
            Image cellImage = newRuneCell.GetComponent<Image>();
            if (cellImage != null)
            {
                cellImage.color = rune.isEquipped ? Color.red : new Color32(3, 82, 141, 255);
            }

            // Handle the selection indicator (child named "selected")
            Transform selectedTransform = newRuneCell.transform.Find("selected");
            if (selectedTransform != null)
            {
                selectedTransform.gameObject.SetActive(false); // Initially hide it
            }

            // Add click listener to the rune cell
            Button runeCellButton = newRuneCell.GetComponent<Button>();
            if (runeCellButton != null)
            {
                runeCellButton.onClick.AddListener(() =>
                {
                    SelectRune(rune, newRuneCell);
                });
            }
        }
    }
    private void SelectRune(Rune rune, GameObject runeCell)
    {
        selectedRune = rune;
        listButtonCell.SetActive(true);
        text_description.text = rune.runeData.description;
        foreach (Transform child in listRuneParent)
        {
            Transform selectedTransform = child.Find("selected");
            if (selectedTransform != null)
            {
                selectedTransform.gameObject.SetActive(false);
            }
        }

        Transform currentSelectedTransform = runeCell.transform.Find("selected");
        if (currentSelectedTransform != null)
        {
            currentSelectedTransform.gameObject.SetActive(true);
        }

        if (selectedRune != null)
        {
            textBtnEquip.text = selectedRune.isEquipped ? "Unequip" : "Equip";
        }
    }

    // This method toggles the "Equip" or "Unequip" status of the selected rune
    private void ToggleEquipStatus()
    {
        if (selectedRune != null)
        {
            int equippedRunes = playerData.runes.FindAll(rune => rune.isEquipped).Count;

            if (equippedRunes >= 3 && !selectedRune.isEquipped)
            {
                Debug.Log("All slots are full. Cannot equip more runes.");
                return;
            }

            selectedRune.isEquipped = !selectedRune.isEquipped;
            if (!selectedRune.isEquipped)
            {
                selectedRune.index = 0;
                StartCoroutine(UnEquipRuneRequest(selectedRune.key));
            }
            else
            {
                int availableIndex = GetAvailableIndex();
                if (availableIndex != -1)
                {
                    selectedRune.index = availableIndex;
                    StartCoroutine(EquipRuneRequest(selectedRune.key, selectedRune.index));
                }
                else
                {
                    Debug.Log("No available slots.");
                    return;
                }
            }
            // Update the button text
            textBtnEquip.text = selectedRune.isEquipped ? "Unequip" : "Equip";

            // Update the UI
            UpdateRuneList();
            UpdateSlot();
        }
    }
    private int GetAvailableIndex()
    {
        List<int> occupiedIndexes = new List<int>();

        // Get all the occupied indices (1 to 3)
        foreach (Rune rune in playerData.runes)
        {
            if (rune.isEquipped)
            {
                occupiedIndexes.Add(rune.index);
            }
        }

        // Check for the smallest available index
        for (int i = 1; i <= 3; i++)
        {
            if (!occupiedIndexes.Contains(i))
            {
                return i;  // Return the first available index
            }
        }

        return -1; // No available slots
    }
    [System.Serializable]
    public class EquipData
    {
        public int id;
        public int index;
    }
    private IEnumerator EquipRuneRequest(int key, int index)
    {
        // Create login data object
        EquipData equipData = new EquipData
        {
            id = key,
            index = index
        };

        // Convert to JSON
        string jsonData = JsonUtility.ToJson(equipData);
        Debug.Log(jsonData);
        // Create request
        using (UnityWebRequest request = new UnityWebRequest(DataManager.Instance.SERVER_URL + "/api/game/equip-rune", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.SetRequestHeader("Authorization", $"Bearer {DataManager.Instance.Get<string>("token")}");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);
            }

        }
    }

    [System.Serializable]
    public class UnEquipData
    {
        public int id;
    }
    private IEnumerator UnEquipRuneRequest(int key)
    {
        UnEquipData equipData = new UnEquipData
        {
            id = key,
        };
        string jsonData = JsonUtility.ToJson(equipData);
        Debug.Log(jsonData);
        using (UnityWebRequest request = new UnityWebRequest(DataManager.Instance.SERVER_URL + "/api/game/unequip-rune", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.SetRequestHeader("Authorization", $"Bearer {DataManager.Instance.Get<string>("token")}");
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);
            }

        }
    }
}



using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RuneGUI : MonoBehaviour
{
    public Button button;
    public GameObject GUI;
    public PlayerData playerData;
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;
    public GameObject cellRune;
    public Transform listRuneParent;
    public Button buttonEquip;
public GameObject listButtonCell;
    private Rune selectedRune;
    private TMP_Text textBtnEquip;

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
            if (!selectedRune.isEquipped){
                selectedRune.index =0;
            }else{
               int availableIndex = GetAvailableIndex();
            if (availableIndex != -1)
            {
                selectedRune.index = availableIndex;
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
}

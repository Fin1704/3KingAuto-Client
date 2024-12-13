using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroGUI : MonoBehaviour
{
    public Button button;
    public Button battleButton;
    public GameObject GUI;
    public PlayerData playerData;
    public GameObject cardFrame;
    public Transform listCharacterParent;

    public TMP_Text level_text;
    public TMP_Text def_text;
    public TMP_Text atk_text;
    public TMP_Text hp_text;
    public TMP_Text speed_text;

    private Character selectedCharacter;
    private Dictionary<int, GameObject> spawnedCharacters = new Dictionary<int, GameObject>();
    public Vector3 spawnPosition;

    private void Start()
    {
        button.onClick.AddListener(openGUI);
        battleButton.onClick.AddListener(Battle);
        GUI.SetActive(false);
        DisplayCharacterCards();
    }

    private void DisplayCharacterCards()
    {
        if (!playerData.is_get) return;
        foreach (Transform child in listCharacterParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var character in playerData.characters)
        {
            GameObject card = Instantiate(cardFrame, listCharacterParent);
            UpdateCardWithCharacterData(card, character);
        }
    }

    private void UpdateCardWithCharacterData(GameObject card, Character character)
    {
        Transform imageTransform = card.transform.Find("Mask");
        Instantiate(character.characterImage, imageTransform);

        Button button = card.GetComponent<Button>();
        if (button == null)
        {
            button = card.AddComponent<Button>();
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            ShowCharacterInfo(character.id);
        });
    }

    private void ShowCharacterInfo(int id)
    {
        Character character = playerData.GetCharacterById(id);
        selectedCharacter = character;
         if (battleButton != null)
        {
            TMP_Text buttonText = battleButton.GetComponentInChildren<TMP_Text>();
            if (selectedCharacter.isBattle)
            {
                buttonText.text = "Go Back";
            }
            else
            {
                buttonText.text = "Battle";
            }
        }
        Debug.Log($"Selected Character: {character.isBattle}");

        level_text.text = character.level.ToString();
        def_text.text = character.defense.ToString();
        atk_text.text = $"{character.attackMin}-{character.attackMax}";
        hp_text.text = character.hp.ToString();
        speed_text.text = character.moveSpeed.ToString();
    }

    private void Battle()
    {
        if (selectedCharacter == null)
        {
            Debug.LogError("No character selected for battle!");
            return;
        }

        if (selectedCharacter.isBattle)
        {
            RemoveCharacter();
            selectedCharacter.isBattle=false;
            return;
        }

        if (spawnPosition == null)
        {
            Debug.LogError("Spawn position is not assigned.");
            return;
        }

        if (selectedCharacter.characterPrefab == null)
        {
            Debug.LogError("Character prefab is not assigned.");
            return;
        }

        GameObject spawnedCharacter = Instantiate(selectedCharacter.characterPrefab, spawnPosition, Quaternion.identity);
        Player spawnedCharacterData=spawnedCharacter.GetComponent<Player>();
        spawnedCharacterData.SetDataByCharacter(selectedCharacter);
        spawnedCharacters.Add(selectedCharacter.id, spawnedCharacter);
        selectedCharacter.isBattle = true;
        if (battleButton != null)
        {
            TMP_Text buttonText = battleButton.GetComponentInChildren<TMP_Text>();
            if (selectedCharacter.isBattle)
            {
                buttonText.text = "Go Back";
            }
            else
            {
                buttonText.text = "Battle";
            }
        }

    }

    private void openGUI()
    {
        DisplayCharacterCards();
        GUI.SetActive(!GUI.activeSelf);
    }

    void Update() { }

    public void RemoveCharacter()
    {
        if (spawnedCharacters.TryGetValue(selectedCharacter.id, out GameObject gameObject))
        {
            Destroy(gameObject);

            spawnedCharacters.Remove(selectedCharacter.id);
        }
        battleButton.GetComponentInChildren<TMP_Text>().text = "Battle";
    }
}

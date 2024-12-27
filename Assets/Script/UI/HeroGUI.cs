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

    public TMP_Text bonus_def_text;
    public TMP_Text bonus_atk_text;
    public TMP_Text bonus_hp_text;
    public TMP_Text bonus_speed_text;

    private Character selectedCharacter;
    private Dictionary<int, GameObject> spawnedCharacters = new Dictionary<int, GameObject>();
    public Vector3 spawnPosition;
    private Image buttonCurrent;
    public GameObject detailPanel;
    private void Start()
    {

        button.onClick.AddListener(openGUI);
        battleButton.onClick.AddListener(Battle);
        DisplayCharacterCards();
        GUI.SetActive(false);
        UpdateRuneBonus();
    }
    private void UpdateRuneBonus()
    {
        Character dataRune = playerData.GetBonusRune();
        bonus_def_text.text = dataRune.defense == 0 ? "" : "+" + dataRune.defense.ToString();
        bonus_atk_text.text = dataRune.attackMax == 0 ? "" : "+" + dataRune.attackMin.ToString() + "-" + dataRune.attackMax.ToString();
        bonus_hp_text.text = dataRune.hp == 0 ? "" : "+" + dataRune.hp.ToString();
        bonus_speed_text.text = dataRune.moveSpeed == 0 ? "" : "+" + dataRune.moveSpeed.ToString();

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
        Instantiate(character.icon, imageTransform);

        Button button = card.GetComponent<Button>();
        if (button == null)
        {
            button = card.AddComponent<Button>();
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {

            if (buttonCurrent)
            {
                buttonCurrent.color =new Color(97, 97, 103);
            }
            buttonCurrent = card.GetComponent<Image>();
            buttonCurrent.color = new Color(198, 198, 198);
            ShowCharacterInfo(character.id);


        });
    }

    private void ShowCharacterInfo(int id)
    {
        Character character = playerData.GetCharacterById(id);
        UpdateRuneBonus();

        selectedCharacter = character;
        if (battleButton != null)
        {
            TMP_Text buttonText = battleButton.GetComponentInChildren<TMP_Text>();
            if (selectedCharacter.isBattle)
            {
                buttonText.text = "Go Back";
                battleButton.GetComponent<Image>().color = new Color(198, 198, 198);

            }
            else
            {
                buttonText.text = "Battle";
                battleButton.GetComponent<Image>().color = new Color(97, 97, 103);

            }
        }
        Debug.Log($"Selected Character: {character.isBattle}");

        level_text.text = character.exp.ToString() + "/" + character.level.ToString();
        def_text.text = character.defense.ToString();
        atk_text.text = $"{character.attackMin}-{character.attackMax}";
        hp_text.text = character.hp.ToString();

        speed_text.text = character.moveSpeed.ToString();
        detailPanel.SetActive(true);

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
            selectedCharacter.isBattle = false;
            return;
        }

        if (spawnPosition == null)
        {
            Debug.LogError("Spawn position is not assigned.");
            return;
        }

        if (selectedCharacter.character == null)
        {
            Debug.LogError("Character prefab is not assigned.");
            return;
        }

        GameObject spawnedCharacter = Instantiate(selectedCharacter.character, spawnPosition, Quaternion.identity);
        Player spawnedCharacterData = spawnedCharacter.GetComponent<Player>();
        Character dataRune = playerData.GetBonusRune();
        Character dataCharacter = playerData.GetCharacterById(selectedCharacter.id);
        dataCharacter.hp += dataRune.hp;
        dataCharacter.attackMin += dataRune.attackMin;  
        dataCharacter.attackMax += dataRune.attackMax;  
        dataCharacter.defense += dataRune.defense;  
        dataCharacter.attackSpeed += dataRune.attackSpeed;  
        dataCharacter.moveSpeed += dataRune.moveSpeed;  
        
        spawnedCharacterData.SetDataByCharacter(dataCharacter);
        spawnedCharacters.Add(selectedCharacter.id, spawnedCharacter);
        selectedCharacter.isBattle = true;
        if (battleButton != null)
        {
            TMP_Text buttonText = battleButton.GetComponentInChildren<TMP_Text>();
            if (selectedCharacter.isBattle)
            {
                buttonText.text = "Go Back";
                battleButton.GetComponent<Image>().color = new Color(198, 198, 198);

            }
            else
            {
                buttonText.text = "Battle";
                battleButton.GetComponent<Image>().color = new Color(97, 97, 103);

            }
        }

    }

    private void openGUI()
    {
        EventManager.FireEvent("OnOpenUI", "HeroGUI");

        detailPanel.SetActive(false);
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
        battleButton.GetComponent<Image>().color = new Color(97, 97, 103);
    }
}

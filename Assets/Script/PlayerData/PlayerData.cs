using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/PlayerData")]
public class PlayerData : ScriptableObject
{
    public bool is_get;
    public string userName;
    public int gold;
    public int gem;
 
    public List<Rune> runes = new List<Rune>();
    public List<Character> characters=new List<Character>();

    public CharacterMasterData characterPrefabData;
    public RuneMasterData runeMasterData;
    private void OnEnable()
    {
        is_get = false;
        userName = string.Empty; 
        gold = 0;
        gem = 0;
        characters = new List<Character>();
        runes = new List<Rune>();
    }

    public void ResetData()
    {
        gold = 0;
        gem = 0;
        userName = "Unknown";
        characters.Clear();
        runes.Clear();
    }
    public void AddCoins(int amount)
    {
        gold += amount;
    }

    public void AddCharacter(Character character)
    {
        character.character = characterPrefabData.GetPrefabById(character.id);
        character.icon = characterPrefabData.GetImageById(character.id);
        characters.Add(character);
    }
    public void AddRune(Rune rune)
    {
        rune.runeData=runeMasterData.runeMasterData[rune.id];
        runes.Add(rune);
    }
    public Character GetCharacterById(int id)
    {
        foreach (var character in characters)
        {
            if (character.id == id)
            {
                return character;
            }
        }
        return null;  
    }
}

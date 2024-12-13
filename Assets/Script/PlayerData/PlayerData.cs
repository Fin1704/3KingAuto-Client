using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/PlayerData")]
public class PlayerData : ScriptableObject
{
    public bool is_get;
    public string userName;
    public int gold;
    public int gem;
    public List<Character> characters;
    public CharacterPrefabData characterPrefabData;
    private void OnEnable()
    {
        is_get = false;
        userName = string.Empty; // Chuỗi trống
        gold = 0;
        gem = 0;
        characters = new List<Character>(); // Tạo danh sách mới
    }

    public void ResetData()
    {
        gold = 0;
        gem = 0;
        userName = "Unknown";
        characters.Clear();
    }
    public void AddCoins(int amount)
    {
        gold += amount;
    }

    public void AddCharacter(Character character)
    {
        character.characterPrefab = characterPrefabData.GetPrefabById(character.id);
        character.characterImage = characterPrefabData.GetImageById(character.id);
        characters.Add(character);
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
        return null;  // Nếu không tìm thấy
    }
}

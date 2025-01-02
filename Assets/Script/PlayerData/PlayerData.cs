using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/PlayerData")]
public class PlayerData : ScriptableObject
{
    public bool is_get;
    public string userName;
    public int gold;
    public int gem;
 public string lastDailyReward;
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
        lastDailyReward = string.Empty;
        characters = new List<Character>();
        runes = new List<Rune>();
    }

    public void ResetData()
    {
        gold = 0;
        gem = 0;
        userName = "Unknown";
        lastDailyReward = string.Empty;
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
    public void UpdateRewardDate()
    {
          DateTime today = DateTime.Today;

        // Chuyển thành chuỗi định dạng "yyyy-MM-dd"
        string formattedDate = today.ToString("yyyy-MM-dd");
        lastDailyReward = formattedDate;
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
    public bool UseGem(int cost){
        if(gem>=cost){
            gem-=cost;
            return true;
        }
        return false;
    }
    public void SetGold(int amount){
        gold=amount;
    }
     
     public bool UseGold(int cost){
        if(gold>=cost){
            gold-=cost;
            return true;
        }
        return false;
    }
    public Character GetBonusRune(){
        Character character = new Character();
        foreach (var rune in runes)
        { 
            if(rune.isEquipped==false) continue;
            if(rune.runeData.Type==RuneType.hp){
                character.hp+=rune.runeData.power;
            }else if(rune.runeData.Type==RuneType.attackMin){
                character.attackMin+=rune.runeData.power;
            }else if(rune.runeData.Type==RuneType.attackMax){
                character.attackMax+=rune.runeData.power;
            }else if(rune.runeData.Type==RuneType.defense){
                character.defense+=rune.runeData.power;
            }else if(rune.runeData.Type==RuneType.attackSpeed){
                character.attackSpeed+=rune.runeData.power;
            }else if(rune.runeData.Type==RuneType.moveSpeed){
                character.moveSpeed+=rune.runeData.power;
            }
        }
        return character;
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Character Data", order = 1)]
public class CharacterData : ScriptableObject
{
    public Character[] characters;

   
}

[System.Serializable]
public class Character
{
    public bool isBattle = false;
    public int id;
    public string name;
    public int level;
    public int hp;
    public int attackMin;
    public int attackMax;
    public int defense;
    public float attackSpeed;
    public float moveSpeed;
    public GameObject characterPrefab;
    public GameObject characterImage;
}

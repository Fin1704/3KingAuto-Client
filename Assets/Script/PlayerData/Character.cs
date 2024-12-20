using UnityEngine;



[System.Serializable]
public class Character
{
    public bool isBattle = false;
    public int id;
    public int exp;
    public string name;
    public int level;
    public int hp;
    public int attackMin;
    public int attackMax;
    public int defense;
    public float attackSpeed;
    public float moveSpeed;
    public GameObject character;
    public GameObject icon;
}

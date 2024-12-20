using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Character Data", order = 1)]
public class CharacterData : ScriptableObject
{
    public int id;
    public GameObject character;
    public GameObject icon;
}
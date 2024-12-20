using UnityEngine;

[CreateAssetMenu(fileName = "New Rune", menuName = "Rune System/Rune")]
public class RuneData : ScriptableObject
{
    public int id;
    public string Name;
    public int Level;
    public RuneType Type;
    public int power;
    public Sprite icon;
    public string description;
}

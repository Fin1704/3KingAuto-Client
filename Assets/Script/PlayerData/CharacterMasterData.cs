using AYellowpaper.SerializedCollections;
using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterPrefabData", menuName = "Game/Character Prefab Data", order = 1)]
public class CharacterMasterData : ScriptableObject
{
    [SerializedDictionary("ID", "Value")]
    public SerializedDictionary<int, CharacterData> characterMasterData;

 
    public GameObject GetPrefabById(int id)
    {
        return characterMasterData[id].character;
    }
     public GameObject GetImageById(int id)
    {
         return characterMasterData[id].icon;
    }
}

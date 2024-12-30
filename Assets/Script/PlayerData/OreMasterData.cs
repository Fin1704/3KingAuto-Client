using AYellowpaper.SerializedCollections;
using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "OrePrefabData", menuName = "Game/Ore Prefab Data", order = 1)]
public class OreMasterData : ScriptableObject
{
    [SerializedDictionary("ID", "Value")]
    public SerializedDictionary<int, OreData> oreMasterData;

 
    public OreData GetPrefabById(int id)
    
    {
        return oreMasterData[id];
    }
}

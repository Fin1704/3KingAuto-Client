using AYellowpaper.SerializedCollections;
using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "RuneMasterData", menuName = "Game/Rune Master Data", order = 1)]
public class RuneMasterData : ScriptableObject
{
    [SerializedDictionary("ID", "Value")]
    public SerializedDictionary<int, RuneData> runeMasterData;

     public Sprite GetImageById(int id)
    {
         return runeMasterData[id].icon;
    }
}

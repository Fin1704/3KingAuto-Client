using Spine.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterPrefabData", menuName = "Game/Character Prefab Data", order = 1)]
public class CharacterPrefabData : ScriptableObject
{
    public CharacterPrefab[] characterPrefabs; // Danh sách các ID và prefab

    [System.Serializable]
    public class CharacterPrefab
    {
        public int id;                
        public GameObject prefab;     
        public GameObject image;
    }

    public GameObject GetPrefabById(int id)
    {
        foreach (var characterPrefab in characterPrefabs)
        {
            if (characterPrefab.id == id)
            {
                return characterPrefab.prefab;
            }
        }
        return null; 
    }
     public GameObject GetImageById(int id)
    {
        foreach (var characterPrefab in characterPrefabs)
        {
            if (characterPrefab.id == id)
            {
                return characterPrefab.image;
            }
        }
        return null; 
    }
}

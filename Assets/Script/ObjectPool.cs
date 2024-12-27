// Add to your GameManager or dedicated ObjectPool class
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private static Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    public static GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        string key = prefab.name;
        
        if (!poolDictionary.ContainsKey(key))
            poolDictionary[key] = new Queue<GameObject>();

        GameObject obj;
        if (poolDictionary[key].Count == 0)
        {
            obj = Instantiate(prefab);
            obj.name = key;
        }
        else
        {
            obj = poolDictionary[key].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }

        if (parent) obj.transform.SetParent(parent);
        return obj;
    }

    public static void ReturnToPool(GameObject obj)
    {
        string key = obj.name;
        obj.SetActive(false);
        poolDictionary[key].Enqueue(obj);
    }
}

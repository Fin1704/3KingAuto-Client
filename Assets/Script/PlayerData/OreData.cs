using UnityEngine;

[CreateAssetMenu(fileName = "OreData", menuName = "ScriptableObjects/Ore", order = 1)]
public class OreData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public int id; // ID của mỏ quặng
    public string oreName; // Tên của mỏ quặng
    public Sprite sprite; // Hình ảnh đại diện của mỏ quặng

    [Header("Thông tin giá trị")]
    public int sellingPrice; // Giá bán của mỏ quặng
    
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopGUI : MonoBehaviour
{
    public Button button;
    public GameObject GUI;

private void Start()
    {
        button.onClick.AddListener(openGUI);
     
        GUI.SetActive(false);
       
    }
      private void openGUI()
    {
                EventManager.FireEvent("OnOpenUI", "ShopGUI");

        GUI.SetActive(!GUI.activeSelf);
    }
}

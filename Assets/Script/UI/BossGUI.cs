using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossGUI : MonoBehaviour
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
        GUI.SetActive(!GUI.activeSelf);
    }
}

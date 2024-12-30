using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MinerGUI : MonoBehaviour
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
                EventManager.FireEvent("OnOpenUI", "MinerGUI");

        GUI.SetActive(!GUI.activeSelf);
    }
}


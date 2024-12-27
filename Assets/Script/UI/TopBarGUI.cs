using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopBarGUI : MonoBehaviour
{
    public PlayerData playerData;
    public TMP_Text gold_text;
    public TMP_Text gem_text;
    void Start()
    {
    }


    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        gold_text.text = playerData.gold.ToString();
    }

}
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour{
    public GameObject winPanel;
    public Button closeWinBtn;

    public GameObject losePanel;
    public Button closeLoseBtn;
    public RuneMasterData runeMasterData;
    public Image spriteRune;
    private bool isHaveBoss = false;    
   private void OnEnable()
    {
        EventManager.StartListening("OnBossManagerStart", OnBossManagerStart);
        EventManager.StartListening("OnBossManagerEnd", OnBossManagerEnd);
        EventManager.StartListening("OnBossManagerTimeUp", OnBossManagerTimeUp);


    }

   public void Awake()
    {
        closeLoseBtn.onClick.AddListener(() =>
        {
            losePanel.SetActive(false);
        });
        closeWinBtn.onClick.AddListener(() =>
        {
            winPanel.SetActive(false);
        });
    }

    private void OnDisable()
    {
        EventManager.StopListening("OnBossManagerEnd", OnBossManagerEnd);
    }

    private void OnBossManagerStart(object[] obj)
    {
      int countdown = (int)obj[0];
      isHaveBoss=true;
    }
     private void OnBossManagerEnd(object[] obj)

    {
       
       
         int idRune = (int)obj[0];
         spriteRune.sprite = runeMasterData.GetImageById(idRune);
          winPanel.SetActive(true);
           EventManager.FireEvent("OnBossDeath");
            isHaveBoss=false;
    }
     private void OnBossManagerTimeUp(object[] obj)
    {
        if (isHaveBoss){
            EventManager.FireEvent("OnBossDeath");
            losePanel.SetActive(true);
            isHaveBoss=false;
        }
        
    }
}
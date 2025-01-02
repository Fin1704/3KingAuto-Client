using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject _BossGUI;
    public GameObject _HeroGUI;
    public GameObject _RuneGUI;
    public GameObject _ShopGUI;
    public GameObject _RankGUI;
    public GameObject _MinerGUI;
    public GameObject _SevenLoginGUI;

    void OnEnable()
    {
        EventManager.StartListening("OnOpenUI", OnOpenUI);
    }

    private void OnOpenUI(object[] parameters)
    {

        string nameUI = (string)parameters[0];
        if (nameUI == "BossGUI")
        {
            _HeroGUI.SetActive(false);
            _RuneGUI.SetActive(false);
            _ShopGUI.SetActive(false);
            _RankGUI.SetActive(false);
            _MinerGUI.SetActive(false);
            _SevenLoginGUI.SetActive(false);

        }
        else if (nameUI == "HeroGUI")
        {
            _BossGUI.SetActive(false);
            _RuneGUI.SetActive(false);
            _ShopGUI.SetActive(false);
            _RankGUI.SetActive(false);
            _MinerGUI.SetActive(false);
            _SevenLoginGUI.SetActive(false);



        }
        else if (nameUI == "RuneGUI")
        {
            _BossGUI.SetActive(false);
            _HeroGUI.SetActive(false);
            _ShopGUI.SetActive(false);
            _RankGUI.SetActive(false);
            _MinerGUI.SetActive(false);
            _SevenLoginGUI.SetActive(false);



        }
        else if (nameUI == "ShopGUI")
        {
            _BossGUI.SetActive(false);
            _HeroGUI.SetActive(false);
            _RuneGUI.SetActive(false);
            _RankGUI.SetActive(false);
            _MinerGUI.SetActive(false);
            _SevenLoginGUI.SetActive(false);



        }
        else if (nameUI == "MinerGUI")
        {
            _BossGUI.SetActive(false);
            _HeroGUI.SetActive(false);
            _RuneGUI.SetActive(false);
            _ShopGUI.SetActive(false);
            _RankGUI.SetActive(false);
            _SevenLoginGUI.SetActive(false);

        }
        else if (nameUI == "SevenLoginGUI")
        {
            _BossGUI.SetActive(false);
            _HeroGUI.SetActive(false);
            _RuneGUI.SetActive(false);
            _ShopGUI.SetActive(false);
            _MinerGUI.SetActive(false);
            _RankGUI.SetActive(false);


        }
    }

    void OnDisable()
    {
        EventManager.StopListening("OnOpenUI", OnOpenUI);
    }

}
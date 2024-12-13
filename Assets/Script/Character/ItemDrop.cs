using UnityEngine;
using System.Collections;

public class ItemDrop : MonoBehaviour
{
    public int coinAmount = 10;
    public float autoPickUpTime = 1f;
    public PlayerData playerData;

    private void Start()
    {
        StartCoroutine(AutoPickUp());
    }

    private IEnumerator AutoPickUp()
    {
        yield return new WaitForSeconds(autoPickUpTime);
        playerData.AddCoins(coinAmount);
        Destroy(gameObject);

    }





}

using UnityEngine;
using System.Collections;

public class ItemDrop : MonoBehaviour
{
    public int coinAmount = 10;
    public float autoPickUpTime = 1f;

    private void Start()
    {
        StartCoroutine(AutoPickUp());
    }

    private IEnumerator AutoPickUp()
    {
        yield return new WaitForSeconds(autoPickUpTime);
        Destroy(gameObject);

    }





}

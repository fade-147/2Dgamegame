using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoss1 : MonoBehaviour
{
    public GameObject Boss;
    public GameObject BossUI;
    public GameObject BossBing;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Boss.SetActive(true);
            BossUI.SetActive(true);
            Destroy(this);
        }
    }
}

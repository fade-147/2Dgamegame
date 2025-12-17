using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossContinueChengbao : MonoBehaviour
{
    public GameObject BossFinish1;
    public GameObject BossFinish2;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BossFinish1.SetActive(true);
            BossFinish2.SetActive(true);
            Destroy(this.gameObject);
        }
    }
}

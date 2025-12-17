using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;

public class BossContinue : MonoBehaviour
{
    public GameObject BossBounds;
    public Boss2Rabbit22 Rabbit;
    private bool StartBoss=false;

    private void Awake()
    {
        Rabbit = GetComponent<Boss2Rabbit22>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")&&!StartBoss )
        {
            BossBounds.SetActive(true);
            StartBoss = true;
        }
    }

    private void Update()
    {
        if (Rabbit == null)
        {
            Rabbit = GetComponent<Boss2Rabbit22>();
        }
        if (Rabbit == null) return;
        if (Rabbit.currentHealth < 2f)
        {
            BossBounds.SetActive(false);
        }
    }
}

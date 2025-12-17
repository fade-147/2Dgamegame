using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChongwuStart : MonoBehaviour
{
    public GameObject BossBound1;
    public GameObject BossBound2;
    public Chongwu1 Chongwu;
    private bool StartBoss = false;

    private void Awake()
    {
        Chongwu = GetComponent<Chongwu1>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !StartBoss)
        {
            BossBound1.SetActive(true);
            BossBound2.SetActive(true);
            StartBoss = true;
        }
    }

    private void Update()
    {
        if (Chongwu == null)
        {
            Chongwu = GetComponent<Chongwu1>();
        }
        if (Chongwu == null) return;
        if (Chongwu.currentHealth < 2f)
        {
            BossBound1.SetActive(false);
            BossBound2.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFinish : MonoBehaviour
{
    private Boss2Rabbit Rabbit;
    public GameObject Bound;
    private void Awake()
    {
        Rabbit = GetComponent<Boss2Rabbit>();
    }
    private void Update()
    {
        if (Rabbit.currentHealth < 10)
        {
            Bound.SetActive(false);
        }
    }
}


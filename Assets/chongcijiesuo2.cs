using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chongcijiesuo2 : MonoBehaviour
{
    public chongcijiesuo chongci;
    private jiesuojineng jiesuo;
    public float dashdashSpeed = 10f;

    private void Awake()
    {
        jiesuo = GetComponent<jiesuojineng>();
    }

    private void Update()
    {
        if (jiesuo.jiesuoFinish)
        {
            chongci.dashSpeed = dashdashSpeed;
        }
    }
}

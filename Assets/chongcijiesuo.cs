using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chongcijiesuo : MonoBehaviour
{
    public Chongci chongci;
    private jiesuojineng jiesuo;
    public float dashSpeed = 9f;

    private void Awake()
    {
        jiesuo =GetComponent <jiesuojineng>();
    }

    private void Update()
    {
        if (jiesuo.jiesuoFinish)
        {
            chongci.DashSpeed = dashSpeed;
        }
    }
}

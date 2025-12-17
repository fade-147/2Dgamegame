using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wudijiesuo : MonoBehaviour
{
    private jiesuojineng jiesuo;
    public wudijinshen jinshen;

    private void Awake()
    {
        jiesuo=GetComponent <jiesuojineng>();
    }

    private void Update()
    {
        if (jiesuo.jiesuoFinish)
        {
            jinshen.wudiMaxTime = 3f;
        }
    }
}

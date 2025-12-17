using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class feidanjiesuo1 : MonoBehaviour
{
    public skillManager feidan;
    private jiesuojineng jiesuo;
    public int feidanNewCount = 6;

    private void Awake()
    {
        jiesuo=GetComponent<jiesuojineng>();
    }
    private void Update()
    {
        if (jiesuo.jiesuoFinish)
        {
            feidan.feidanCount = feidanNewCount;
        }
    }
}

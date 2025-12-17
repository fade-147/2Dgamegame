using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class feidanjiesuo3 : MonoBehaviour
{
    public feidanjiesuo1  feidan;
    private jiesuojineng jiesuo;
    public int feidanNewNewCount=6;

    private void Awake()
    {
        jiesuo = GetComponent<jiesuojineng>();
    }
    private void Update()
    {
        if (jiesuo.jiesuoFinish)
        {
            feidan.feidanNewCount = feidanNewNewCount;
        }
    }
}

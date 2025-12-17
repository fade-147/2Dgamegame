using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class feidanjiesuo2 : MonoBehaviour
{
    public feidanjiesuo3 feidan;
    private jiesuojineng jiesuo;

    private void Awake()
    {
        jiesuo = GetComponent<jiesuojineng>();
    }
    private void Update()
    {
        if (jiesuo.jiesuoFinish)
        {
            feidan.feidanNewNewCount = 8;
        }
    }
}

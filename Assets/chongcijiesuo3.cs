using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chongcijiesuo3 : MonoBehaviour
{
    public Chongci chongci;
    private jiesuojineng jiesuo;

    private void Awake()
    {
        jiesuo = GetComponent<jiesuojineng>();
    }

    private void Update()
    {
        if (jiesuo.jiesuoFinish)
        {
            chongci.ChongLengque = 0.8f;
        }
    }
}

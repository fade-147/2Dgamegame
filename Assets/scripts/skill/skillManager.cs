using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.UI;


public class skillManager : MonoBehaviour
{
    public bool jiesuo = false;

    private Animator anim;
    public PlayInputControl inputControl;
    public GameObject zidan;
    public Transform tran;
    public AudioSource feidan;
    public  float skillCD = 2;
    public Image feidanUI;

    public int feidanCount = 4;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        inputControl = new PlayInputControl();
       
        inputControl.gamePlayer.a8feidan.started += Feidan;
        feidanUI.fillAmount = 1;
        skillCD = 2;
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void Update()
    {

        if (skillCD > 0 && jiesuo)
        {
            skillCD-= Time.deltaTime;
            if(skillCD < 0)
            {
                skillCD = 0;
            }
            feidanUI.fillAmount = skillCD/2;
        }
    }

    private void Feidan(InputAction.CallbackContext context)
    {
        if (skillCD == 0 && jiesuo)
        {
            Debug.Log("aa");
            //zidan.GetComponent<zidan>().trackingTransform = GameObject.FindWithTag("Enemy").transform;
            anim.SetTrigger("feidan");
            feidan.Play();
            //yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < feidanCount; i++)
            {    //向八个方向发射飞弹
                float d = 45 * i;
                float r = (Mathf.PI / 180) * d;
                float x = (float)Mathf.Cos(r);
                float y = (float)Mathf.Sin(r);
                Instantiate(zidan, tran.position + new Vector3(x, y, 0) * 0.5f, Quaternion.Euler(0, 0, d));  //以不同的角度发射预制体
            }
            skillCD = 2;
        }
    }
}

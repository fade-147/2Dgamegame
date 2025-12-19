using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;


public class Chongci : MonoBehaviour
{
    public float currentPower;
    public float maxPower=100f;
    public Image NengliangUI;

    public float DashSpeed=50f;
    public float dashTime;
    public float startDashTimer;
    public float ChongLengque=1f;
    public float chongLengqueTime;
    public Vector2 originzevelocity;
    public bool canchongci=true;

    public Image ChongciUI;
    public bool isDashing;
    public GameObject dashObj;
    public Rigidbody2D rb;
    public PlayInputControl inputControl;
    public Transform playTransform;
    public mouse mou;
    public PyhsicsCheck Grounding;

    public int HuifuPower=4;
    private void Awake()
    {
        currentPower = maxPower;
        inputControl = new PlayInputControl();
        inputControl.gamePlayer.Chongci.started += skillchongci;
    }


    private void OnEnable()
    {
        inputControl.Enable();
    }
    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void FixedUpdate()
    {
        if(currentPower <maxPower)
        {
            currentPower += Time.deltaTime* HuifuPower;
            NengliangUI.fillAmount = currentPower / maxPower;
        }

        if (isDashing)
        {
           
            if (startDashTimer > 0)
            {
                    startDashTimer -= Time.deltaTime;
                if (canchongci && currentPower >=10f)    //等待冲刺结束
                {
                    //rb.velocity = new Vector2(playTransform.localScale.x * DashSpeed, 0);
                    //rb.AddForce(mou.direction * DashSpeed, ForceMode2D.Impulse);
                    originzevelocity = rb.velocity;         //储存冲刺前的速度
                    rb.velocity =mou.direction* DashSpeed;
                    canchongci =false;        //防止冲刺时再次冲刺

                    currentPower -= 20f;      
                    NengliangUI.fillAmount = currentPower / maxPower;  //更改能量条UI
                }
            }
            else       
            {
                
                chongLengqueTime = ChongLengque;   
               rb.velocity =new Vector2 (rb.velocity.x,1);
                isDashing = false;
               // rb.velocity = originzevelocity;    //冲刺结束后，还原速度
            }
        }
        else
        {
            if (chongLengqueTime  > 0)
            {
                chongLengqueTime -= Time.deltaTime;
                if (chongLengqueTime < 0 || Grounding.isGround)
                {
                    chongLengqueTime = 0;
                    canchongci = true;
                }
                ChongciUI.fillAmount = chongLengqueTime / ChongLengque;     //更新技能图标
            }
            
        }
    }

    private IEnumerator FadeCoroutine()
    {
        yield return new WaitForSeconds(0.3f);
        dashObj.SetActive(false);
    }

    private void skillchongci(InputAction.CallbackContext context)
    {
        dashObj .SetActive(true);     //启动残影特效
        if (chongLengqueTime <=0)
        {
            isDashing = true;
        }
        startDashTimer = dashTime;
        StartCoroutine(FadeCoroutine());
    }
   

}

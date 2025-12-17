using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class wudijinshen : MonoBehaviour
{
    public SpriteRenderer playerSprite;
    public PlayInputControl inputControl;
    public Image jinshenUI;
    public Character character;

    public bool jiesuo=false;

    [Header("无敌时间")]
    public float lengqueMaxTime=6f;
    public float lengqueCorrentTime;
    public float wudiMaxTime=2f;
    public float wudiCorrentTime;
    private Color originalColor; // 角色原始颜色
    private bool isInvincible = false; // 是否处于无敌状态（防止重复触发）
    public GameObject jinseLight;
    private void Awake()
    {
        lengqueCorrentTime = lengqueMaxTime;
        // 保存角色的原始颜色（避免恢复时颜色错误）
        playerSprite.GetComponent<SpriteRenderer>();
        originalColor = playerSprite.color;
        inputControl = new PlayInputControl();
        inputControl.gamePlayer.wudijinshen.started += skilljinshen;
        jinshenUI.fillAmount = 1;
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
        if (!jiesuo) return;

        if(wudiCorrentTime  > 0)
        {
            wudiCorrentTime -= Time.deltaTime;
            character.invulnerable = true;
        }
        else if (lengqueCorrentTime > 0)
        {
            jinseLight .SetActive(false);
            isInvincible = false;
            playerSprite.color = originalColor;
            if (wudiCorrentTime  < 0)
            {
                wudiCorrentTime = 0;
            }

            lengqueCorrentTime -= Time.deltaTime;
            if (lengqueCorrentTime < 0)
            {
                lengqueCorrentTime = 0;

            }
            jinshenUI.fillAmount = lengqueCorrentTime / lengqueMaxTime;     //更新技能图标
        }
    }
    private void skilljinshen(InputAction.CallbackContext context)
    {
        if (lengqueCorrentTime == 0 && !isInvincible && jiesuo)
        {
           wudiCorrentTime = wudiMaxTime;
            isInvincible = true;
            lengqueCorrentTime = lengqueMaxTime;
            playerSprite.color = new Color(1f, 1f, 0f);
            jinseLight.SetActive (true);
        }
    }
}

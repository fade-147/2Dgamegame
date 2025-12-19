using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class gan : MonoBehaviour
{
    public PlayInputControl inputControl;
    public GameObject player;
    public Transform firePoint;
    public float zidanSpeed=12f;
    public bool isfashe;
    public mouse mou;
    public Rigidbody2D rb;
    public float ganTime;
    public float ganMaxTime = 0.8f;
    public bool useGan=false;
    public float zidanNumMax=5;
    public float zidanNum;
    public Image ZidanUI;

    GameObject currentWeapon;   //记录下现在使用的武器
    GameObject currentzidan;
    public GameObject gongjian2;
    public GameObject normalzidan;
    public GameObject gongjian;
    public GameObject gongjianjian;
    public GameObject cazicazi;
    public GameObject cazicazicazi;
    public GameObject leishen;
    public GameObject leishenzhichui;

    public  bool haveRanged;
    public  bool isGong2Equipped;
    public  bool isGongjianEquipped;
    public  bool isCaziEquipped;
    public  bool isLeiShenEquipped;
    public GameObject ZidanUIUI;
    private void Awake()
    {
        zidanNum = zidanNumMax;
        ganTime = ganMaxTime;
        inputControl = new PlayInputControl();
        inputControl.gamePlayer.gan.canceled += outgan;
        inputControl.gamePlayer.mouseAttack.started += fashe;
        inputControl.gamePlayer.mouseAttack.canceled += outfashe;
    }
    private void Start()
    {
        if (gongjian2 != null)
            gongjian2.SetActive(false);
        if (gongjian != null)
            gongjian.SetActive(false);
        if (cazicazi != null)
            cazicazi.SetActive(false);
        if (leishen != null)
            leishen.SetActive(false);

        // 订阅武器变化事件（确保InventoryManager已初始化）
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.OnWeaponChanged += OnWeaponChanged;
            // 初始检查一次当前装备（避免启动时未触发事件）
            OnWeaponChanged(
                InventoryManager.instance.currentMeleeWeapon,
                InventoryManager.instance.currentRangedWeapon,
                InventoryManager.instance.currentfangyuWeapon
            );
        }
    }
    // 武器变化时的回调
    private void OnWeaponChanged(Item currentMelee, Item currentRanged,Item currentFangyu)
    {
        if(currentRanged == null)
        {
            haveRanged = false;
        }
        else
        {
            haveRanged = true;
        }
        ZidanUIUI.SetActive(haveRanged);
        Debug.Log("aaaaaaaa");
            // 检查当前远程武器是否为“Qiang”（名称匹配）
            isGong2Equipped = currentRanged != null && currentRanged.itemName == "gongjian2";
            isGongjianEquipped = currentRanged != null && currentRanged.itemName == "gongjian1";
            isCaziEquipped = currentRanged != null && currentRanged.itemName == "cazicazi";
            isLeiShenEquipped = currentRanged != null && currentRanged.itemName == "雷神之锤";

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
        float ganValue= inputControl.gamePlayer.gan.ReadValue<float>();

        if(ganValue > 0.1f)
        {
            isgan();
        }

        if(ganTime >= 0)
        {
            ganTime-= Time.deltaTime;
        }
    }
    private void fashe(InputAction.CallbackContext context)
    {
        isfashe = true;
    }
    private void outfashe(InputAction.CallbackContext context)
    {
        isfashe=false;
    }
    private void isgan()
    {
        currentWeapon = null;
        useGan = true;
            // 核心：计算子弹需要旋转的角度（度）
            float angle = Mathf.Atan2(mou.direction.y, mou.direction.x) * Mathf.Rad2Deg;
        if (!haveRanged)
        {
            return;
        }
        if (isGong2Equipped)
        {
           gongjian2 .SetActive(true);
           currentWeapon = gongjian2;
            currentzidan = normalzidan;
            angle -= 45f;
        }
        else if(isGongjianEquipped)
        {
            gongjian.SetActive(true);
            currentWeapon = gongjian;
            currentzidan = gongjianjian;
            angle -= 45f;
        }
        else if(isCaziEquipped)
        {
            cazicazi.SetActive(true);
            currentWeapon = cazicazi;
            currentzidan = cazicazicazi;
            angle -= 45f;
        }
        else if(isLeiShenEquipped)
        {
            leishen.SetActive(true);
            currentWeapon = leishen;
            currentzidan = leishenzhichui;
            angle -= 45f;
        }
        
         bool isPlayerFacingLeft = (player.transform.localScale.x < 0); // 根据你的角色翻转逻辑

        if (isPlayerFacingLeft)
        {
            // 角色面朝左时，枪械需要额外的180度旋转补偿
            angle -= 90f;
            // 同时，根据鼠标的左右方向调整枪械精灵的垂直翻转，以确保视觉正确
           // currentWeapon.transform.localScale = new Vector3(currentWeapon.transform.localScale.x, (mou.direction.x < 0) ? Mathf.Abs(currentWeapon.transform.localScale.y) : Mathf.Abs(currentWeapon.transform.localScale.y), currentWeapon.transform.localScale.z);
        }
        else
        {
            // 角色面朝右时，逻辑相对简单，只根据鼠标方向调整翻转
          //  currentWeapon.transform.localScale = new Vector3(currentWeapon.transform.localScale.x, (mou.direction.x < 0) ? Mathf.Abs(currentWeapon.transform.localScale.y) : Mathf.Abs(currentWeapon.transform.localScale.y), currentWeapon.transform.localScale.z);
        }
        // 应用旋转
        currentWeapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);





        if (isfashe && ganTime <=0&&zidanNum>=1)
        {
            zidanNum--;
            ZidanUI.fillAmount = zidanNum / zidanNumMax;
            if (isPlayerFacingLeft)
            {
                angle -= 90f;
            }
            ganTime = ganMaxTime;
            // 使用计算出的角度创建一个旋转值
            Quaternion bulletRotation = Quaternion.Euler(0f, 0f, angle);


            // 实例化子弹，并应用计算好的旋转
            GameObject bulletInstance = Instantiate(currentzidan, firePoint.position, bulletRotation);
           // GameObject bullet = Instantiate(normalzidan, firePoint.position, bulletRotation);
            Rigidbody2D bulletRb=bulletInstance .GetComponent<Rigidbody2D>();
            bulletRb.velocity = mou.direction * zidanSpeed;
            isfashe = false;
            Destroy(bulletInstance , 3f);
        }
    }
    private void outgan(InputAction.CallbackContext context)
    {
        useGan=false;
        if(currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class mouse : MonoBehaviour
{
    //private Vector3 mousePosition;
    //public bool isAttacking;
    //private Animator animator;

    //// 用于存储鼠标位置与角色位置的方向
    //private Vector2 direction;

    //// 声明攻击特效和动画参数
    //public GameObject slashEffect; // 刀光特效预制体
    //private string attackAnimParameter = "AttackDirection"; // 动画参数

    //private void Start()
    //{
    //    // 获取Animator组件
    //    animator = GetComponent<Animator>();
    //}

    //private void Update()
    //{
    //    // 获取鼠标位置并转换为世界坐标[1,2](@ref)
    //    mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    mousePosition.z = 0; // 确保z坐标为0[3](@ref)

    //    // 计算方向向量[4](@ref)
    //    direction = (mousePosition - transform.position).normalized;

    //    // 检测鼠标点击[7](@ref)
    //    if (Input.GetMouseButtonDown(0) && !isAttacking) // 左键点击且未在攻击中
    //    {
    //        // 触发攻击
    //        DetermineAttackDirection();
    //    }
    //}

    //private void DetermineAttackDirection()
    //{
    //    isAttacking = true;

    //    // 根据鼠标在角色上方或下方设置动画参数[11](@ref)
    //    // 你也可以使用Trigger来控制动画切换[9,11](@ref)
    //    if (direction.y > 0)
    //    {
    //        // 鼠标在角色上方，播放攻击动画1
    //        animator.SetTrigger("mouseattackUP");
    //    }
    //    else
    //    {
    //        // 鼠标在角色下方，播放攻击动画2
    //        animator.SetTrigger("mouseattackDOWN");
    //    }
    //}

    //// 这个函数将在动画的特定帧被调用（通过Animation Event）
    //public void SpawnSlashEffect()
    //{
    //    if (slashEffect != null)
    //    {
    //        // 实例化刀光特效
    //        GameObject effect = Instantiate(slashEffect, transform.position, Quaternion.identity);

    //        // 计算刀光朝向鼠标的角度[1,2](@ref)
    //        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //        effect.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    //        // 特效播放完成后销毁
    //        Destroy(effect, 0.5f); // 根据特效实际长度调整时间
    //    }

    //    // 重置攻击状态
    //    isAttacking = false;
    //    animator.SetInteger(attackAnimParameter, 0);
    //}


    GameObject daoEffect;
    public GameObject slashEffect1;
    public GameObject slashEffect2;
    private Animator animator;
    private Vector2 mousePosition;
    public Vector2 direction;
    public PlayInputControl inputControl;
    public bool canAttack=true;
    public Transform attackPoint;
    private gan ganuseing;

    private bool haveDao;
    private bool isbaojian1;
    private bool ishuoyanjian;

    private void Start()
    {
        ganuseing = GetComponent<gan>();
        animator = GetComponent<Animator>();
        canAttack = true;
        

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
    private void OnWeaponChanged(Item currentMelee, Item currentRanged, Item currentFangyu)
    {
        if (currentMelee == null)
        {
            haveDao = false;
        }
        else
        {
            haveDao = true;
        }
        //Debug.Log("aaaaaaaa");
        // 检查当前远程武器是否为“Qiang”（名称匹配）
        isbaojian1 = currentMelee != null && currentMelee.itemName == "宝剑1";
        ishuoyanjian = currentMelee != null && currentMelee.itemName == "火焰剑";


    }
    private void Awake()
    {
        inputControl = new PlayInputControl();
        inputControl.gamePlayer.mouseAttack.started += OnAttack;
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
        // 使用新的Input System获取鼠标位置
        mousePosition = Mouse.current.position.ReadValue();

        // 将屏幕坐标转换为世界坐标
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane)
        );
        worldPosition.z = 0;

        // 计算方向
         direction = (worldPosition - transform.position).normalized;

    }

    // 处理鼠标点击
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (canAttack && !ganuseing.useGan && haveDao)
        {
            //根据装备的武器来判断刀光类型
            if (isbaojian1)
            {
                daoEffect = slashEffect1;
            }else if (ishuoyanjian)
            {
                daoEffect= slashEffect2;
            }



           // 根据方向判断上下攻击
           if (direction.y > 0)
            {
                // 鼠标在角色上方 - 播放攻击动画1
                animator.SetTrigger("mouseattackUP");
            }
            else
            {
                animator.SetTrigger("mouseattackDOWN");
            }
    
           if(transform.localScale.x< 0&&direction.x>0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            if (transform.localScale.x > 0 && direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            Vector3 spawnPosition=attackPoint .position;
              // 实例化刀光特效
              GameObject effect = Instantiate(daoEffect ,spawnPosition , Quaternion.identity);
              // 计算刀光朝向鼠标的角度[1,2](@ref)
              float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
              angle -= 90f;
        
              effect.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

              // 特效播放完成后销毁
               Destroy(effect, 0.5f); // 根据特效实际长度调整时间
              canAttack = false;

        }
    }
}


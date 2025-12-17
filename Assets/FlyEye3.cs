using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class FlyEye3 : Enemy
{
    public Image xietiao;
    private Character character;
    private Transform enemyTran;

    [Header("玩家与检测参数")]
    public Transform player; 
    public float detectionRange = 7f; // 射线最大长度
    public LayerMask playerLayer; 
    private float originalScaleX;  

    [Header("攻击相关配置")]
    public Animator enemyAnim;
    public float attackRange = 5f; // 攻击触发距离
    public float attackInterval = 1.5f; // 攻击冷却时间
    private bool isAttacking = false; // 核心控制：是否正在攻击
    private float lastAttackTime; // 上次攻击时间

    [Header("移动控制")]
    public float stopDistance = 5f; // 靠近玩家停止移动的距离和attackRange一致

    [Header("毒刺攻击配置")]
    public Transform stingerSpawnPoint; // 毒刺发射点
    public GameObject stingerPrefab; // 毒刺预制体
    public float stingerSpeed = 8f; // 毒刺飞行速度
    public float recoilForce = 3f; // 后坐力大小（向后的力）
    public float recoilDuration = 0.5f; // 后坐力持续时间
    public float forwardForce = 4f; // 后坐力后向前的力

    [Header("空中巡逻配置")]
    public float patrolSpeed = 1.5f; // 巡逻移动速度
    public float patrolRangeX = 4f; // 左右巡逻的最大距离
    private Vector3 initialPatrolPos; // 巡逻初始位置
    private int patrolDir = 1; // 巡逻方向

    [Header("受击击退配置")]
    public float knockbackDamping = 5f;
    public float knockbackDuration = 1f;
    private bool isKnockedBack = false;
    private float knockbackEndTime;

    protected override void Awake()
    {
        base.Awake();
        patrolState = new BoarPatrolState();
        chaseState = new BoarChaseState();
        character = GetComponent<Character>();
        enemyTran = character.GetComponent<Transform>();
        enemyAnim = GetComponent<Animator>();
        pyhsicsCheck.YesCheckGround = false;

        // 记录敌人初始的X轴缩放
        originalScaleX = transform.localScale.x;

        // 记录空中巡逻位置
        initialPatrolPos = transform.position;
        // 飞行敌人取消重力
        rb.gravityScale = 0; 
    }

    protected override void Update()
    {
        base.Update();
        xietiao.fillAmount = character.currentHealth / character.maxHealth;
        if (enemyTran.localScale.x < 0)      //根据敌人的面朝方向调整头顶血条的翻转
        {
            xietiao.transform.localScale = new Vector3((float)-0.0046, (float)0.0052, (float)0.0052);
        }
        else //if (rb.velocity.x >= 0)
        {

            xietiao.transform.localScale = new Vector3((float)0.0046, (float)0.0052, (float)0.0052);
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            return;
        }
        // 击退状态下，优先处理刹车逻辑
        if (isKnockedBack)
        {
            HandleKnockbackDamping();
            if (Time.time >= knockbackEndTime)
            {
                isKnockedBack = false; // 结束击退状态
                    
            }
            return; // 击退时跳过所有其他逻辑
        }

        // 攻击状态下，跳过所有检测/翻转逻辑
        if (isAttacking || player == null) return;

        //发射射线检测玩家
        bool isPlayerVisible = CheckPlayerVisibility();
        // 计算与玩家的距离，判断是否进入攻击范围
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
       
        
        bool isInAttackRange = distanceToPlayer <= attackRange && isPlayerVisible;

        if (isInAttackRange)
        {
            // 进入攻击范围：停止移动 + 执行攻击
            Attack();
        }
        else if (isPlayerVisible)
        {
            // 可见但未到攻击范围(要追击了)：正常翻转朝向
            FlipTowardsPlayer();
        }
        else
        {
            // 未检测到玩家时，左右折返巡逻
            UpdatePatrolDirection();
        }
    }


    public override void Move()
    {
        // 攻击时直接停止移动，不执行任何移动逻辑
        if (isAttacking || Time.time - lastAttackTime < attackInterval || player == null)
        {
             
            enemyAnim.SetBool("walk", false);
            enemyAnim.SetBool("run", false);

            return;
        }


        // 如果射线检测到玩家就追击
        bool isPlayerVisible = CheckPlayerVisibility();
        if (isPlayerVisible)
        {
            // 先判断是否到停止距离，避免靠近后仍移动
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer > stopDistance)
            {
                Vector2 chaseDir = (player.position - transform.position).normalized;
                rb.velocity = new Vector2(chaseDir.x * currentSpeed * 0.1f, chaseDir.y * currentSpeed * 0.1f);  //最后面的常数是追击速度
            }
            else
            {
                // 到达停止距离：停止X轴移动。开始攻击
                rb.velocity = new Vector2(0, 0);
            }
        }
        else
        {
            //未检测到玩家，就执行空中巡逻
            rb.velocity = new Vector2(patrolDir * patrolSpeed, 0);
        }
    }

    void UpdatePatrolDirection()
    {
        // 超出左边界：向右走
        if (transform.position.x < initialPatrolPos.x - patrolRangeX)
        {
            patrolDir = 1;
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z); // 朝右
        }
        // 超出右边界：向左走
        else if (transform.position.x > initialPatrolPos.x + patrolRangeX)
        {
            patrolDir = -1;
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z); // 朝左
        }
    }
    // 重写父类的方法（覆盖原有水平击退）
    public override void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        // 适配默认朝右的翻转逻辑（保留原有缩放逻辑，可根据需求调整）
        if (attackTrans.position.x - transform.position.x > 0)
        {
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (attackTrans.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);
        }

        //受伤状态 + 动画
        isHurt = true;
        anim.SetTrigger("hurt");

        //计算“朝角色反方向”的二维击退方向（含X+Y轴）
        Vector2 dir = (transform.position - attackTrans.position).normalized; // 敌人位置 - 攻击者位置 = 反方向

        // 调用带刹车的击退逻辑（替换原有AddForce）
        TakeKnockback(dir, hurtForce); // hurtForce是父类的击退力度字段

        // 执行受伤后逻辑（无需协程，刹车逻辑已在Update中处理）
        hitps.Play();
        StartCoroutine(ResetHurtState());
    }
    IEnumerator ResetHurtState()
    {
        yield return new WaitForSeconds(0.5f);
        isHurt = false;      // 重置受伤状态
    }
    public void TakeKnockback(Vector2 knockbackDir, float knockbackForce)
    {
        isKnockedBack = true;
        knockbackEndTime = Time.time + knockbackDuration;
        rb.velocity = knockbackDir * knockbackForce; // 二维方向击退
        isAttacking = false;            //击退时强制终止攻击，避免干扰速度
    }

    /// <summary>
    /// 处理击退阻尼（刹车逻辑）
    /// </summary>
    void HandleKnockbackDamping()
    {
        float speedThreshold = 0.1f;
        if (rb.velocity.magnitude < speedThreshold)
        {
            rb.velocity = Vector2.zero; // 直接清零
        }
        else
        {
            //改用Lerp衰减（比乘子更稳定），同时衰减Y轴
            float dampingStep = knockbackDamping * Time.deltaTime;
            dampingStep = Mathf.Clamp01(dampingStep); 
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, dampingStep);
            // 额外衰减Y轴
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Lerp(rb.velocity.y, 0, dampingStep * 0.5f));
        }
    }

    bool CheckPlayerVisibility()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        // 射线起点,敌人头部
        Vector2 rayOrigin = (Vector2)transform.position;
        //获取玩家的“身体中心”
        Vector2 playerBodyPos;
        // 优先用玩家的碰撞体中心
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerBodyPos = playerCollider.bounds.center; 
        }
        else
        {

            playerBodyPos = (Vector2)player.position + Vector2.up * 1f;
        }


        Vector2 direction = (playerBodyPos - rayOrigin).normalized; // 目标点改为身体中心
        // 发射射线
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, detectionRange, playerLayer);

        // 判断结果：若击中玩家且无遮挡
        if (hit.collider != null)
        {

            if (hit.collider.CompareTag("Player"))
            {
                Debug.DrawRay(rayOrigin, direction * detectionRange, Color.green,0.1f); 
                return true;
            }
            else // 击中障碍物，视线被挡
            {
                Debug.DrawRay(rayOrigin, direction * hit.distance, Color.red, 0.1f); 
                return false;
            }
        }

        Debug.DrawRay(rayOrigin, direction * detectionRange, Color.yellow, 0.1f); 
        return false;
    }

    void FlipTowardsPlayer()
    {
        // 计算敌人到玩家的X轴方向差
        float directionX = player.position.x - transform.position.x;


        float bufferDistance = 1.5f; // 缓冲距离

        // 只有当距离超过缓冲范围时，才翻转朝向
        if (directionX > bufferDistance)
        {
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (directionX < -bufferDistance)
        {
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);
        }
        // 距离在缓冲范围内时，不修改朝向
    }


    void Attack()
    {
        // 冷却时间未到或正在攻击中不执行攻击
        if (Time.time - lastAttackTime < attackInterval || isAttacking) return;

        // 标记为攻击状态（禁止移动/转头）
        isAttacking = true;
 
        // 更新攻击冷却时间
        lastAttackTime = Time.time;
        // 执行后坐力+发射毒刺逻辑（通过协程实现分步效果）


        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        //向后的后坐力
        Vector2 shootDirection = (player.position - stingerSpawnPoint.position).normalized;
        rb.velocity = -shootDirection * recoilForce;
        yield return new WaitForSeconds(recoilDuration);

        // 发射毒刺
        SpawnStinger();

        // 向前回弹
        rb.velocity = shootDirection * forwardForce;
        yield return new WaitForSeconds(recoilDuration);

        // 重置速度（避免持续移动）
        rb.velocity = new Vector2(0, 0);

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    /// <summary>
    /// 生成毒刺并发射
    /// </summary>
    void SpawnStinger()
    {
        if (stingerPrefab == null || stingerSpawnPoint == null) return;


        Vector2 playerBodyPos;
        // 用玩家的碰撞体中心
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerBodyPos = playerCollider.bounds.center; 
        }
        else
        {

            playerBodyPos = (Vector2)player.position + Vector2.up * 1f;
        }
        // 计算从发射点到玩家的方向向量（归一化，确保速度一致）
        Vector2 shootDirection = (playerBodyPos - (Vector2 )stingerSpawnPoint.position).normalized;

        //计算发射角度
        float shootAngle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        //实例化毒刺（设置旋转，让毒刺朝向飞行方向）
        GameObject stinger = Instantiate(
            stingerPrefab,
            stingerSpawnPoint.position,
            Quaternion.Euler(0, 0, shootAngle) 
        );

        // 给毒刺施加朝向玩家的速度
        Rigidbody2D stingerRb = stinger.GetComponent<Rigidbody2D>();
        stingerRb.velocity = shootDirection * stingerSpeed;

        // 销毁毒刺
        Destroy(stinger, 1.5f);
    }

}
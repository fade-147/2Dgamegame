using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyEye2 : Enemy
{
    public Image xietiao;
    private Character character;
    private Transform enemyTran;

    [Header("玩家与检测参数")]
    public Transform player; 
    public float detectionRange = 5f; 
    public LayerMask playerLayer; 
    private float originalScaleX;  // 存储原始X缩放

    [Header("攻击相关配置")]
    private Animator enemyAnim; 
    public float attackRange = 1f; 
    public float attackInterval = 1f; // 攻击冷却时间
    public string attackTriggerName = "Attack"; // 攻击动画Trigger名称
    private bool isAttacking = false; // 核心控制：是否正在攻击
    private float lastAttackTime; // 上次攻击时间（冷却用）

    [Header("移动控制")]
    public float stopDistance = 1f; // 靠近玩家停止移动的距离和attackRange一致


    protected override void Awake()
    {
        base.Awake();
        patrolState = new BoarPatrolState();
        chaseState = new BoarChaseState();
        character = GetComponent<Character>();
        enemyTran = character.GetComponent<Transform>();
        enemyAnim = GetComponent<Animator>();


        // 记录敌人初始的X轴缩放（用于后续翻转）
        originalScaleX = transform.localScale.x;
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

        // 攻击状态下，跳过所有检测/翻转逻辑
        if (isAttacking|| player == null) return;

        // 发射射线检测玩家
        bool isPlayerVisible = CheckPlayerVisibility();

        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
        bool isInAttackRange = distanceToPlayer <= attackRange && isPlayerVisible;

        if (isInAttackRange)
        {
            // 进入攻击范围：停止移动 + 执行攻击
            Attack();
        }
        else if (isPlayerVisible)
        {
            // 可见但未到攻击范围：正常翻转朝向
            FlipTowardsPlayer();
        }
    }


    public override void Move()
    {
        // 攻击时直接停止移动，不执行任何移动逻辑
        if (isAttacking || Time.time - lastAttackTime < attackInterval || player == null)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(0, rb.velocity.y); // 仅保留Y轴速度
            enemyAnim.SetBool("walk", false);
            enemyAnim.SetBool("run", false);

            return;
        }


        // 如果射线检测到玩家就追击
        bool isPlayerVisible = CheckPlayerVisibility();
        if (isPlayerVisible)
        {
            // 先判断是否到停止距离，避免靠近后仍移动
            float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);
            if (distanceToPlayer > stopDistance)
            {
                float faceDirX = Mathf.Sign(transform.localScale.x);
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                rb.velocity = new Vector2((float)(currentSpeed * faceDirX * 0.15), rb.velocity.y);
            }
            else
            {
                // 到达停止距离：停止X轴移动。开始攻击
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        else
        {
            base.Move();
        }
    }

    bool CheckPlayerVisibility()
    {
        // 射线起点：敌人头部
        Vector2 rayOrigin = (Vector2)transform.position + Vector2.up * 0.5f;
        // 射线方向：指向玩家
        Vector2 direction = (player.position - transform.position).normalized;
        // 发射射线
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, detectionRange, playerLayer);

        // 若击中玩家且无遮挡（或击中玩家且优先级最高）
        if (hit.collider != null)
        {
            // 检测击中的是否是玩家（通过标签或层）
            if (hit.collider.CompareTag("Player"))
            {
                Debug.DrawRay(rayOrigin, direction * detectionRange, Color.green); // 调试用绿线
                return true;
            }
            else // 击中障碍物，视线被挡
            {
                Debug.DrawRay(rayOrigin, direction * hit.distance, Color.red); // 调试用红线
                return false;
            }
        }

        Debug.DrawRay(rayOrigin, direction * detectionRange, Color.yellow); // 未击中任何物体（黄线）
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
        // 触发攻击动画
        enemyAnim.SetTrigger("attack");
        enemyAnim.SetBool("walk", false);
        enemyAnim.SetBool("run", false);

        // 更新攻击冷却时间
        lastAttackTime = Time.time;
    }

    public void FinishAttack()
    {
        isAttacking =false;
    }
}
    




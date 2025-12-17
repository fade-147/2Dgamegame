using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum PetState
{
    FollowPlayer,  // 跟随玩家
    AttackEnemy,   // 攻击敌人
    Teleporting    // 传送中（不可操作）
}
public class Chongwu1 : MonoBehaviour
{
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    public Image ChongwuUI;
    [Header("受伤无敌")]
    public float invulnerableDuration;
    private float invulnerableCounter;
    public bool invulnerable;
    private bool isHuifu;

    [Tooltip("玩家Transform（拖拽赋值）")]
    public Transform player;
    [Tooltip("宠物移动速度")]
    public float moveSpeed = 5f;
    [Tooltip("跳跃力")]
    public float jumpForce = 15f;
    [Tooltip("攻击范围（距离敌人多近触发攻击）")]
    public float attackRange = 1f;
    [Tooltip("跟随最大距离（超过则强制跟随）")]
    public float maxFollowDistance = 8f;
    [Tooltip("跟随最小距离（避免贴脸）")]
    public float minFollowDistance = 1.5f;
    [Tooltip("墙体检测距离（前方多远检测墙）")]
    public float wallCheckDistance = 0.5f;
    [Tooltip("跳不过墙后传送延迟（秒）")]
    public float teleportDelay = 3f;
    [Tooltip("攻击冷却（秒）")]
    public float attackCooldown = 1f;

    public bool isAttack=false;
    
    public PyhsicsCheck pyGround;  

    [Header("检测层")]
    public LayerMask enemyLayer;    // 敌人层
    public LayerMask groundLayer;   // 地面层
    public LayerMask wallLayer;     // 墙体层

    // 私有变量
    public Animator wuqi;    
    private Rigidbody2D rb;
    private Animator anim;
    private PetState currentState;
    private Transform targetEnemy;  // 目标敌人
    private float lastAttackTime;   // 上次攻击时间
    private bool isJumpingOverWall; // 是否正在跳墙
    private float wallBlockTime;    // 被墙阻挡的时间
    private float lastDistanceToPlayer;
    private float lastTeleportTime; // 记录上次传送的时间
    public float teleportCooldown = 1f; // 1秒内只能传送1次

    [Header("跟随缓冲（解决抽搐）")]
    public float followLagDistance = 2f; // 落后多少距离才移动（你要的2f）
    public float moveSmoothTime = 0.1f; // 移动平滑时间（越小越灵敏，越大越顺滑）
    private Vector2 currentVelocity; // 平滑移动用的临时变量

    void Awake()
    {
        // 获取组件
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pyGround = GetComponent<PyhsicsCheck>();

        //初始化血条血量
        currentHealth = maxHealth;
        ChongwuUI.fillAmount = 1;
        // 初始状态：跟随玩家
        currentState = PetState.FollowPlayer;
        //初始化上一帧，作为距离玩家距离的上一帧
        lastDistanceToPlayer = Vector2.Distance(transform.position, player.position);
    }

    void Update()
    {
        if (invulnerable)    //无敌时间计数
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }

        if(currentHealth<maxHealth)   //每秒自动回血
        {
            currentHealth += Time.deltaTime;
            ChongwuUI.fillAmount = currentHealth / maxHealth;
        }
        else
        {
            currentHealth = maxHealth;
            isHuifu = false;
            ChongwuUI.color = new Color(1, 1, 1);
        }

        // 传送中不执行任何逻辑
        if (currentState == PetState.Teleporting) return;

        //记录时间，传送冷却
        bool canTeleport = Time.time - lastTeleportTime > teleportCooldown;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > 9f && canTeleport)
        {
            lastTeleportTime = Time.time; // 立即记录时间，启动冷却
            StartCoroutine(TeleportToPlayerBehind());
        }
        else if (pyGround.touchLeftWall && pyGround.touchRightWall && canTeleport)
        {
            lastTeleportTime = Time.time; // 立即记录时间，启动冷却
            StartCoroutine(TeleportToPlayerBehind());  //说明卡墙了，直接传送
        }

        // 1. 优先检测敌人（攻击优先级 > 跟随）
        FindClosestEnemy();
        
        // 2. 状态切换逻辑
        if (targetEnemy != null && Vector2.Distance(transform.position, targetEnemy.position) <= maxFollowDistance&&!isHuifu)
        {
            currentState = PetState.AttackEnemy; // 有敌人则攻击
        }
        else
        {
            currentState = PetState.FollowPlayer; // 无敌人则跟随
        }

        // 3. 执行对应状态的行为
        switch (currentState)
        {
            case PetState.AttackEnemy:
                AttackEnemyBehavior();
                break;
            case PetState.FollowPlayer:
                FollowPlayerBehavior();
                break;
        }

        // 更新动画参数
        UpdateAnimator();
    }

    /// <summary>
    /// 寻找最近的敌人
    /// </summary>
    void FindClosestEnemy()
    {
        // 检测范围：比最大跟随距离稍大，确保优先攻击
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, maxFollowDistance + 2f, enemyLayer);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        targetEnemy = closestEnemy;
    }

    /// <summary>
    /// 攻击敌人的行为
    /// </summary>
    void AttackEnemyBehavior()
    {
        if (targetEnemy == null&&isAttack ) return;

        // 计算朝向敌人的方向
        Vector2 direction = (targetEnemy.position - transform.position).normalized;
        direction.y = 0; // 只在X轴移动（2D地面）

        // 距离敌人超过攻击范围 → 移动过去
        if (Vector2.Distance(transform.position, targetEnemy.position) > attackRange)
        {
            MoveTowards(direction);
            // 检测前方是否有墙，有则跳跃
            CheckWallAndJump();
        }
        // 进入攻击范围 → 触发攻击
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // 停止移动
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }

        // 翻转宠物朝向
        FlipSprite(direction.x);
    }

    /// <summary>
    /// 跟随玩家的行为
    /// </summary>
    void FollowPlayerBehavior()
    {
        if (player == null) return;

        // 1. 计算核心距离：宠物到玩家的水平距离（忽略Y轴，避免上下波动影响）
        float horizontalDistance = Mathf.Abs(player.position.x - transform.position.x);
        // 2. 计算“触发移动的阈值”：最小跟随距离 + 滞后缓冲（2f）
        float moveTriggerDistance = minFollowDistance + followLagDistance;

        // 3. 只有“落后超过2f”时才移动，否则平稳停止
        if (horizontalDistance > moveTriggerDistance)
        {
            // 计算朝向玩家的目标的水平方向
            Vector2 targetDirection = new Vector2(player.position.x - transform.position.x, 0).normalized;
            // 平滑移动（核心：用Vector2.SmoothDamp避免瞬间启停）
            Vector2 targetVelocity = targetDirection * moveSpeed;
            rb.velocity = new Vector2(
                Mathf.SmoothDamp(rb.velocity.x, targetVelocity.x, ref currentVelocity.x, moveSmoothTime),
                rb.velocity.y
            );

            // 检测墙并跳跃 + 翻转朝向
            CheckWallAndJump();
            FlipSprite(targetDirection.x);
        }
        else
        {
            // 缓冲范围内：平稳减速到停止（而非瞬间置0）
            rb.velocity = new Vector2(
                Mathf.SmoothDamp(rb.velocity.x, 0, ref currentVelocity.x, moveSmoothTime),
                rb.velocity.y
            );
        }

        // 检测是否被墙阻挡且跳不过 → 计时传送
        CheckWallBlockAndTeleport();
    }

    /// <summary>
    /// 移动逻辑
    /// </summary>
    void MoveTowards(Vector2 direction)
    {
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }

    /// <summary>
    /// 检测前方墙体并跳跃
    /// </summary>
    void CheckWallAndJump()
    {


        // 2. 核心判断：与玩家的距离是否在变远（替代原有的“朝墙移动”）
        float currentDistanceToPlayer = Vector2.Distance(transform.position, player.position);
        // 加0.01f阈值，避免微小位置波动误触发
        bool isDistanceToPlayerIncreasing = currentDistanceToPlayer - lastDistanceToPlayer > 0.01f;
        //如果距离玩家过远，并且前面有墙，也会触发跳跃
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // 满足条件则跳跃
        if ((isDistanceToPlayerIncreasing|| distanceToPlayer>6f) && (pyGround.touchLeftWall || pyGround.touchRightWall) && pyGround.isGround && !isJumpingOverWall)
        {
            Debug.Log("触发跳跃！");
            rb.velocity = new Vector2(rb.velocity.x * 0.5f, jumpForce); // 保留少量水平速度，跳得更远
            isJumpingOverWall = true;
            Invoke(nameof(ResetJumpState), 0.8f);
        }

        // 更新上一帧距离
        lastDistanceToPlayer = currentDistanceToPlayer;
    }

    /// <summary>
    /// 检测被墙阻挡并触发传送
    /// </summary>
    void CheckWallBlockAndTeleport()
    {
        
        // 只要碰墙且在地面，就计时
        bool isBlockedByWall = (pyGround .touchLeftWall || pyGround.touchRightWall) && pyGround.isGround;
        if (isBlockedByWall)
        {
            wallBlockTime += Time.deltaTime;
            if (wallBlockTime >= teleportDelay)
            {
                StartCoroutine(TeleportToPlayerBehind());
                wallBlockTime = 0;
            }
        }
        else
        {
            wallBlockTime = 0;
        }
    }

    /// <summary>
    /// 传送到玩家身后
    /// </summary>
    IEnumerator TeleportToPlayerBehind()
    {
        currentState = PetState.Teleporting;
        rb.velocity = new Vector2(0, 0);
        //  anim.SetTrigger("Teleport"); // 可添加传送动画
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(0.5f);
        // 计算玩家身后的位置（偏移2个单位）
        Vector3 playerBehindPos = player.position - player.right;
        playerBehindPos.y = player.position.y + 1f;     //这里需要向上一段距离，不然会卡进墙里
        transform.position = playerBehindPos;
        //显示
        GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        currentState = PetState.FollowPlayer;
    }

    /// <summary>
    /// 攻击逻辑（可扩展：添加伤害、动画、音效）
    /// </summary>
    void Attack()
    {
        anim.SetTrigger("Attack");
        wuqi.SetTrigger("Attack");
        isAttack = true;
    }

    
    // 翻转朝向
    
    void FlipSprite(float directionX)
    {
        if (directionX != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(directionX), 1, 1);
        }
    }

    
    // 重置跳墙状态
    
    void ResetJumpState()
    {
        isJumpingOverWall = false;
    }

  
    // 更新动画参数
   
    void UpdateAnimator()
    {
        anim.SetBool("IsMoving", Mathf.Abs(rb.velocity.x) > 1f);
      //  anim.SetBool("IsJumping", !Physics2D.OverlapCircle(transform.position - new Vector3(0, 0.6f, 0), 0.2f, groundLayer));
     //   anim.SetBool("IsAttacking", currentState == PetState.AttackEnemy && Vector2.Distance(transform.position, targetEnemy.position) <= attackRange);
    }

    /// <summary>
    /// Gizmos绘制检测范围（调试用）
    /// </summary>
    void OnDrawGizmos()
    {
        // 攻击范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        // 跟随范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxFollowDistance);
    }

    public void TakeDamage(Attack attacker)
    {
        if (invulnerable||isHuifu )
        {
            return;
        }
        if (currentHealth == 0) return;

        if (currentHealth - attacker.damage > 0)
        {
            anim.SetTrigger("Hurt");
            currentHealth -= attacker.damage;
            if (!invulnerable)
            {
                invulnerable = true;
                invulnerableCounter = invulnerableDuration;
            }
            ChongwuUI.fillAmount = currentHealth / maxHealth;
        }
        else
        {
            currentHealth = 0;   //触发死亡
            isHuifu = true;
            ChongwuUI.color = new Color(1, 0, 1);
        }
    }
}

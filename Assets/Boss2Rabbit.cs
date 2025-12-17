using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public enum BossAttackType
{
    Attack1 = 1,
    Attack2 = 2,
    Attack3 = 3,
    Attack4 = 4
}
public class Boss2Rabbit : MonoBehaviour
{
    [Header("基础配置")]
    public Animator bossAnimator; // 拖拽赋值BOSS的Animator
    public Rigidbody2D rb;
    private PyhsicsCheck pyCheck;
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    [Header("受伤无敌")]
    public float invulnerableDuration;
    private float invulnerableCounter;
    public bool invulnerable;
    private bool isEnrage = false;

    [Header("冲刺攻击配置（Attack4）")]
    public float dashSpeed = 25f; // 冲刺速度（越大越快）
    public float dashDuration = 2f; // 冲刺持续时间（秒）
    public LayerMask playerLayer; // 玩家层（用于锁定冲刺方向）
    public Transform playerTransform; // 玩家位置引用
    private Vector2 dashDirection; // 冲刺方向
    public float idleInterval = 2f; // 每隔2秒选一次攻击

    public ParticleSystem hiitps;

    [Header("炮弹配置")]
    public GameObject bulletPrefab; // 拖拽赋值炮弹预制体
    public GameObject bulletsmall;
    public float bulletSpawnOffset = 5f; // 炮弹在BOSS身后的偏移距离（可调，比如1f）
    public float bulletSpeed = 5f; // 炮弹移动速度

    private bool isAttacking = false; // 标记是否在攻击（避免重复触发）
    private Coroutine attackSelectCoroutine; // 控制定时的协程
    private Coroutine dashCoroutine; // 冲刺协程（单独控制，避免冲突）

    public Image BossUI;
    public GameObject BossUIObj;
    private PickupSpawner pickupSpawner;   //掉落道具脚本的引用
    private bool everPickUp = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bossAnimator = GetComponent<Animator>();
        pyCheck = GetComponent<PyhsicsCheck>();
        currentHealth = maxHealth;

        // 启动Idle状态的攻击选择循环
        StartAttackSelectLoop();
    }
    private void Awake()
    {
        pickupSpawner = GetComponent<PickupSpawner>();
    }
    private void Update()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindWithTag("Player")?.transform;
        }
        bossAnimator.SetFloat("velocityY", rb.velocity.y);

        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }
        BossUI.fillAmount = currentHealth / maxHealth;
    }

    // 启动“每隔2秒选攻击”的协程
    void StartAttackSelectLoop()
    {
        if (attackSelectCoroutine != null)
            StopCoroutine(attackSelectCoroutine);

        attackSelectCoroutine = StartCoroutine(AttackSelectTimer());
    }

    // 核心协程：每隔2秒生成1~10随机数，选攻击
    IEnumerator AttackSelectTimer()
    {
        while (true)
        {
            // 只有Idle状态（非攻击）才等待+选攻击
            if (!isAttacking)
            {
                yield return new WaitForSeconds(idleInterval); // 等2秒
                SelectAttackByRandomNum(); // 选攻击
            }
            else
            {
                yield return null; // 攻击中不操作，帧等
            }
        }
    }
    public void TurnToPlayer()
    {
        // 计算BOSS与玩家的水平距离（仅X轴，2D横向转身）
        float playerX = playerTransform.position.x;
        float bossX = transform.position.x;

        // 玩家在左侧 → 面朝左
        if (playerX < bossX - 0.1f) // 减0.1避免微小偏移反复转身
        {
            transform.localScale = new Vector3(-1, 1, 1); // X轴翻转
        }
        // 玩家在右侧 → 面朝右
        else if (playerX > bossX + 0.1f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        // 玩家在正前方（误差范围内）→ 不转身
    }

    // 其他原有方法（AttackSelectTimer、SelectAttackByRandomNum等）保留

    // 生成1~10随机数，按区间选攻击
    void SelectAttackByRandomNum()
    {
        int randomNum = UnityEngine.Random.Range(1, 11); // 生成1~10（左闭右开，所以max=11）
        BossAttackType selectedAttack = BossAttackType.Attack1;

        // 按数字区间判断攻击类型（直观易懂，想改概率直接调数字）
        if (randomNum <= 3) // 1、2、3 → Attack1
            selectedAttack = BossAttackType.Attack1;
        else if (randomNum <= 5) // 4、5 → Attack2
            selectedAttack = BossAttackType.Attack2;
        else if (randomNum <= 7) // 6、7 → Attack3
            selectedAttack = BossAttackType.Attack3;
        else // 8、9、10 → Attack4
            selectedAttack = BossAttackType.Attack4;
        TurnToPlayer();
        // 切换到选中的攻击状态
        TriggerAttack(selectedAttack);
    }

    // 给Animator传参，触发攻击
    void TriggerAttack(BossAttackType attackType)
    {
        isAttacking = true; // 标记为攻击中，暂停定时
        bossAnimator.SetInteger("AttackType", (int)attackType); // 传攻击类型
        bossAnimator.SetTrigger("DoAttack"); // 触发攻击动画

        if (attackType == BossAttackType.Attack1)
        {
            bossAnimator.SetBool("isGround", false);
        }
        if (attackType == BossAttackType.Attack2)
        {
            // 1. 确定冲刺方向：优先朝向玩家，无玩家则朝BOSS右侧（默认）
            dashDirection = Vector2.right; // 默认方向
                                           // 计算BOSS到玩家的方向（仅水平，如需上下可保留y轴）
            dashDirection = (playerTransform.position - transform.position).normalized;
            dashDirection.y = 0; // 仅水平冲刺，如需上下冲刺可删掉这行


            // 2. 启动冲刺协程（先停止旧的冲刺，避免重复）
            if (dashCoroutine != null) StopCoroutine(dashCoroutine);
            dashCoroutine = StartCoroutine(DashCoroutine());
        }
        if(attackType == BossAttackType.Attack3)
        {
            SpawnBullet();
        }
        if( attackType == BossAttackType.Attack4)
        {
            SpawnThreeBullets();
        }
    }

    // 冲刺协程：控制冲刺的开始/持续/结束
    IEnumerator DashCoroutine()
    {
        yield return new WaitForSeconds(1f);
        float dashTimer = 0f;
        // 冲刺中持续移动
        while (dashTimer < dashDuration)
        {
            dashTimer += Time.deltaTime;
            // 运动学刚体移动：用MovePosition保证匀速
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }
        // 冲刺结束：停止移动
        dashCoroutine = null;

    }

    // 供状态机调用：攻击结束后回到Idle
    public void OnAttackEnd()
    {
        isAttacking = false; // 取消攻击标记
        // 重置Animator参数（避免残留）
        bossAnimator.SetInteger("AttackType", -1);
        bossAnimator.ResetTrigger("DoAttack");
    }
    public void TakeDamage(Attack attacker)
    {

        if (invulnerable)
        {
            return;
        }
        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            //  BossUI.fillAmount = currentHealth / maxHealth;
            //执行受伤

            if (!invulnerable)
            {
                invulnerable = true;
                invulnerableCounter = invulnerableDuration;

            }
        }
        else
        {
            currentHealth = 0;   //触发死亡
            BossUIObj.SetActive(false);
            if(!everPickUp)
            {
                pickupSpawner.DropItems();
                everPickUp = true;
            }
            bossAnimator.SetTrigger("bossDead");
            bossAnimator.Update(0f);
            StartCoroutine(DestroyAfterAnimation());

        }

        if (currentHealth <= maxHealth / 2 && !isEnrage)
        {
            //      GetComponent<Animator>().SetTrigger("isEnrage");
            invulnerable = true;
            invulnerableCounter = 2f;
            isEnrage = true;
        }
    }
    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(2f);
        //   BossxieUI.SetActive(false);
        Destroy(this.gameObject);
    }

    public void hiit()
    {
        hiitps.Play();
    }
    public void SpawnBullet()
    {
        // 获取BOSS当前面朝方向（1=右，-1=左）
        float faceDir = transform.localScale.x > 0 ? 1 : -1;

        // 计算炮弹生成位置：BOSS身后（面朝右→身后是左侧，面朝左→身后是右侧）
        // 偏移逻辑：BOSS位置 + 身后方向的偏移向量（仅水平）
        Vector2 spawnPos = transform.position;
        spawnPos.x += -faceDir * bulletSpawnOffset; // -faceDir 就是“身后”方向

        int randomType = UnityEngine.Random.Range(0, 2); // 生成0或1
        if (randomType == 1) // 随机数为1时，位置偏下2f
        {
            spawnPos.y -= 2f; // Y轴减2f = 偏下方（2D场景Y轴向上，减=下）
        }
        // 生成炮弹（实例化）
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.Euler(0, 0, 90));
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = new Vector2(faceDir * bulletSpeed, 0);      //给炮弹一个速度
    }

    public void SpawnThreeBullets()
    {
        // 计算基础生成位置：玩家正上方8f
        Vector2 playerPos = playerTransform.position;
        Vector2 baseSpawnPos = new Vector2(playerPos.x, playerPos.y + 8f);

        // 定义3个并排子弹的位置（横向间距3f）
        // 左子弹：基准x-3f → 中间子弹：基准x → 右子弹：基准x+3f（间距3f）
        Vector2[] spawnPositions = new Vector2[]
        {
            new Vector2(baseSpawnPos.x - 3f, baseSpawnPos.y), // 左子弹
            new Vector2(baseSpawnPos.x, baseSpawnPos.y),       // 中子弹
            new Vector2(baseSpawnPos.x + 3f, baseSpawnPos.y)  // 右子弹
        };

        // 4. 遍历生成3个子弹，同时向下发射
        foreach (Vector2 spawnPos in spawnPositions)
        {
            // 生成子弹（设置旋转，可根据需求调整）
            GameObject bullet = Instantiate(
                bulletsmall,
                spawnPos,
                Quaternion.Euler(0, 0, 0) // 向下发射建议旋转0度
            );

            // 给子弹绑定“垂直向下移动”逻辑
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                // 核心：垂直向下发射（Vector2.down）
                bulletRb.velocity = Vector2.down * bulletSpeed;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossChongwu : MonoBehaviour
{
    public Transform playertrans;
    public Dialogue dialogue;
    public bool isFlipped = false;
    private PickupSpawner pickupSpawner;   //掉落道具脚本的引用
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    public float moveSpeed = 5f;   //激动速度
    private Rigidbody2D rb;
    public Image BossUI;
    public GameObject BossxieUI;
    private Animator anim;
    [Header("受伤无敌")]
    public float invulnerableDuration;
    private float invulnerableCounter;
    public bool invulnerable;
    private bool isEnrage = false;
    [Header("攻击冷却（秒）")]
    public float attackCooldown = 1f;
    private float lastAttackTime;   // 上次攻击时间
    public float attackRange = 2.5f; //攻击范围
    [Header("事件广播")]
    public VoidEventSO GetChongBlueEvent;

    public Animator wuqi;
    public ParticleSystem hitps;
    private bool everPickUp;
    public bool isAttack;
    private bool StartDialogue=false;
    public bool isEffecting;   //正在进行技能；
    private GameObject Player;
    private Coroutine attackSelectCoroutine; // 控制定时的协程

    //死亡后关闭Boss空气墙
    public GameObject BossBound1;
    public GameObject BossBound2;

    public void Awake()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        rb=GetComponent<Rigidbody2D>();
        pickupSpawner = GetComponent<PickupSpawner>();
        Player =GameObject.FindGameObjectWithTag("Player");
    }
    private void Start()
    {
        // 启动Idle状态的攻击选择循环
        StartAttackSelectLoop();
    }
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
            if (!isEffecting)
            {
                yield return new WaitForSeconds(5f); // 等5秒再次进行选择
                SelectAttackByRandomNum(); // 选攻击
            }
            else
            {
                yield return null; // 攻击中不操作，帧等
            }
        }
    }
    void SelectAttackByRandomNum()
    {
        int randomNum = UnityEngine.Random.Range(1, 3); // 生成1~3（左闭右开)
        
        // 按数字区间判断攻击类型（直观易懂，想改概率直接调数字）
        if (randomNum == 1)
        {
            isEffecting = true;
            anim.SetTrigger("Effecting1");
        }
        else if (randomNum == 2)
        {
            isEffecting = true;
            anim.SetTrigger("Effecting2");
        }
        rb.velocity = new Vector2(0, rb.velocity.y);  //停止移动
        LookAtPlayer();
    }
    public void LookAtPlayer()
    {
        playertrans = GameObject.FindGameObjectWithTag("Player").transform;

        if (transform.position.x > playertrans.position.x && isFlipped)
        {
            transform.localScale = new Vector3((float)-1.76201, (float)1.76201, (float)1.76201);           // transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < playertrans.position.x && !isFlipped)
        {
            transform.localScale = new Vector3((float)1.76201, (float)1.76201, (float)1.76201);
            // transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }
    public void TakeDamage(Attack attacker)
    {
        //if (playercontroller.isfangyu==true)
        //{
        //    OKfangyu = true;
        //}
        //else
        //{
        //    OKfangyu = false;
        //}
        if (invulnerable)
        {
            return;
        }
        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            BossUI.fillAmount = currentHealth / maxHealth;
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
            anim.SetTrigger("bossDead");
            
            if (!everPickUp)
            {
                pickupSpawner.DropItems();
                everPickUp = true;
            }
            BossBound1.SetActive(false);
            BossBound2.SetActive(false);
            if(!StartDialogue)
            {
               DialogueManager.Instance.StartDialogue(dialogue);
                StartDialogue = true;
            }
            anim.Update(0f);
            StartCoroutine(DestroyAfterAnimation());

        }

        if (currentHealth <= maxHealth / 2 && !isEnrage)
        {
            invulnerable = true;
            invulnerableCounter = 2f;
            isEnrage = true;
        }
    }
    private void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }
        anim.SetBool("IsMoving", Mathf.Abs(rb.velocity.x) > 1f);   //更新动画参数
        AttackEnemyBehavior();  //走进玩家进行攻击
    }
    private IEnumerator DestroyAfterAnimation()
    {
        
        
        yield return new WaitForSeconds(1.5f);
        BossxieUI.SetActive(false);
        GetChongBlueEvent.RaiseEvent();
        Destroy(this.gameObject);
    }
    public void hit()
    {
        hitps.Play();
    }

    void AttackEnemyBehavior()
    {
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        if (Player == null || isAttack || isEffecting) return;

        // 计算朝向敌人的方向
        Vector2 direction = (Player.transform .position - transform.position).normalized;
        direction.y = 0; // 只在X轴移动（2D地面）

        // 距离敌人超过攻击范围 → 移动过去
        if (Vector2.Distance(transform.position, Player.transform.position) > attackRange)
        {
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

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
        LookAtPlayer();
    }
    void Attack()
    {
        anim.SetTrigger("Attack");
        wuqi.SetTrigger("Attack");
        isAttack = true;
    }
}

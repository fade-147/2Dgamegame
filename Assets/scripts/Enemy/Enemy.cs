using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
//using static UnityEngine.RuleTile.TilingRuleOutput;

public class Enemy : MonoBehaviour
{
    public Rigidbody2D rb;
    [HideInInspector]public Animator anim;
    [HideInInspector]public PyhsicsCheck pyhsicsCheck;

    [Header("基本参数")]
    public float normalSpeed;
    public float chaseSpeed;
    [HideInInspector] public float currentSpeed;
    public Vector3 faceDir;
    public float hurtForce;
    public Transform attacker;

    [Header("检测")]
    public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("计时器")]
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;
    public float lostTime;
    public float lostTimeCounter;

    [Header("状态")]
    public bool isHurt;
    public bool isDead;
    private BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;


    public ParticleSystem hitps;
    protected virtual void Awake()
    {
        rb=GetComponent<Rigidbody2D>();
        anim=GetComponent<Animator>();
        pyhsicsCheck =GetComponent<PyhsicsCheck>();

        currentSpeed = normalSpeed;
        //waitTimeCounter=waitTime;
    }

    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    protected virtual void Update()
    {
        
        faceDir = new Vector3(transform.localScale.x, 0, 0);

        currentState.LogicUpdate ();
        TimeCounter();

    }

    private void FixedUpdate()
    {
        if(!isHurt &&!isDead && !wait)
         Move();
        currentState .PhysicsUpdate ();
    }

    private void OnDisable()
    {
        currentState .OnExit(); 
    }

    public virtual void Move()
    {
        rb.velocity =new Vector2(currentSpeed *faceDir.x*Time.deltaTime,rb.velocity.y);
    }

    //计时器
    public void TimeCounter()
    {
        if (wait)
        {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0)
            {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(-faceDir.x, 4, 4);
            }
        }

        if (!FoundPlayer() && lostTimeCounter >0)
        {
            lostTimeCounter-=Time .deltaTime;
        }
        //else
        //{
        //    lostTimeCounter = lostTime;
        //}

                
    }

    public bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDir, checkDistance, attackLayer);
        
    }

    public void SwitchState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            _ => null
        };
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);

    }

    public virtual void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        if (attackTrans.position.x - transform.position.x > 0)
        {
           transform.localScale = new Vector3(4, 4, 4);
        }
        if (attackTrans.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(-4, 4, 4);
        }
        //受伤被击退
        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x,0).normalized ;
        rb.velocity = new Vector2(0, rb.velocity.y);
        StartCoroutine(OnHurt(dir));
    }

    IEnumerator OnHurt(Vector2 dir)
    {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        hit();
        yield return new WaitForSeconds(0.5f);
        isHurt = false;

    }

    public void OnDie()
    {
        gameObject.layer = 2;
        anim.SetBool("dead",true);
        isDead = true;
    }

    public void DestroyAfterAnimation()
    {
        Destroy(this.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset+new Vector3(checkDistance *transform.localScale.x,0), 0.2f);
    }

    private void hit()
    {
        hitps.Play();
    }
}

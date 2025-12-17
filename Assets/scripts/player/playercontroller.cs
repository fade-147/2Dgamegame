using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("监听事件")]
    public SceneLoadEvent sceneLoadEvent;
    public VoidEventSO afterSceneLoadedEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;

    public Chongci chongci;
    public PlayInputControl inputControl;
    private Rigidbody2D rb;
    private PyhsicsCheck pyhsicsCheck;
    private PlayerAnimation playerAnimation;
    private Character character;
    public Vector2 inputDirection;
    public CapsuleCollider2D coll;
    [Header("基本参数")]

    public float speed;
    public float jumpForce;
    public float jumpcount;
    public float RollForce;  //滚动的推力
    public float RollDuration = 0f;
    public float RollTime = 0f;
    public Vector3 walloffsetRight;
    public Vector3 walloffsetLeft;



    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;
    public LayerMask wallLayer;
    public ParticleSystem slideParticle;

    [Header("状态")]
    public bool isHurt;
    public float hurtForce;
    public bool isDead;
    public bool isAttack;
    public bool isRoll=false;
    public bool isLeftwall,isRightwall;
    public bool isWallMove;
    public bool isfangyu;


    [Header("发射设置")]
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float fireCooldown = 0.5f;
    public float fireballSpeed = 10f;

    [Header("输入设置")]
    public InputAction fireAction = new InputAction("Fire", InputActionType.Button, "<Keyboard>/u");

    private float lastFireTime = 0f;

    [Header("固定镜头")]
    public bool isInFixedCameraArea = false;
    public Rect fixedCameraBounds; // 接收触发脚本传递的范围
    public enum Wallstate
    {
        wallGrab,    //抓住墙的状态
        wallslide,   //下滑的状态
        wallclimb,  //向上爬的状态
        walljump,    //蹬墙跳
        none
    }

    Wallstate ws;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        pyhsicsCheck = GetComponent<PyhsicsCheck>();
        coll = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        character= GetComponent<Character>();

        inputControl = new PlayInputControl();

        //跳跃
        inputControl.gamePlayer.Jump.started += Jump;

        //攻击
        inputControl.gamePlayer.Attack.started += PlayerAttack;

        //翻滚
        inputControl.gamePlayer.rolling.started += isRolling;

        ////爬墙
        //inputControl.gamePlayer.clbWall .started += isRolling;

        ////防御
        //inputControl.gamePlayer.fangyu.started += isfangyu;
        inputControl.Enable();          //防止复活后还是可以移动
    }

   
    public void Start()
    {
        ws = Wallstate.none;
    }

    private void OnEnable()
    {
        sceneLoadEvent.loadRequestEvent += OnLoadEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;

        fireAction.Enable();
        fireAction.performed += OnFire;
    }

    private void OnDisable()
    {
        inputControl.Disable();
        sceneLoadEvent.loadRequestEvent -= OnLoadEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;

        fireAction.Disable();
        fireAction.performed -= OnFire;
    }


    private void Update()
    {
        inputDirection = inputControl.gamePlayer.Move.ReadValue<Vector2>();

        CheckState();
        WallMove ();
        fangyu();
        WallParrticle();

        //固定镜头
        // 2. 固定镜头时，限制玩家在镜头范围内
        if (isInFixedCameraArea)
        {
            Vector3 clampedPos = transform.position;
            // 限制X轴在镜头左右边界内
            clampedPos.x = Mathf.Clamp(clampedPos.x, fixedCameraBounds.xMin, fixedCameraBounds.xMax);
            // 限制Y轴在镜头上下边界内
            clampedPos.y = Mathf.Clamp(clampedPos.y, fixedCameraBounds.yMin, fixedCameraBounds.yMax);
            transform.position = clampedPos;
        }

    }

    private void FixedUpdate()
    {
        if (!isHurt && !chongci.isDashing)   
        {
            Move();
        }

        //翻滚
        Roll();
    }

    //读取游戏进度
    private void OnLoadDataEvent()
    {
        isDead = false;
    }

    //加载场景，停止控制
    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        inputControl.gamePlayer .Disable ();
    }

    //加载结束，可以控制
    private void OnAfterSceneLoadedEvent()
    {
        inputControl.gamePlayer.Enable();

    }


    public void Move()
    {   //移动
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime,rb.velocity.y);

        int faceDir = (int)transform.localScale.x;

        if (inputDirection.x > 0)
            faceDir = 1;
        if(inputDirection.x < 0)
            faceDir = -1;
        //翻转
        transform.localScale = new Vector3(faceDir, 1, 1);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (pyhsicsCheck.isGround)
        {
            jumpcount = 2;
        }

        if (jumpcount == 1)
        {
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, 0);
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            GetComponent<AudioDefination>()?.PlayAudioClip();
            jumpcount -= 1;
        }

        if (jumpcount==2)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            GetComponent<AudioDefination>()?.PlayAudioClip();
            jumpcount -= 1;
        }

    }

    private void isRolling(InputAction.CallbackContext obj)
    {
        if (!isRoll)
        {
            isRoll = true;
        }
   
     }


    private void PlayerAttack(InputAction.CallbackContext context)
    {
        if (!isRoll)
        {
            playerAnimation.PlayAttack();
            isAttack = true;
        }
    }

    #region UnityEvent
    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;

        rb .AddForce (dir*hurtForce , ForceMode2D.Impulse);
        isRoll = false;
    }

    public void PlayerDead()
    {
        isDead = true;
        inputControl.gamePlayer.Disable();
    }
    #endregion


    private void CheckState()
    {
        coll.sharedMaterial = pyhsicsCheck.isGround ? normal : wall;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        

    }

    public void Roll()
    {

        if (isRoll)
        {
            if (RollTime <= RollDuration)
            {
                //施加推力
                rb.AddForce( new Vector2( transform.localScale.x * RollForce, 0), ForceMode2D.Impulse);

                RollTime += Time.fixedDeltaTime;
            }
            else
            {
                //完成翻滚
                isRoll = false;
                RollTime = 0f;
            }
        }
    }

    public void WallMove()
    {

        bool isPressingclb = inputControl.gamePlayer.clbWall.ReadValue<float > () > 0;
        bool isPressingW = inputControl.gamePlayer.clbMoveW.ReadValue<float>() > 0;
        bool isPressingS = inputControl.gamePlayer.clbMoveS.ReadValue<float>() > 0;
        if (isPressingclb  && (pyhsicsCheck.touchLeftWall || pyhsicsCheck.touchRightWall))
        {
                isWallMove = true;
        }
        else
        {
            isWallMove = false;
        }

        if(isWallMove)
        {
            rb.gravityScale = 0f;

            if (isPressingW)
            {
                Wallclimb();
            }
            else if (isPressingS)
            {
                Wallslide();
            }
            else
            {
                WallGrab();
            }
        }
        else
        {
            ws = Wallstate.none;
            rb.gravityScale = 4f;
        }
    }

    public void WallGrab()
    {
        rb.velocity =Vector2.zero;
        ws = Wallstate.wallGrab;
        if (jumpcount == 0)
        {
            jumpcount += 1;
        }
    }
    
    public void Wallclimb()
    {
        rb.velocity =Vector2 .zero;
        rb.velocity = new Vector2(rb.velocity.x, 5);
        ws = Wallstate.wallclimb;

    }

    public void Wallslide()
    {
        rb.velocity = Vector2.zero;
        rb.velocity = new Vector2(rb.velocity.x,-5);
        ws = Wallstate.wallslide;

    }

    //爬墙粒子效果
    public void WallParrticle()
    {
        var main = slideParticle.main;
        if (ws == Wallstate.wallslide ||ws==Wallstate.wallclimb)
        {
            if (isLeftwall)
            {
                slideParticle .gameObject .transform .position =transform .position - walloffsetLeft;
            }
            else
            {
                slideParticle.gameObject.transform.position = transform.position + walloffsetRight;

            }
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear ;

        }
    }

    private void fangyu()
    {
        bool isPressingfangyu = inputControl.gamePlayer.fangyu.ReadValue<float>() > 0;
        if(isPressingfangyu)
        {
            isfangyu = true;
            character.invulnerable = true;

        }
        else
        {
            isfangyu= false;          
        }


    }


    void OnFire(InputAction.CallbackContext context)
    {
        if (Time.time >= lastFireTime + fireCooldown)
        {
            ShootFireball();
            lastFireTime = Time.time;
        }
    }

    void ShootFireball()
    {
        if (fireballPrefab == null)
        {
            Debug.LogError("火球预制体未分配!");
            return;
        }

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = fireball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = firePoint.forward * fireballSpeed;
        }

        Debug.Log("发射火球!");
    }

    
}



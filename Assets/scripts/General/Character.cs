using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour,ISaveable
{
    [Header("事件监听")]
    public VoidEventSO newGameEvent;

    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;

    [Header("受伤无敌")]
    public float invulnerableDuration;
    private float invulnerableCounter;
    public bool invulnerable;

    public bool OKfangyu;
    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;

    private PickupSpawner pickupSpawner;  //掉落道具脚本
    private NewBehaviourScript playercontroller;

    public bool isPlayer;
    private float DunFangyu;

    public GameObject FireEffect;
    public bool FireStart=false;
    private float FireCurrentTime=0;
    private float FireMaxTime=3f;

    private void Awake()
    {
        pickupSpawner = GetComponent<PickupSpawner>();  //获取掉落道具脚本的引用
    }
    private void Start()
    {
        if (!isPlayer)
        {
            DunFangyu = 0;
            return;
        }
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
    private void OnWeaponChanged(Item currentMelee, Item currentRanged, Item currentFangyu)
    {
        // 检查当前(名称匹配）
        if (currentFangyu == null)
        {
            DunFangyu = 0;
        }else if (currentFangyu != null && currentFangyu.itemName == "金边木盾1")
        {
            DunFangyu = 2;
        }else if (currentFangyu != null && currentFangyu.itemName == "金边木盾2")
        {
            DunFangyu = 4;
        }else if (currentFangyu != null && currentFangyu.itemName == "金包木盾")
        {
            DunFangyu = 6;
        }

    }
    private void NewGame()
    {
        //让开始新游戏的时候，人物的血量和能量都是满的
        currentHealth = maxHealth;
        OnHealthChange?.Invoke(this);
        Debug.Log("okok");
    }

    public void OnEnable()
    {
        //currentHealth = maxHealth;
        newGameEvent.OnEventRaised += NewGame;

        ISaveable saveable = this;
        saveable.RegisterSaveDate();
    }

    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= NewGame;
        ISaveable saveable = this;
        saveable.UnregisterSaveDate();
    }

    private void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter-= Time.deltaTime;
            if(invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }
        if (FireEffect != null)
        {
            if (FireStart)
            {
                FireCurrentTime = FireMaxTime;
                FireEffect.SetActive(true);
                FireStart = false;
            }
            if (FireCurrentTime > 0)
            {
                FireCurrentTime -= Time.deltaTime;
                currentHealth -= Time.deltaTime * 3;
                
            }
            else
            {
                FireCurrentTime = 0;
                FireEffect.SetActive(false);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("keel")){
            //死亡，更新血量
            if(currentHealth > 0)        //防止重复触发
            {
                currentHealth = 0;
                OnHealthChange?.Invoke(this);
                OnDie?.Invoke();
            }
        }
    }

    //拾取回血道具
    public virtual void RestoreHealth(float value)
    {
        if (currentHealth == maxHealth) return;
        if (currentHealth ==0) return;

        if(currentHealth + value > maxHealth)
        {  
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += value;
        }
        OnHealthChange?.Invoke(this);//更新血量
    }


    //传进来bool看是否是近战，放在attack中可能因刀光被销毁而无法还原时间速率
    public void TakeDamage(Attack attacker,bool isjinzhan)     
    {
        if (invulnerable)
        {
            return;
        }
        if (currentHealth == 0) return;

        if (currentHealth - attacker.damage > 0)
        {
            if(attacker.damage - DunFangyu >= 0)
            {
                currentHealth -= (attacker.damage - DunFangyu);
            }
            TriggerInvulnerable();
            //执行受伤
            OnTakeDamage?.Invoke(attacker.transform);
            OnHealthChange?.Invoke(this);
        }
        else
        {
            currentHealth = 0;   //触发死亡
            OnDie?.Invoke();
            pickupSpawner.DropItems();       //掉落道具
            
            OnHealthChange?.Invoke(this);
        }

        if (isjinzhan && currentHealth - attacker.damage >= 0)
        {
            StartCoroutine(TriggerHitStop());
        }
        //if (playercontroller.isfangyu==true)
        //{
        //    OKfangyu = true;
        //}
        //else
        //{
        //    OKfangyu = false;
        //}
    }
    [Header("实现顿帧")]
    public float hitStopDuration = 0.05f;          //顿帧的时间
    public float timeScaleDuringHitStop = 0;
    private bool isHitStopping = false;
    private IEnumerator TriggerHitStop()
    {
        if (isHitStopping) yield break;
        isHitStopping = true;

        Time.timeScale = timeScaleDuringHitStop;
        Time.fixedDeltaTime = 0.01f * Time.timeScale;          //实现顿帧

        yield return new WaitForSecondsRealtime(hitStopDuration);

        Time.timeScale = 1;              //恢复原来的时间
        Time.fixedDeltaTime = 0.02f;

        isHitStopping = false;
    }
    //触发受伤无敌
    private void TriggerInvulnerable()
    {
        if (!invulnerable) {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void GetSaveDate(Data data)        //数据的保存
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            data.characterPosDict[GetDataID().ID] = transform.position;
            data.floatSavedData[GetDataID().ID + "health"] = this.currentHealth;
     //       data.floatSavedData[GetDataID().ID + "power"] = this.currentPower;
        }
        else
        {
            data.characterPosDict.Add(GetDataID().ID, transform.position);
            data.floatSavedData.Add(GetDataID().ID + "health", this.currentHealth);
       //     data.floatSavedData.Add(GetDataID().ID + "power", this.currentPower);
        }
    }

    public void LoadDate(Data data)       
    {
        if (data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            transform.position =data.characterPosDict [GetDataID().ID];
            this.currentHealth =data.floatSavedData [GetDataID().ID+"health"];
            //this.currentPower = data.floatSavedData[GetDataID().ID + "power"];

            //通知UI更新血量和能量 
            OnHealthChange?.Invoke(this);
        }
    }

    public void DestroyAfterAnimation()       //在死亡动画中添加的动画事件
    {
        Destroy(this.gameObject);
    }

   
}

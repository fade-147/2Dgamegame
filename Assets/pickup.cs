using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class pickup : MonoBehaviour
{
    private Character playercharacter;

    [Header("上抛动画")]
    [SerializeField] private float throwHeight = 1f;
    [SerializeField] private float throwDuration=1f;

    [Header("拾取范围")]
    [SerializeField] private float pickUpDistance = 3f;  //自动拾取范围
    [SerializeField] private float moveSpeed = 5f;      //自动拾取速度
    private bool canPickUp = false;    //是否可以拾取

    private ItemOnWord GetGetGet;
 // GameObject playerObject = GameObject.FindWithTag("Player");

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag ("Player");
        if (playerObject != null)
        {
            playercharacter = playerObject.GetComponent<Character>();
        }
        ThrowItem();
        GetGetGet = GetComponent<ItemOnWord>();
    }
    private void Update()
    {
        if (playercharacter == null) return;
        if (canPickUp && Vector2.Distance(transform.position, playercharacter.transform.position) < pickUpDistance)
        {
            Vector2 dir = (playercharacter .transform.position - transform.position).normalized;
            transform.Translate(dir * moveSpeed * Time.deltaTime);
        }
    }

    public enum PickupType
    {
        Coin,
        HealingPotion,
        otherss
    }
    [SerializeField] private PickupType pickupType;
    [SerializeField] private int value;

    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (canPickUp && collision.gameObject.GetComponent<PlayerAnimation >())
        {
            
            CollectPickup();
            if (GetGetGet != null)
            {
                GetGetGet.AddNewItem();
            }
        }
    }

    private void ThrowItem()
    {
        //使用DOtween创建动画
        transform.DOJump(transform.position, throwHeight, 1, throwDuration)
        .OnComplete(() =>
        {
            canPickUp = true;
        });
    }

    //根据不同的类型执行不同的逻辑
    private void CollectPickup()
    {
        switch(pickupType)
        {
            case PickupType.Coin:  //处理金币逻辑
                HandleCoinPickup();
                break;
            case PickupType.HealingPotion:  //处理治疗药水的逻辑
                HandleHealingPotionPickup();
                break;
            case PickupType.otherss:
                break;
        }
        Destroy (gameObject);
    }

    private void HandleCoinPickup()
    {
        //如果是金币，则增加金币的数量
        GameManager.Instance.AddCoins(value);
    }

    private void HandleHealingPotionPickup()
    {
        //如果是血包，则给玩家回血
        playercharacter.RestoreHealth(value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pohuai : MonoBehaviour
{
    [Header("基本属性")]
    public float currentHealth=1f;
    public GameObject deathEffectPrefab;
    public bool ispohuai = false;
    private PickupSpawner pickupSpawner;   //掉落道具脚本的引用
    private void Awake()
    {
        pickupSpawner =GetComponent <PickupSpawner>();
    }

    public void TakeDamage(Attack attacker)
    {
        if (ispohuai) return;
        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            //执行受伤
        }
        else
        {
            ispohuai=true;        //防止多次触发死亡，导致生成多个道具
            currentHealth = 0;   //触发死亡
            if(deathEffectPrefab != null)
            {
                GameObject effect =Instantiate(deathEffectPrefab , transform.position ,transform.rotation );  //破坏后生成特效
               // Destroy ( effect,2f );
            }
            pickupSpawner.DropItems();
            Destroy (gameObject );
        }

    }
}

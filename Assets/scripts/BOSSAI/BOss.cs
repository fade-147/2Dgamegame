using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;


public class BOss : MonoBehaviour
{

    public Transform player;
    public bool isFlipped = false;
    public GameObject BossFinish1;
    public GameObject BossFinish2;

    private PickupSpawner pickupSpawner;   //掉落道具脚本的引用

    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    public Image BossUI;
    public GameObject BossxieUI;
    public Animator anim;
    public GameObject Boss;

    [Header("受伤无敌")]
    public float invulnerableDuration;
    private float invulnerableCounter;
    public bool invulnerable;
    private bool isEnrage=false;

    public ParticleSystem hitps;
    private bool everPickUp;
    public void LookAtPlayer()
    {
        player=GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 flipped = transform.localScale;
        flipped.z = -1f;

        if(transform.position .x>player.position.x&&isFlipped)
        {
            transform .localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }

    public void Awake()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        pickupSpawner = GetComponent<PickupSpawner>();
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
            BossFinish1.SetActive(false);
            BossFinish2.SetActive(false);
            if (!everPickUp)
            {
                pickupSpawner.DropItems();
                everPickUp = true;
            }
            anim.Update(0f);
            StartCoroutine(DestroyAfterAnimation());

        }
        
        if(currentHealth <= maxHealth / 2 &&!isEnrage )
        {
            GetComponent<Animator>().SetTrigger("isEnrage");
            invulnerable = true;
            invulnerableCounter = 2f;
            isEnrage = true;
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        BossxieUI.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        Destroy(Boss);
    }

    public void hit()
    {
        hitps.Play();
    }

   
}

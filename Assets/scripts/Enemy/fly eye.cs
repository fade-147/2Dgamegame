using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class flyeye : Enemy
{
    public Image xietiao;
    private Character character;
    private Transform enemyTran;
    
    protected override void Awake ()
    {
        base.Awake(); 
        patrolState =new BoarPatrolState ();
        chaseState =new BoarChaseState ();
        character =GetComponent <Character> ();
        enemyTran = character.GetComponent<Transform>();
    }

    protected override void Update()
    {
        base .Update();
        xietiao.fillAmount = character.currentHealth / character.maxHealth;
        if (enemyTran.localScale .x < 0)      //根据敌人的面朝方向调整头顶血条的翻转
        {
            xietiao.transform.localScale = new Vector3((float)-0.0046, (float)0.0052, (float)0.0052);
        }
        else //if (rb.velocity.x >= 0)
        {
        
            xietiao.transform.localScale = new Vector3((float)0.0046, (float)0.0052, (float)0.0052);
        }
    }
}



  //public override void Move()
  //  {
  //      base.Move();
  //      anim.SetBool("walk", true);
  //  }


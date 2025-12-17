using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseAttackEffect : MonoBehaviour
{

    public float raiseCountTime=0f;
    public float raiseMaxTime=0.1f;     
    // 引用PlayerController（可拖拽赋值，或代码查找）
    public NewBehaviourScript  playerController;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<NewBehaviourScript>();
        }
    }
    private void Update()    //加冷却防止重复触发
    {
       
        if (raiseCountTime > 0)
        {
            raiseCountTime-= Time.deltaTime;
        }
        if(raiseCountTime < 0)
        {
            raiseCountTime = 0;
        }
    }

    // 触发器碰撞检测（攻击物体的Collider2D是Trigger）
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<NewBehaviourScript>();
        }

        // 仅在攻击状态下，且碰撞对象是“敌人”（通过Tag判断）
        if ( raiseCountTime==0f&& other.CompareTag("Enemy"))
        {
            if (playerController != null)
            {
                // 给PlayerController的jumpcount加一
                playerController.jumpcount += 1;
                raiseCountTime = raiseMaxTime;
              
            }
          
        }
    }

}

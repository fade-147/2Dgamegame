using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;


public class Attack : MonoBehaviour
{
    public int damage;
    public float attackRange;
    public float attackRate;
    public bool isjinzhan;    //标记是否为近战
    public gan Gan;
    public bool zidanjiajia;
    public bool HaveFire;
    public bool HaveEle;
                          
  
    private void OnTriggerStay2D(Collider2D other)
    {
     
        if(other.GetComponent<Character>()!=null)
        {
            other.GetComponent<Character>()?.TakeDamage(this,isjinzhan);
            if (Gan != null&&!zidanjiajia)
            {
                if (Gan.zidanNum < 5)
                {
                    Gan.zidanNum++;
                }
                zidanjiajia = true;
            }
        }

        if (other.GetComponent<BOss>() != null)
        {
            other.GetComponent<BOss>()?.TakeDamage(this);
            if (isjinzhan)
                other.GetComponent<BOss>()?.hit();
        }
        if (other.GetComponent<BossChongwu>() != null)
        {
            other.GetComponent<BossChongwu>()?.TakeDamage(this);
            if (isjinzhan)
                other.GetComponent<BOss>()?.hit();
        }
        if (other.GetComponent<Boss2Rabbit>() != null)
        {
            other.GetComponent<Boss2Rabbit>()?.TakeDamage(this);
            if (isjinzhan)
                other.GetComponent<Boss2Rabbit>()?.hiit();
        }
        if (other.GetComponent<Boss2Rabbit22>() != null)
        {
            other.GetComponent<Boss2Rabbit22>()?.TakeDamage(this);
            if (isjinzhan)
                other.GetComponent<Boss2Rabbit22>()?.hiit();
        }
        if (other.GetComponent<Chongwu1>() != null)
        {
            other.GetComponent<Chongwu1>()?.TakeDamage(this);
           
        }
        other.GetComponent<pohuai>()?.TakeDamage(this);

    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (Gan != null)
        {
            zidanjiajia = false;
            Gan.ZidanUI.fillAmount = Gan.zidanNum / Gan.zidanNumMax;
        }
        if (HaveFire&&other.GetComponent<Character>()!=null)
        {
            other.GetComponent<Character>().FireStart = true;
        }
        if (HaveEle&&other.GetComponent<Character>()!=null)
        {
            other.GetComponent<Character>().EleStart = true;
        }
    }

}

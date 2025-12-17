using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BossChongEff2 : StateMachineBehaviour
{
    private BossChongwu bossChongwu;
    private GameObject wuqi;
    private Animator _childAnimator;
    private Rigidbody2D rb;
    public Transform playertrans;
    private Vector3 Playtransform;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossChongwu = animator.GetComponent<BossChongwu>();
        wuqi = animator.GetComponent<GameObject>();
        rb=animator.GetComponent<Rigidbody2D>();
        Transform childTrans = animator.transform.Find("Frank_attack-Sheet_0");
        _childAnimator = childTrans.GetComponent<Animator>();
        _childAnimator.SetBool("AttackEff2",true);

        //转向面向玩家
       // bossChongwu.LookAtPlayer();
        //记录玩家的相对位置，进行冲刺
        playertrans = GameObject.FindGameObjectWithTag("Player").transform;
        if (animator. transform.position.x > playertrans.position.x)
        {
            Playtransform = new Vector3(-5,0,0); 
            
        }
        else if (animator.transform.position.x < playertrans.position.x)
        {
            Playtransform= new Vector3(5,0,0);
            
        }
    }
        

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //开始冲刺
        rb.velocity = Playtransform;
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossChongwu.isEffecting = false;
        _childAnimator.SetBool("AttackEff2", false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}

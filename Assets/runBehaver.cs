using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class runBehaver : StateMachineBehaviour
{
    public float timer;
    public float minTime;
    public float maxTime;
    public Rigidbody2D rb;
    private Transform bosstransform;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer=Random .Range(minTime,maxTime);
        rb=animator.GetComponent <Rigidbody2D>();
        bosstransform =animator.GetComponent <Transform>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.velocity =new Vector2 (rb.velocity .x,rb.velocity.y);
        if (timer <= 0)
        {
            animator.SetTrigger("jump");
               //在boss敌人的跳跃动画中插入跳跃事件
    
       
         // 重置Y轴速度（避免下落过程中跳跃力被抵消，导致跳不高）
         rb.velocity = new Vector2(rb.velocity.x, 0);
         // 施加向上的冲量（ForceMode2D.Impulse适合瞬间发力，符合跳跃手感）
         //rb.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
          rb.velocity = new Vector2(rb.velocity.x, 10);
    
      }
        else
        {
            timer-=Time .deltaTime;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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

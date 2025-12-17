using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEngine.RuleTile.TilingRuleOutput;

public class jumpBehaver : StateMachineBehaviour
{
    public float timer;
    public float minTime;
    public float maxTime;
    public float jumpForce;

    public Transform player;
    Rigidbody2D rb;
    BOss boss;


    //public Transform playerPos;
    public float speed;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //playerPos=GameObject.FindGameObjectWithTag("Player").GetComponent <Transform>();
        timer=Random.Range(minTime,maxTime);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<BOss>();
       // rb.velocity = new Vector2(rb.velocity.x, 15f);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (timer <= 0)
        {
            animator.SetTrigger("idle");
        }
        else
        {
            timer-=Time.deltaTime; 
        }


        boss.LookAtPlayer();   //³¯ÏòÍæ¼ÒÌøÔ¾

        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        //Vector2 target=new Vector2(playerPos.position .x, animator.transform.position.y);
        //animator.transform.position =Vector2.MoveTowards(animator .transform.position, target, speed*Time.deltaTime);
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

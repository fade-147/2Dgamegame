using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private PyhsicsCheck physicsCheck;
    private NewBehaviourScript playerController;
    private Character character;

    private void Awake()
    {
        anim=GetComponent <Animator>();
        rb=GetComponent <Rigidbody2D>();
        physicsCheck =GetComponent<PyhsicsCheck >();
        playerController = GetComponent<NewBehaviourScript>();
        character = GetComponent<Character>();
    }

    private void Update()
    {
        SetAnimation();
    }

    public void SetAnimation()
    {
        anim.SetFloat("velocityX", Mathf .Abs(rb.velocity.x));
        anim.SetFloat("velocityY", rb.velocity.y);
        anim.SetBool("isGround", physicsCheck.isGround);
        anim.SetBool("isDead", playerController .isDead);
        anim.SetBool("isAttack", playerController.isAttack);
        anim.SetBool("isRolling",playerController .isRoll );
        anim.SetBool("isclbWall", playerController.isWallMove);
        anim.SetBool("isfangyu", playerController.isfangyu);
        anim.SetBool("OKfangyu", character.OKfangyu);
    }

    public void PlayHurt()
    {
        anim.SetTrigger("hurt");
    }

    public void PlayAttack()
    {
        anim.SetTrigger("attack");
    }
}

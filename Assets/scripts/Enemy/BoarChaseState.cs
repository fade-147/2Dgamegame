using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        CurrentEnemy = enemy;
        CurrentEnemy.currentSpeed = CurrentEnemy.chaseSpeed;
        CurrentEnemy.anim.SetBool("run",true);
    }

    public override void LogicUpdate()
    {
        if(CurrentEnemy.lostTimeCounter <= 0)
        {
            CurrentEnemy .SwitchState (NPCState.Patrol);
        }
        if (!CurrentEnemy.pyhsicsCheck.isGround || (CurrentEnemy.pyhsicsCheck.touchLeftWall && CurrentEnemy.faceDir.x > 0) || (CurrentEnemy.pyhsicsCheck.touchRightWall && CurrentEnemy.faceDir.x < 0))
        {
            CurrentEnemy.transform .localScale =new Vector3 (CurrentEnemy .faceDir.x*-1,4,4);
        }

    }

    public override void PhysicsUpdate()
    {

    }
    public override void OnExit()
    {
        CurrentEnemy.lostTimeCounter =CurrentEnemy.lostTime;
        CurrentEnemy.anim.SetBool("run", false );

    }

}

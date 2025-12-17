using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        CurrentEnemy = enemy;
        CurrentEnemy.currentSpeed = CurrentEnemy.normalSpeed;
    }

    public override void LogicUpdate()
    {
        //TODO:·¢ÏÖplayerÇÐ»»µ½chase
        if (CurrentEnemy.FoundPlayer())
        {
            CurrentEnemy.SwitchState(NPCState.Chase);
        }

        if (!CurrentEnemy.pyhsicsCheck.isGround || (CurrentEnemy.pyhsicsCheck.touchLeftWall && CurrentEnemy.faceDir.x > 0) || (CurrentEnemy.pyhsicsCheck.touchRightWall && CurrentEnemy.faceDir.x < 0))
        {
            CurrentEnemy.wait = true;
            CurrentEnemy.anim.SetBool("walk", false);
        }
        else
        {
            CurrentEnemy.anim.SetBool("walk", true);
        }

    }

    public override void PhysicsUpdate()
    {

    }
    public override void OnExit()
    {
          CurrentEnemy.anim.SetBool("walk", false);

    }

}

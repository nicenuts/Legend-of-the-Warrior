using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEneny = enemy;
        currentEneny.currentSpeed = currentEneny.normalSpeed;  
    }

    public override void LogicUpdate()
    {
        //发现敌人的时候，由巡逻状态转化为追击状态
        if (currentEneny.FoundPlayer())
        {
            currentEneny.SwitchState(NPCstate.chase);    
        }

        //撞墙判断和敌人前方悬崖判断（防止野猪踩空掉落）
        if((!currentEneny.physicsCheck.isGround) || (currentEneny.physicsCheck.touchLeftWall && currentEneny.faceDir.x<0 ) || (currentEneny.physicsCheck.touchRightWall && currentEneny.faceDir.x>0))
        {
            currentEneny.wait = true;
            currentEneny.anim.SetBool("walk", false);
        }
        else
        {
            currentEneny.anim.SetBool("walk",true);
        }

    }

    public override void PhysicsUpdate()
    {
        
    }
    
    public override void OnExit()
    {
        currentEneny.anim.SetBool("walk", false);
    }
}

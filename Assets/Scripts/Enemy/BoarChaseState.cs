using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEneny = enemy;
        currentEneny.currentSpeed = currentEneny.chaseSpeed;
        currentEneny.anim.SetBool("run", true);
    }
    
    public override void LogicUpdate()
    {
        //如果玩家不在野猪的视野范围内超出一定时间，则变为巡逻状态
        if(currentEneny.lostTimeCounter<=0)
        {
            currentEneny.SwitchState(NPCstate.patrol); 
        }

        //撞墙转身
        if((!currentEneny.physicsCheck.isGround) || (currentEneny.physicsCheck.touchLeftWall && currentEneny.faceDir.x<0 ) || (currentEneny.physicsCheck.touchRightWall && currentEneny.faceDir.x>0))
        {
            currentEneny.transform.localScale = new Vector3(currentEneny.faceDir.x,1,1);  //立刻转身
        }
    }


    public override void PhysicsUpdate()
    {
  
    }
    
    public override void OnExit()
    {
        currentEneny.anim.SetBool("run", false);  //退出chase状态时，同时也退出run动画
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Locomotion : Player_State_Base
{
    private float accelerateVel,decelerateVel,turnVel;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetFloat("MoveBlend", 0);
        player.SetLeaning(true);
        player.PointerMode_Guard(false);
        animator.SetBool("Strafe",false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if(!player.particle_target.IsEnforced()) player.SetClosestTarget();
        
        if (finished) return;
        if (Canvas_Player.AB_Pressed&& !animator.IsInTransition(0))
        {
            animator.SetBool("Strafe",true);
            animator.SetFloat("Strafe_X",0);
            animator.SetFloat("Strafe_Y",0);
            finished = true;
            return;
        }

        if (PreInput(animator)) return;



    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (finished) return;
        
        
        Vector3 targetDir = Quaternion.Euler(0,CamArm.Degree(),0)*
                            new Vector3(Canvas_Player.LS.x,0,Canvas_Player.LS.y).normalized;
        bool isAccelerating = false;
        //이동 애니메이션 설정
        if (Canvas_Player.LS_Scale > 0.1f)
        {
            float targetBlend = Mathf.SmoothDamp(animator.GetFloat("MoveBlend"),1,ref accelerateVel,player.accelerateDuration);
            isAccelerating = animator.GetFloat("MoveBlend")+Time.deltaTime*0.001f < targetBlend;
            animator.SetFloat("MoveBlend", targetBlend);
        }
        else
        {
            float targetBlend = Mathf.SmoothDamp(animator.GetFloat("MoveBlend"),0.0f,
                ref decelerateVel,player.decelerateDuration);
            isAccelerating = animator.GetFloat("MoveBlend")+Time.deltaTime*0.001f < targetBlend;
            animator.SetFloat("MoveBlend", targetBlend);
        }
        //이동 설정
        float speed = player.moveCurve.Evaluate(animator.GetFloat("MoveBlend")) * player.moveSpeed;
        Vector3 pos = player.transform.position + animator.deltaPosition.normalized*speed*Time.deltaTime;
        //회전 설정
        float currentDeg = player.transform.rotation.eulerAngles.y;
        float targetDeg= Canvas_Player.LS_Scale>0.1f? 
            Quaternion.LookRotation(targetDir).eulerAngles.y: currentDeg;
        float deg = Mathf.SmoothDampAngle(currentDeg, targetDeg, ref turnVel, player.turnDuration);
        Quaternion rot = Quaternion.Euler(0, deg, 0);
        
        player.UpdateLeaning(isAccelerating,speed);
        player.Move(pos,rot);
    }
}

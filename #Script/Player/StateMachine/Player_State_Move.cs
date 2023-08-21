using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Player_State_Move : Player_State_Base
{
    public float turnSpeed;//회전속도
    public float stopDegree = 150;//한계 상대각도
    public float moveSpeed=1;//이동속도
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (finished) return;
        if(PreInput(animator)) return;
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        //이동 입력 해제 [또는] 한계 각도 초과 시 => 다음으로(마지막이므로 처음으로 Repeat.) 
        if (Canvas_Player.LS_Scale <= 0.1f || Mathf.Abs(player.Deg_JSL_Relative()) > stopDegree)
        {
            animator.SetFloat("StartMove",0);
            animator.SetBool("Move",false);
            finished = true;
            return;
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (finished) return;
        base.OnStateMove(animator, stateInfo, layerIndex);
        
        //이동 입력 방향으로 회전값 target에 설정.
        float deg_js = player.DEG(Canvas_Player.Degree_Left());
        float deg_cam = player.DEG(CamArm.Degree());
        float target = player.DEG(deg_js + deg_cam);
        target = Mathf.MoveTowardsAngle(animator.transform.rotation.eulerAngles.y, 
            target, turnSpeed * Time.deltaTime);
        Vector3 moveVec = animator.deltaPosition * moveSpeed;
        player.Move(animator.transform.position+moveVec,Quaternion.Euler(0,target,0));
    }
}
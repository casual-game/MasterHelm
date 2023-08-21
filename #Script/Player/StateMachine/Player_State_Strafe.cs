using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;

public class Player_State_Strafe : Player_State_Base
{
	private float turnVel;
	private Vector3 accelerateVel, decelerateVel;
	private float lastNormalizedTime = 0;
	private float enteredTime = -100;
	private bool rollcheck = false;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		animator.SetFloat("Strafe_X",0);
		animator.SetFloat("Strafe_Y",0);
		player.SetLeaning(true);
		player.PointerMode_Guard(true);
		player.guarded = false;
		enteredTime = Time.unscaledTime;
		rollcheck = false;
	}
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
	    
	    base.OnStateUpdate(animator, stateInfo, layerIndex);
	    if (finished) return;
	    
	    //수동 구르기 (가드한 경우 구르기 불가)
	    if (!player.guarded&&!rollcheck && !Canvas_Player.AS_Dragged
	        &&Canvas_Player.AB_ReleasedTime -  Canvas_Player.AB_PressedTime < player.preInput_Roll&& !Canvas_Player.AB_Pressed)
	    {
		    rollcheck = true;
		    player.Roll();
		    animator.SetBool("Strafe",false);
		    finished = true;
		    return;
	    }
	    

	    //if (rollcheck&&!player.guarded) return;
	    //수동 선입력
	    if (player.CanSkill(animator))
	    {
		    finished = true;
		    player.Skill();
		    return;
	    }
	    else if (player.CanAttack(animator))
	    {
		    finished = true;
		    player.Attack();
		    return;
	    }
	    if (!Canvas_Player.AB_Pressed)
	    {
		    animator.SetBool("Strafe",false);
		    Canvas_Player.AB_PressedTime = -100;
		    finished = true;
	    }
    }
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
	    base.OnStateMove(animator, stateInfo, layerIndex);
	    if (finished) return;
	    
		//회전
	    float targetDeg;
	    float currentDeg = player.transform.rotation.eulerAngles.y;
	    if (player.target != null && Canvas_Player.AS_Scale<0.1f 
	        && (!rollcheck||Time.unscaledTime-enteredTime>player.rollInputDelay))
	    {
		    Vector3 dist = player.target.transform.position - player.transform.position;
		    dist.y = 0;
		    targetDeg = 90 - Mathf.Atan2(dist.z, dist.x) * Mathf.Rad2Deg;
	    }
	    else
	    {
		    Vector3 targetDir;
		    if (Canvas_Player.AS_Scale > 0.1f)
		    {
			    targetDir = Quaternion.Euler(0, CamArm.Degree(), 0) *
			                new Vector3(Canvas_Player.AS.x, 0, Canvas_Player.AS.y).normalized;
		    }
		    else if (Canvas_Player.LS_Scale > 0.1f)
		    {
			    targetDir = Quaternion.Euler(0, CamArm.Degree(), 0) *
			                new Vector3(Canvas_Player.LS.x, 0, Canvas_Player.LS.y).normalized;
		    }
		    else
		    {
			    targetDir = player.transform.forward;
		    }
		    targetDeg= Quaternion.LookRotation(targetDir).eulerAngles.y;
	    }
	    
	    
	    float deg = Mathf.SmoothDampAngle(currentDeg, targetDeg, ref turnVel, player.turnDuration);
	    Quaternion rot = Quaternion.Euler(0, deg, 0);
	    //이동
	    Vector3 moveDir = Quaternion.Euler(0, CamArm.Degree(), 0) *
	                      new Vector3(Canvas_Player.LS.x, 0, Canvas_Player.LS.y).normalized;
	    Vector3 vec = Vector3.zero;
	    Vector3 currentVec = new Vector3(animator.GetFloat("Strafe_X"), 0, animator.GetFloat("Strafe_Y"));
	    if (Canvas_Player.LS_Scale > 0.1f)
	    {
			
		    Vector3 targetVec = Quaternion.Euler(0, -player.transform.rotation.eulerAngles.y, 0) * moveDir;
		    vec = Vector3.SmoothDamp(currentVec,targetVec,ref accelerateVel,player.accelerateDuration*0.5f);
	    }
	    else
	    {
		    Vector3 targetVec = Vector3.zero;
		    vec = Vector3.SmoothDamp(currentVec,targetVec,ref decelerateVel,player.decelerateDuration*0.5f);
	    }
	    animator.SetFloat("Strafe_X",vec.x);
	    animator.SetFloat("Strafe_Y",vec.z);
		//소리
		if(lastNormalizedTime-(int)lastNormalizedTime<=0.2f && 0.2f<stateInfo.normalizedTime-(int)stateInfo.normalizedTime) player.Footstep_L();
		if(lastNormalizedTime-(int)lastNormalizedTime<=0.7f && 0.7f<stateInfo.normalizedTime-(int)stateInfo.normalizedTime) player.Footstep_R();
		lastNormalizedTime = stateInfo.normalizedTime;
	    player.Move(player.transform.position+animator.deltaPosition*player.strafeSpeed,rot);
    }
}

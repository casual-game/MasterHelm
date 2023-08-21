using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_BowStrafe : Player_State_Base
{
	private float turnVel;
	private Vector3 accelerateVel, decelerateVel;
	private float lastNormalizedTime = 0;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		player.prefab_bow.SetHold(true);
		animator.SetFloat("Bow_X",0);
		animator.SetFloat("Bow_Y",0);
		animator.SetBool("Bow",true);
		player.SetLeaning(false);
	}
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (finished) return;
        
        if (!Canvas_Player.RB_Pressed)
        {
	        animator.SetBool("Bow",false);
	        if (player.target != null)
	        {
		        Vector3 lookVec= player.target.transform.position-player.transform.position;
		        lookVec.y = 0;
		        player.Move(player.transform.position,Quaternion.LookRotation(lookVec));
	        }
	        finished = true;
        }
    }
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
	    base.OnStateMove(animator, stateInfo, layerIndex);
	    if (finished) return;
	    
	    //회전
	    float currentDeg = player.transform.rotation.eulerAngles.y;
	    float targetDeg = ShootDeg();
	    float deg = Mathf.SmoothDampAngle(currentDeg, targetDeg, ref turnVel, player.turnDuration*0.5f);
	    Quaternion rot = Quaternion.Euler(0, deg, 0);
	    //이동
	    Vector3 moveDir = Quaternion.Euler(0, CamArm.Degree(), 0) *
	                      new Vector3(Canvas_Player.LS.x, 0, Canvas_Player.LS.y).normalized;
	    Vector3 vec = Vector3.zero;
	    Vector3 currentVec = new Vector3(animator.GetFloat("Bow_X"), 0, animator.GetFloat("Bow_Y"));
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
	    animator.SetFloat("Bow_X",vec.x);
	    animator.SetFloat("Bow_Y",vec.z);
		//소리
		if(lastNormalizedTime-(int)lastNormalizedTime<=0.2f && 0.2f<stateInfo.normalizedTime-(int)stateInfo.normalizedTime) player.Footstep_L();
		if(lastNormalizedTime-(int)lastNormalizedTime<=0.7f && 0.7f<stateInfo.normalizedTime-(int)stateInfo.normalizedTime) player.Footstep_R();
		lastNormalizedTime = stateInfo.normalizedTime;
	    player.Move(player.transform.position+animator.deltaPosition*player.strafeSpeed,rot);
    }
}

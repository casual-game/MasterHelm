using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_MoveStart : Player_State_Base
{
	public float turnSpeed=90;//회전 속도
	public float moveSpeed=1;//이동 속도
	public AnimationCurve turnCurve = AnimationCurve.EaseInOut(0,0,1,1);//회전 속도 보간 Curve	
	
	private float startDeg;
	private float endDeg;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		startDeg = animator.transform.rotation.eulerAngles.y;
		endDeg = startDeg + animator.GetFloat("StartMove");
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (finished) return;
		if(PreInput(animator)) return;
		base.OnStateUpdate(animator, stateInfo, layerIndex);
		
		
		//이동 입력 종료 => 처음으로
		if (Canvas_Player.LS_Scale <= 0.1f)
		{
			animator.SetBool("Move",false);
			finished = true;
			return;
		}
	}

	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (finished) return;
		base.OnStateMove(animator, stateInfo, layerIndex);
		
		//[최종 각도 + 보정각도] 형식으로 표현된 변수 target (자연스러운 회전)
		float target = endDeg;
		if (Canvas_Player.LS_Scale > 0.1f) target += player.DEG(player.Deg_JSL_Absolute() - endDeg);
		//회전. 
		endDeg = Mathf.MoveTowardsAngle(endDeg,target , turnSpeed * Time.deltaTime);
		//최종 각도의 벡터로 내적한 벡터로만 이동.
		Vector3 rotVec = Quaternion.Euler(0, endDeg, 0) * Vector3.forward.normalized;
		float moveDist = !animator.IsInTransition(0)?
			Mathf.Max(0,Vector3.Dot(rotVec,animator.deltaPosition * moveSpeed)):0;
		player.Move(animator.transform.position +rotVec*moveDist,
			Quaternion.Euler(0,Mathf.Lerp(startDeg,endDeg,turnCurve.Evaluate(stateInfo.normalizedTime)),0));
	}
}
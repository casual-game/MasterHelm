using System.Collections;
using System.Collections.Generic;
using FIMSpace;
using Sirenix.OdinInspector;
using UnityEngine;

public class Enemy_State_Run : Enemy_State_Base
{
	[TitleGroup("이동 설정")][BoxGroup("이동 설정/bg",false)][LabelText("이동 가속시간")]public float movespeedSmoothTime = 0.5f;
	[TitleGroup("이동 설정")][BoxGroup("이동 설정/bg",false)][LabelText("이동 속도")]public float speed=2;
	[TitleGroup("이동 설정")][BoxGroup("이동 설정/bg",false)][LabelText("회전 시간")] public float rotateSmoothTime = 0.5f;
	[TitleGroup("이동 설정")][BoxGroup("이동 설정/bg",false)][LabelText("rootmotion 속도")][Range(0,2)]public float moveSpeedRatio = 0.7f;
	[TitleGroup("이동 설정")] [BoxGroup("이동 설정/bg", false)] [LabelText("rootmotion 사용 여부")] public bool useMotionSpeed = false;
	private float lastRecalculatedTime = -100;
	private int currentVectorPathIndex;

	private float moveSpeed;
	private float moveSpeedCurrentSmooth,rotateCurrentSmooth,animatorCurrentSmoothX,animatorCurrentSmoothY;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		if (finished) return;
		currentVectorPathIndex = 0;
		moveSpeed = 0;
	}
	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateMove(animator, stateInfo, layerIndex);
		if (finished) return;
		Transform t = enemy.transform;
		Vector3 tPos = t.position;
		//path가 없거나 계산 대기중일때
		if (enemy.PF_Count() == 0)
		{
			moveSpeed = Mathf.SmoothDamp(moveSpeed, 0, ref moveSpeedCurrentSmooth, movespeedSmoothTime);
		}
		//path가 계산되었을 경우
		else
		{
			Vector3 targetVec = enemy.PF_GetPosition(currentVectorPathIndex);
			bool arrived = false;
			float rotateDeg;
			//일정 시간마다 최신 path로 업데이트, 도착 여부에 따라 속도 변경
			enemy.PF_UpdatePath(ref currentVectorPathIndex,ref arrived);
			
			if (arrived)
			{
				rotateDeg = t.rotation.eulerAngles.y;
				
				Arrived(animator);
				
			}
			else
			{
				moveSpeed = Mathf.SmoothDamp(moveSpeed, 1, ref moveSpeedCurrentSmooth, movespeedSmoothTime*0.75f);
				//플레이어를 보도록 회전
				Vector3 dist = targetVec - tPos;
				dist.y = 0;
				float targetDeg = Quaternion.LookRotation(dist).eulerAngles.y;
				rotateDeg = Mathf.SmoothDampAngle(t.rotation.eulerAngles.y, targetDeg
					, ref rotateCurrentSmooth, rotateSmoothTime);
			}
			//최종 적용 (이동은 rootmotion으로.)
			if(useMotionSpeed) enemy.Move(tPos + t.forward*(animator.deltaPosition.magnitude)*speed, Quaternion.Euler(0, rotateDeg, 0));
			else enemy.Move(tPos + t.forward*moveSpeedRatio*speed*Time.deltaTime, Quaternion.Euler(0, rotateDeg, 0));

			
		}
	}
	protected void Arrived(Animator animator)
	{
		moveSpeed = Mathf.SmoothDamp(moveSpeed, 0, ref moveSpeedCurrentSmooth, movespeedSmoothTime);
		enemy.State_Finish();
		finished = true;
	}
	
}

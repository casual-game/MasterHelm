using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_State_Backstep : Enemy_State_Base
{
	public float moveSpeedRatio = 1.0f;
	private Quaternion startRot, endRot;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		Vector3 lookVec = Player.instance.transform.position - enemy.transform.position;
		lookVec.y = 0;
		startRot = enemy.transform.rotation;
		endRot = Quaternion.LookRotation(lookVec);
	}

	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateMove(animator, stateInfo, layerIndex);
		if (finished) return;
		if (stateInfo.normalizedTime > 0.8f)
		{
			//enemy.Pattern_Finished();
			finished = true;
			return;
		}
		float ratio = Mathf.Clamp01(stateInfo.normalizedTime * 2);
		Quaternion finalRot = Quaternion.Lerp(startRot,endRot,ratio);
		enemy.Move(enemy.transform.position + animator.deltaPosition*moveSpeedRatio,finalRot);
	}
}

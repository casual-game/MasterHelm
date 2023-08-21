using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_State_Idle : Enemy_State_Base
{
	private bool canFinish;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		canFinish = enemy.state_enabled && enemy.state_NormalState== Enemy.NormalState.Idle;
	}

	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateMove(animator, stateInfo, layerIndex);
		if (finished) return;
		if (canFinish && !animator.IsInTransition(0))
		{
			finished = true;
			enemy.State_Finish();
		}
	}
}

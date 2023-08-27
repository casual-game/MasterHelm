using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_State_Finish : Enemy_State_Base
{
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		if (enemy.isGuardBreak && !enemy.keepSuperArmor)
		{
			enemy.SuperArmor(false);
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateUpdate(animator, stateInfo, layerIndex);
		if(finished) return;
		if (!animator.IsInTransition(0))
		{
			finished = true;
			enemy.State_Finish();
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Hit : Player_State_Base
{
	public float motionSpeed = 1.0f;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		
	}

	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateMove(animator, stateInfo, layerIndex);
		if (finished) return;
		player.Move(player.transform.position+animator.deltaPosition*motionSpeed, animator.rootRotation);
		if (stateInfo.normalizedTime > 0.75f)
		{
			if (player.CanSkill(animator))
			{
				player.Skill();
			}
			else if (player.CanAttack(animator))
			{
				player.Attack();
			}
			else
			{
				player.ChangeState(0);
			}
			finished = true; 
			return;
		}
	}
}

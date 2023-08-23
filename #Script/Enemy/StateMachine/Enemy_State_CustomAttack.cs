using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_State_CustomAttack : Enemy_State_Normal
{
	public float rotateSmoothTime = 0.5f;
	private float rotateCurrentSmooth;
	private Data_EnemyMotion.SingleAttackData currentAttackData;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		currentAttackData = motionData.attackData[0];
		enemy.currentSingleAttackData = currentAttackData;
		if (motionData.attackData[0].isGuardBreak)
		{
			enemy.RoarVoice();
			Canvas_Player_World.instance.Skull();
			enemy.particle_charge.Play();
			enemy.audio_create.Play();
		}
		else
		{
			enemy.AttackVoice();
		}
	}

	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateMove(animator, stateInfo, layerIndex);
		foreach (var data in motionData.attackData)
		{
			bool contain = data.trailRange.x <= stateInfo.normalizedTime &&
			               stateInfo.normalizedTime < data.trailRange.y;
			bool isDifferent = data != currentAttackData;

			if (contain && isDifferent)
			{
				currentAttackData = data;
				enemy.currentSingleAttackData = currentAttackData;
				if (currentAttackData.isGuardBreak)
				{
					Canvas_Player_World.instance.Skull();
					enemy.particle_charge.Play();
					enemy.audio_create.Play();
				}
			}
		}
		
		
		//회전
		Vector3 dist = Player.instance.transform.position - enemy.transform.position;
		dist.y = 0;
		float targetDeg = Quaternion.LookRotation(dist).eulerAngles.y;
		float rotateDeg = animator.transform.rotation.eulerAngles.y;

		bool canRotate = currentAttackData.turnRange.x < stateInfo.normalizedTime &&
		                 stateInfo.normalizedTime < currentAttackData.turnRange.y;
		if (canRotate)
		{
			rotateDeg= Mathf.SmoothDampAngle(enemy.transform.rotation.eulerAngles.y, targetDeg
				, ref rotateCurrentSmooth, rotateSmoothTime);
		}
		Vector3 moveVec = enemy.transform.position;
		if ((enemy.transform.position - Player.instance.transform.position).sqrMagnitude > 2)
		{
			moveVec += animator.deltaPosition * motionData.animMoveSpeed;
		}
		Quaternion moveRot = Quaternion.Euler(0,rotateDeg,0);
		enemy.Move(moveVec,moveRot);
	}
}

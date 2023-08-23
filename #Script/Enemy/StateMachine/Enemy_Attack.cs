using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack : Enemy_State_Base
{
	public float rotateSmoothTime = 0.5f;
	public Data_EnemyMotion motionData;
	public bool dontSkull = false;
	private bool currentTrail = false;
	private float rotateCurrentSmooth;
	
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		currentTrail = false;
		SetTrail(false,false,false);
		if (!dontSkull && motionData.attackData.Count > 0 && motionData.attackData[0].isGuardBreak)
		{
			Canvas_Player_World.instance.Skull();
			enemy.particle_charge.Play();
			enemy.audio_create.Play();
			enemy.isGuardBreak = true;
		}
		else
		{
			enemy.isGuardBreak = false;
			
		}
		enemy.AttackVoice();
		enemy.currentSingleAttackData = motionData.attackData[0];
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (finished) return;
		base.OnStateUpdate(animator, stateInfo, layerIndex);
		
		//Trail범위 체크
		bool isTrail = false, isFirst = true, skulled = false;
		bool left=false, right=false, shield=false;
		foreach (var data in motionData.attackData)
		{
			if (data.trailRange.x <= stateInfo.normalizedTime 
			    && stateInfo.normalizedTime < data.trailRange.y)
			{
				if (!isFirst && !skulled && data.isGuardBreak)
				{
					skulled = true;
					Canvas_Player_World.instance.Skull();
					enemy.particle_charge.Play();
					enemy.audio_create.Play();
				}
				if (enemy.currentSingleAttackData != data)
				{
					enemy.currentSingleAttackData = data;
					enemy.isGuardBreak = data.isGuardBreak;
				}
				isTrail = true;
				left = data.left;
				right = data.right ;
				shield = data.shield;
				break;
			}
			isFirst = false;
		}
		if (currentTrail != isTrail)
		{
			if(isTrail)SetTrail(left, right, shield);
			else SetTrail(false,false,false);
		}
		currentTrail = isTrail;
	}

	public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (finished) return;
		base.OnStateMove(animator, stateInfo, layerIndex);
		
		//
		if (motionData.attackData[motionData.attackData.Count - 1].trailRange.y < stateInfo.normalizedTime) return;
		
		//회전
		Vector3 dist = Player.instance.transform.position - enemy.transform.position;
		dist.y = 0;
		float targetDeg = Quaternion.LookRotation(dist).eulerAngles.y;
		float rotateDeg = Mathf.SmoothDampAngle(enemy.transform.rotation.eulerAngles.y, targetDeg
			, ref rotateCurrentSmooth, rotateSmoothTime);
		Vector3 moveVec = enemy.transform.position;
		if ((enemy.transform.position - Player.instance.transform.position).sqrMagnitude > 2)
		{
			moveVec += animator.deltaPosition * motionData.animMoveSpeed;
		}
		Quaternion moveRot = Quaternion.Euler(0,rotateDeg,0);
		enemy.Move(moveVec,moveRot);
	}
}

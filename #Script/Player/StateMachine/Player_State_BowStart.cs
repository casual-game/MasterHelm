using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_BowStart : Player_State_Base
{
	private float turnVel;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		player.ChangeWeaponData(Player.CurrentWeaponData.Bow);

		animator.speed = 1;
		if (player.prefab_shield != null) player.prefab_shield.Trail_Off();
		if (player.prefab_weaponL != null) player.prefab_weaponL.Trail_Off();
		if (player.prefab_weaponR != null) player.prefab_weaponR.Trail_Off();
		if (player.prefab_shield_SkillL != null) player.prefab_shield_SkillL.Trail_Off();
		if (player.prefab_shield_SkillR != null) player.prefab_shield_SkillR.Trail_Off();
		if (player.prefab_weaponL_SkillL != null) player.prefab_weaponL_SkillL.Trail_Off();
		if (player.prefab_weaponL_SkillR != null) player.prefab_weaponL_SkillR.Trail_Off();
		if (player.prefab_weaponR_SkillL != null) player.prefab_weaponR_SkillL.Trail_Off();
		if (player.prefab_weaponR_SkillR != null) player.prefab_weaponR_SkillR.Trail_Off();
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateUpdate(animator, stateInfo, layerIndex);
		if (finished) return;
		
		
		if (stateInfo.normalizedTime>0.5f)
		{
			player.prefab_bow.SetHold(true);
			player.prefab_bow.SetPulling(true);
			finished = true;
			return;
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
		player.Move(player.transform.position,rot);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_State_Base : StateMachineBehaviour
{
    protected Enemy enemy;
    [HideInInspector] public bool finished = false;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (enemy == null) enemy = animator.GetComponent<Enemy>();
        enemy.stateMachineBehavior = this;
        finished = false;
        enemy.currentSingleAttackData = null;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (animator.IsInTransition(0) &&
            stateInfo.shortNameHash != animator.GetNextAnimatorStateInfo(0).shortNameHash) finished = true;
        if(!animator.IsInTransition(0) && 
            stateInfo.shortNameHash != animator.GetCurrentAnimatorStateInfo(0).shortNameHash) finished = true;
        if (finished) return;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (animator.IsInTransition(0) &&
            stateInfo.shortNameHash != animator.GetNextAnimatorStateInfo(0).shortNameHash) finished = true;
        if(!animator.IsInTransition(0) && 
           stateInfo.shortNameHash != animator.GetCurrentAnimatorStateInfo(0).shortNameHash) finished = true;
        if (finished) return;
    }

    protected void SetTrail(bool left, bool right, bool shield)
    {
        if (enemy.prefab_Weapon_L != null)
        {
            if(left) enemy.prefab_Weapon_L.Trail_On();
            else enemy.prefab_Weapon_L.Trail_Off();
        }
        if (enemy.prefab_Weapon_R != null)
        {
            if(right) enemy.prefab_Weapon_R.Trail_On();
            else enemy.prefab_Weapon_R.Trail_Off();
        }
        if (enemy.prefab_Shield != null)
        {
            if(shield) enemy.prefab_Shield.Trail_On();
            else enemy.prefab_Shield.Trail_Off();
        }
    }
}

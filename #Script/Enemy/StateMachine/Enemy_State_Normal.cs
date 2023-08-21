using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_State_Normal : Enemy_State_Base
{
    public float motionSpeed = 1.0f;
    public bool isSkull = false;
    public Data_EnemyMotion motionData;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if(isSkull) Canvas_Player_World.instance.Skull();
        if (motionData != null) enemy.currentSingleAttackData = motionData.attackData[0];
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (finished) return;
        enemy.Move(enemy.transform.position + animator.deltaPosition * motionSpeed, animator.rootRotation);
    }
}

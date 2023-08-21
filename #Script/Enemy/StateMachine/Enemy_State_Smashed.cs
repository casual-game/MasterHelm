using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_State_Smashed : Enemy_State_Base
{
    public float moveRatio = 1.0f;
    public Quaternion lookRot;
    public bool lookBack = false;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Vector3 lookVec = Player.instance.transform.position - animator.transform.position;
        lookVec.y = 0;
        lookRot = Quaternion.LookRotation(lookVec);
        if (lookBack) lookRot *= Quaternion.Euler(0, 180, 0);
        enemy.Move(enemy.transform.position,lookRot);
    }
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if(finished) return;
        
        lookRot *= animator.deltaRotation;
        enemy.Move(animator.transform.position+animator.deltaPosition*moveRatio,lookRot);
    }
    
}
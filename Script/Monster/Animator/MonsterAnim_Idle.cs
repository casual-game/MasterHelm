using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MonsterAnim_Idle : MonsterAnim_Base
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _monster.Equipment_Equip();
        _monster.Set_HitState(Monster.HitState.Ground);
        _monster.Set_IsReadyTrue();
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        Vector3 velocity = _monster.Get_Agent().velocity;
        Vector3 lookVec = _monster.transform.forward;
        float targetSpeedY = Vector3.Dot(velocity, lookVec);
        float targetSpeedX = Mathf.Sqrt(velocity.sqrMagnitude - targetSpeedY * targetSpeedY);
        float speedY = Mathf.Lerp(animator.GetFloat(GameManager.s_movey), 
            targetSpeedY/_monster.Get_Agent().speed, 10 * Time.deltaTime);
        float speedX = Mathf.Lerp(animator.GetFloat(GameManager.s_movex), 
            targetSpeedX/_monster.Get_Agent().speed, 10 * Time.deltaTime);
        animator.SetFloat(GameManager.s_movey,speedY);
        animator.SetFloat(GameManager.s_movex,speedX);
    }
}

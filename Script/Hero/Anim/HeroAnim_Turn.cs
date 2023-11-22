using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Turn : HeroAnim_Base
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _hero.Set_SpeedRatio(0.35f);
        animator.SetFloat(GameManager.s_speed,_hero.Get_SpeedRatio());
        animator.SetBool(GameManager.s_leftstate,false);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
        //구르기
        if (_hero.Get_IsRollTiming())
        {
            Set_Roll(animator);
            return;
        }
        _hero.Move_Nav(animator.deltaPosition * _heroData.moveMotionSpeed_normal * 0.5f,
            animator.rootRotation);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnim_Idle : MonsterAnim_Base
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _monster.Equip();
        _monster.Core_HitState(Monster.HitState.Ground);
        _monster.Set_IsReadyTrue();
    }
}

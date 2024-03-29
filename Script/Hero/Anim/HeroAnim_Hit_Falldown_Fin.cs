using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class HeroAnim_Hit_Falldown_Fin : HeroAnim_Base
{
    public float motionSpeed = 1.0f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        animator.SetBool(GameManager.s_hit,false);
        _hero.Core_ResetRollInput();
        SoundManager.Play(_hero.sound_voice_short);
        SoundManager.Play(SoundContainer_Ingame.instance.sound_friction_cloth,0.125f);
        SoundManager.Play(_hero.sound_chain,0.375f);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator, stateInfo)) return;
        
        Vector3 relativePos = animator.deltaPosition*motionSpeed;
        Quaternion nextRot = animator.rootRotation;

        _hero.Move_Nav(relativePos, nextRot);
    }
}

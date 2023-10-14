using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Base : StateMachineBehaviour
{
    public HeroMovement.MoveState moveState;
    public bool useNavPosition = true;
    public bool useTrail = false;
    private bool script_entered = false;
    protected Hero hero;
    protected HeroMovement movement;
    [HideInInspector] public bool isFinished = false;
    [HideInInspector] public bool cleanFinished = false; //isFinished상태에서도 작동하는 일부 사례에서만 쓰입니다. (히트 시 attack계열 작업 종료)
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (!script_entered)
        {
            hero = animator.GetComponentInParent<Hero>();
            movement = animator.GetComponent<HeroMovement>();
            script_entered = true;
        }

        isFinished = false;
        cleanFinished = false;
        movement.anim_base = this;
        movement.moveState = moveState;
        movement.agent.updatePosition = useNavPosition;
        movement.trailEffect.active = useTrail;
        animator.speed = 1.0f;
    }

    protected bool IsNotAvailable(Animator animator, AnimatorStateInfo stateInfo)
    {
        bool isNotCurrentState = animator.IsInTransition(0) &&
                              animator.GetNextAnimatorStateInfo(0).shortNameHash != stateInfo.shortNameHash;
        return isNotCurrentState || isFinished;
    }
    protected void UpdateTrail(float normalizedTime, Data_WeaponPack weaponPack)
    {
        bool trail_weaponL = false, trail_weaponR = false, trail_shield = false;
        foreach (var trailData in movement.currentAttackMotionData.TrailDatas)
        {
            bool canTrail = trailData.trailRange.x <= normalizedTime && normalizedTime < trailData.trailRange.y;
            if (canTrail)
            {
                trail_weaponL = trailData.weaponL;
                trail_weaponR = trailData.weaponR;
                trail_shield = trailData.shield;
                break;
            }
        }
        movement.UpdateTrail(weaponPack,trail_weaponL,trail_weaponR,trail_shield);
    }

    protected void Set_Locomotion()
    {
        movement.UpdateTrail(movement.weaponPack_Normal,false,false,false);
        movement.ChangeAnimationState(HeroMovement.AnimationState.Locomotion);
        movement.Equip(null);
        movement.attackIndex = -1;
        isFinished = true;
    }
}

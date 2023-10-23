using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Base : StateMachineBehaviour
{
    public Hero.MoveState moveState;
    public bool useNavPosition = true;
    public bool useTrail = false;
    private bool script_entered = false;
    private static TrailData static_trailData = null;
    protected HeroData hero;
    protected Hero movement;
    [HideInInspector] public bool isFinished = false;
    [HideInInspector] public bool cleanFinished = false; //isFinished상태에서도 작동하는 일부 사례에서만 쓰입니다. (히트 시 attack계열 작업 종료)
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (!script_entered)
        {
            movement = animator.GetComponent<Hero>();
            hero = movement.heroData;
            script_entered = true;
        }
        
        isFinished = false;
        cleanFinished = false;
        movement.Set_AnimBase(this);
        movement.Set_HeroMoveState(moveState);
        movement.Get_NavMeshAgent().updatePosition = useNavPosition;
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
        foreach (var trailData in movement.CurrentAttackMotionData.TrailDatas)
        {
            bool canTrail = trailData.trailRange.x <= normalizedTime && normalizedTime < trailData.trailRange.y;
            if (canTrail)
            {
                trail_weaponL = trailData.weaponL;
                trail_weaponR = trailData.weaponR;
                trail_shield = trailData.shield;
                if (static_trailData != trailData)
                {
                    static_trailData = trailData;
                    movement.Equipment_Collision_Reset(weaponPack);
                }
                break;
            }
        }
        movement.Equipment_UpdateTrail(weaponPack,trail_weaponL,trail_weaponR,trail_shield);
        movement.Equipment_Collision_Interact(weaponPack,trail_weaponL,trail_weaponR,trail_shield);
    }

    protected void Set_Locomotion()
    {
        movement.Equipment_UpdateTrail(movement.weaponPack_Normal,false,false,false);
        movement.Set_AnimationState(Hero.AnimationState.Locomotion);
        movement.Equipment_Equip(null);
        movement.Set_AttackIndex(-1);
        isFinished = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HeroMovement : MonoBehaviour
{
    public enum AnimationState
    {
        Locomotion = 0,
        Roll = 1,
        Attack_Normal = 2,
        Attack_Strong = 3
    }

    public void ChangeAnimationState(AnimationState animationState)
    {
        animator.SetInteger(GameManager.s_state_type, (int)animationState);
        animator.SetTrigger(GameManager.s_state_change);
    }

    //첫번째 공격
    [HideInInspector] public bool isRightAtack = true;
    public int attackIndex = -1;
    [HideInInspector] public PlayerAttackMotionData currentAttackMotionData = null;
    [HideInInspector] private float attackPreInputTime = -100;

    public bool CanStrongAttack()
    {
        return false;
    }

    private void NormalAttack()
    {
        if (moveState == MoveState.Locomotion)
        {
            animator.SetInteger(GameManager.s_chargeenterindex,-1);
            Equip(weaponPack_Main);
            ChangeAnimationState(AnimationState.Attack_Normal);
            Effect_Smoke(0.25f);
        }
        else
        {
            attackPreInputTime = Time.time;
        }
    }
    public void StrongAttack()
    {
        if (CanStrongAttack())
        {
            
        }
        else NormalAttack();
        
    }
    //액션 기능
    private float action_BeginTime = -100;
    public void E_BTN_Action_Begin()
    {
        action_BeginTime = Time.unscaledTime;
    }
    public void E_BTN_Action_Fin()
    {
       Vector3 currentPos= transform.position;
        bool isTimting = Time.unscaledTime - action_BeginTime < hero.dash_roll_delay;
        //인터렉션
        if (Interactable.currentInteractable != null)
        {
            Vector3 interactablePos;
            interactablePos = Interactable.currentInteractable.transform.position;
            interactablePos.y =currentPos.y;
            bool canInteract = moveState == MoveState.Locomotion 
                               && Vector3.Dot(transform.forward, (interactablePos - currentPos).normalized) > 0.3f;
            if (canInteract && isTimting)
            {
                currentLadder = (Ladder)Interactable.currentInteractable;
                float currentY = currentPos.y;
                float dist_bottom = Mathf.Abs(currentLadder.range.x - currentY);
                float dist_top = Mathf.Abs(currentLadder.range.y - currentY);
                animator.SetFloat(GameManager.s_ladder_speed, (dist_bottom < dist_top) ? 1 : -1);
                animator.SetBool(GameManager.s_ladder, true);
                Interactable.currentInteractable.Interact();
                action_BeginTime = -100;
                return;
            }
        }

        //구르기
        bool canRoll = moveState == MoveState.Locomotion;
        if (canRoll && isTimting)
        {
            ChangeAnimationState(AnimationState.Roll);
            action_BeginTime = -100;
            return;
        }
        //Smashed이후 빠른 기상
        bool canSpeedRoll = Time.time < falledTime + hero.hit_Smash_RecoveryInputDelay && moveState == MoveState.Hit;
        if (canSpeedRoll)
        {
            Effect_FastRoll();
            animator.SetBool(GameManager.s_hit,false);
            animator.SetFloat(GameManager.s_hit_rot,-1);
            anim_base.isFinished = true;
            return;
        }
    }
    //히트
    public void Hit_Normal()
    {
        //감속시킨다.
        ratio_speed *=0.35f;
        animator.SetFloat(GameManager.s_crouch,0.6f);
        //타겟 벡터
        Vector3 hitpoint = Vector3.zero;
        Vector3 targetHitVec = hitpoint-transform.position;
        targetHitVec.y = 0;

        Vector3 myLookVec = transform.forward;
        float targetHitDeg = Mathf.Atan2(targetHitVec.z, targetHitVec.x)*Mathf.Rad2Deg;
        float myLookDeg = Mathf.Atan2(myLookVec.z, myLookVec.x) * Mathf.Rad2Deg;
        float degDiff = targetHitDeg - myLookDeg;
        while (degDiff < -180) degDiff += 360;
        while (degDiff > 180) degDiff -= 360;
        degDiff /= 180.0f;
        animator.SetFloat(GameManager.s_hit_rot,degDiff);
        animator.SetTrigger(GameManager.s_hit_additive);
        Effect_Hit_Normal();
    }

    private int hit_strong_type = 0;
    private float hit_strong_delay = 0.2f, hit_strong_time = -100;
    [HideInInspector] public float falledTime = -100;
    public AttackMotionType attackMotionType;
    public PlayerSmashedType playerSmashedType;
    public void Hit_Strong()
    {
        if (Time.time < hit_strong_time + hit_strong_delay) return;
        anim_base.isFinished = true;
        hit_strong_time = Time.time;
        hit_strong_type = (hit_strong_type + 1) % 2;
        
        animator.SetBool(GameManager.s_hit,true);
        animator.SetTrigger(GameManager.s_state_change);
        animator.SetFloat(GameManager.s_hit_rot,1);
        if(playerSmashedType == PlayerSmashedType.None) animator.SetInteger(GameManager.s_hit_type,hit_strong_type);
        else animator.SetInteger(GameManager.s_hit_type,(int)playerSmashedType);
        //타겟 벡터
        Vector3 hitpoint = Vector3.zero;
        Vector3 targetHitVec = hitpoint-transform.position;
        targetHitVec.y = 0;

        Vector3 myLookVec = transform.forward;
        float targetHitDeg = Mathf.Atan2(targetHitVec.z, targetHitVec.x)*Mathf.Rad2Deg;
        float myLookDeg = Mathf.Atan2(myLookVec.z, myLookVec.x) * Mathf.Rad2Deg;
        float degDiff = targetHitDeg - myLookDeg + (int)attackMotionType;
        while (degDiff < -180) degDiff += 360;
        while (degDiff > 180) degDiff -= 360;
        degDiff /= 180.0f;
        animator.SetFloat(GameManager.s_hit_rot,degDiff);
        animator.SetTrigger(GameManager.s_hit_additive);
        
        bool isBloodBottom = playerSmashedType is PlayerSmashedType.None or PlayerSmashedType.Bound or PlayerSmashedType.Stun;
        Effect_Hit_Strong(isBloodBottom);
    }
    public void FallDown()
    {
        falledTime = Time.time;
        Transform t = transform;
        p_smoke.transform.SetPositionAndRotation(t.position + t.forward*-0.2f ,t.rotation);
        p_smoke.Play();   
    }


    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Attack : Player_State_Base
{
    public bool isNormal = false;
    public int attackIndex = 0;
    private bool voiced = false;
    public MotionData_Attack attackData
    {
        get
        {
            return player.motionData;
        }
        set
        {
            player.isStrong = player.data_Weapon_Main.normalAttacks.Count - 1 == attackIndex;
            player.isSkill = false;
            player.motionData = value;
        }
    }
    private bool dashEffect = false;
    private TrailData lastTrailData = null;//중복공격 방지용 -> 공격 구별을 위해 사용
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        dashEffect = false;
        lastTrailData = null;
        voiced = false;
        if (player.data_Weapon_Main.normalAttacks.Count == attackIndex + 1)
        {
            player.audio_mat_armor.Play();
            player.audio_mat_fabric_compact.Play();
            player.audio_action_firering.Play();
            player.SuperArmor(true);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (finished) return;
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        animator.speed = attackData.animSpeed * attackData.animSpeedCurve.Evaluate(stateInfo.normalizedTime);
        
        if (attackData.dashEffectRatio>0 && !dashEffect && stateInfo.normalizedTime > attackData.dashEffectRatio)
        {
            dashEffect = true;
            player.Particle_Dash();
        } 
        if (stateInfo.normalizedTime > attackData.endRatio)
        {
            if (CheckSkill())
            {
                NextSkill(animator);
                finished = true;
                return;
            }
            else if (player.CanRoll())
            {
                player.Roll();
                finished = true;
                return;
            }
            else if (player.CanStrafe())
            {
                player.Strafe();
                finished = true;
                return;
            }
            else if (isNormal && CheckAttack())
            {
                NextAttack(animator);
                finished = true;
                return;
            }
            else
            {
                Exit(animator);
                finished = true;
                player.ChangeState(0);
            }
            return;
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (finished) return;
        base.OnStateMove(animator, stateInfo, layerIndex);
        UpdateTrail(attackData,stateInfo);
        if (player.target != null)
        {
            Vector3 rotVec = player.target.transform.position-player.transform.position;
            rotVec.y = 0;
            Quaternion targetRot = Quaternion.Lerp(player.transform.rotation,
                Quaternion.LookRotation(rotVec), 5 * Time.deltaTime);
            player.Move(IsCloseToTarget()?player.transform.position:
                player.transform.position+animator.deltaPosition*attackData.moveSpeed,targetRot);
        }
        else
        {
            player.Move(player.transform.position+animator.deltaPosition*attackData.moveSpeed,animator.rootRotation);
        }
    }
    public void UpdateTrail(MotionData_Attack data,AnimatorStateInfo stateInfo)
    {
        bool trail_L=false, trail_R=false, trail_S=false;
        foreach (var trailData in data.trails)
        {
            if (trailData.range.x > stateInfo.normalizedTime || stateInfo.normalizedTime >= trailData.range.y) continue;
            //공격대상 초기화
            if (lastTrailData != trailData)
            {
                player.attackedTarget.Clear();
                lastTrailData = trailData;
            }
            if (trailData.left) trail_L = true;
            if (trailData.right) trail_R = true;
            if (trailData.shield) trail_S = true;
            break;
        }

        if (!trail_L && !trail_R && !trail_S)
        {
            bool changeL = player.prefab_weaponL != null && player.prefab_weaponL.trailEffect.active;
            bool changeR = player.prefab_weaponR != null && player.prefab_weaponR.trailEffect.active;
            bool changeS = player.prefab_shield != null && player.prefab_shield.trailEffect.active;

            if (changeL || changeR || changeS) player.SuperArmor(false);

        }
        UpdateProp(player.prefab_weaponL,trail_L);
        UpdateProp(player.prefab_weaponR,trail_R);
        UpdateProp(player.prefab_shield,trail_S);
        void UpdateProp(Prefab_Prop prop,bool trailState)
        {
            if (prop != null)
            {
                bool change =prop.trailEffect.active != trailState;
                if (change)
                {
                    if (trailState)
                    {
                        if (!voiced)
                        {
                            if (player.data_Weapon_Main.normalAttacks.Count == attackIndex + 1 || player.isCharge)
                            {
                                voiced = true;
                            }
                            else if (attackIndex == 0 && PERCENT(50))
                            {
                                voiced = true;
                            }  
                        }
                        
                        prop.Trail_On();
                    }
                    else prop.Trail_Off();
                }
            }
        }
    }

    public void Exit(Animator animator)
    {
        if(player.prefab_weaponL != null) player.prefab_weaponL.Trail_Off();
        if(player.prefab_weaponR != null) player.prefab_weaponR.Trail_Off();
        if(player.prefab_shield != null) player.prefab_shield.Trail_Off();
        
        if(player.motionData.endMotionType != MotionData_Attack.MotionType.RIGHT) Canvas_Player_World.instance.Data_Default();
        animator.speed = 1.0f;
    }
    public bool CheckAttack()
    {
        if (player.death || player.clear) return false;
        return Time.time-Canvas_Player.RB_PressedTime<player.preInput_Attack
            && player.data_Weapon_Main.normalAttacks.Count > attackIndex + 1;
    }
    public bool CheckSkill()
    {
        if (player.death || player.clear) return false;
        bool canSkill = Time.unscaledTime - Canvas_Player.SB_PressedTime < player.preInput_Attack;
        bool skillGauge = (attackData.endMotionType == MotionData_Attack.MotionType.LEFT &&
                         Canvas_Player.instance.skillGauge_L.fullCharged)
                        || (attackData.endMotionType == MotionData_Attack.MotionType.RIGHT &&
                            Canvas_Player.instance.skillGauge_R.fullCharged);
        if (canSkill && !skillGauge) Canvas_Player.instance.InfoText(Canvas_Player.InfoType.No_SkillGauge);
        return canSkill && skillGauge;
    }
    public void NextAttack(Animator animator)
    {
        if (player.data_Weapon_Main.normalAttacks.Count == attackIndex + 2)
        {
            animator.SetBool("Charge",true);
            if (player.prefab_shield != null) player.prefab_shield.charge_Effect.Play();
            if (player.prefab_weaponL != null) player.prefab_weaponL.charge_Effect.Play();
            if (player.prefab_weaponR != null) player.prefab_weaponR.charge_Effect.Play();
            //CamArm.instance.Chromatic(0.03f,0.05f,0.5f);
        }
        animator.SetTrigger("Transition");
        Canvas_Player.RB_PressedTime = -100;
    }
    public void NextSkill(Animator animator)
    {
        animator.SetInteger("SkillState",(int)attackData.endMotionType);
        if (attackData.endMotionType == MotionData_Attack.MotionType.RIGHT) Canvas_Player.instance.skillGauge_R.Use();
        if (attackData.endMotionType == MotionData_Attack.MotionType.LEFT) Canvas_Player.instance.skillGauge_L.Use();
        player.ChangeState(2);
        Canvas_Player.SB_PressedTime = -100;
    }

    
}

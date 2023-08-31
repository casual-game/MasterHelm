using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Skill : Player_State_Base
{
    public enum SkillType { Left=0,Right=1}
    public SkillType skillType;
    public int skillIndex;
    private Prefab_Prop prefab_weaponL, prefab_weaponR, prefab_shield;

    private Data_Skill skill;
    private bool dashed = false, weaponOff = false;

    private Quaternion beginRot;
    private TrailData lastTrailData = null;//중복공격 방지용 -> 공격 구별을 위해 사
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        player.SetLeaning(false);
        dashed = false;
        weaponOff = false;

        switch (skillType)
        {
            case SkillType.Left:
                player.isLeftSkill = true;
                skill = player.data_Weapon_SkillL.SkillL;
                if (player.prefab_weaponL_SkillL != null) prefab_weaponL = player.prefab_weaponL_SkillL;
                if (player.prefab_weaponR_SkillL != null) prefab_weaponR = player.prefab_weaponR_SkillL;
                if (player.prefab_shield_SkillL != null) prefab_shield = player.prefab_shield_SkillL;
                if (skillIndex == 0)
                {
                    player.ChangeWeaponData(Player.CurrentWeaponData.Skill_L);
                }
                break;
            case SkillType.Right:
                player.isLeftSkill = false;
                skill = player.data_Weapon_SkillR.SkillR;
                if (player.prefab_weaponL_SkillR != null) prefab_weaponL = player.prefab_weaponL_SkillR;
                if (player.prefab_weaponR_SkillR != null) prefab_weaponR = player.prefab_weaponR_SkillR;
                if (player.prefab_shield_SkillR != null) prefab_shield = player.prefab_shield_SkillR;
                if(skillIndex==0) player.ChangeWeaponData(Player.CurrentWeaponData.Skill_R);
                break;
        }
        if (skillIndex == 0)
        {
            Manager_Main.instance.mainData.audio_Effecting_Impact.Play();
            player.audio_attack_skill.Play();
            player.audio_mat_armor.Play();
            player.audio_mat_fabric_compact.Play();
            player.audio_action_firering.Play();
            player.Particle_Smoke();
            player.skillBeginTime = Time.unscaledTime;
        }
        

        beginRot = player.transform.rotation;
        lastTrailData = null;

        player.isStrong = true;
        player.isSkill = true;
        player.isCharge = false;
        player.isRevengeSkill = false;
        player.skillData = skill.motions[skillIndex];
        Canvas_Player_World.instance.Update_Data();
        player.SuperArmor(true);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (finished) return;
        //Dash
        float dashRatio = skill.motions[skillIndex].dashRatio;
        if (!dashed && dashRatio>0 && stateInfo.normalizedTime > dashRatio)
        {
            dashed = true;
            player.Particle_Dash();
        }
        //WeaponOff
        if (!weaponOff && stateInfo.normalizedTime > skill.weaponOffRatio)
        {
            weaponOff = true;
            if(prefab_weaponL!=null) prefab_weaponL.On(false,null);
            if(prefab_weaponR!=null) prefab_weaponR.On(false,null);
            if(prefab_shield!=null) prefab_shield.On(false,null);
            
        }
        //Trail
        animator.speed = skill.motions[skillIndex].speedCurve.Evaluate(stateInfo.normalizedTime)
                         * skill.motions[skillIndex].animSpeed;
        UpdateTrail(skill.motions[skillIndex],stateInfo,prefab_weaponL,prefab_weaponR,prefab_shield);
        //DoubleSkill
        bool canInput = skillIndex == skill.motions.Count-1
                        &&skill.motions[skillIndex].canInput.x <= stateInfo.normalizedTime
                        && stateInfo.normalizedTime < skill.motions[skillIndex].canInput.y;
        bool coolTime = (skill.endMotionType == Data_Skill.MotionType.LEFT &&
                         Canvas_Player.instance.skillGauge_L.fullCharged)
                        || (skill.endMotionType == Data_Skill.MotionType.RIGHT &&
                            Canvas_Player.instance.skillGauge_R.fullCharged);
        if (canInput && Time.unscaledTime - Canvas_Player.SB_PressedTime < player.preInput_Attack && coolTime)
        {
            animator.SetInteger("SkillState",(int)skill.endMotionType);
            player.ChangeState(2);
            if (skill.endMotionType == Data_Skill.MotionType.RIGHT) Canvas_Player.instance.skillGauge_R.Use();
            if (skill.endMotionType == Data_Skill.MotionType.LEFT) Canvas_Player.instance.skillGauge_L.Use();
            finished = true;
            return;
        }
        //Roll
        if (canInput && skill.motions.Count <= skillIndex + 1 && player.CanRoll())
        {
            player.Roll();
            player.ChangeWeaponData(Player.CurrentWeaponData.Main);
            finished = true;
            return;
        }
        else if (canInput && player.CanStrafe())
        {
            player.Strafe();
            finished = true;
            return;
        }
        //NormalAttack
        bool normalInputCheck = Canvas_Player.RB_Pressed ||
                                Time.time - Canvas_Player.RB_PressedTime < player.preInput_Attack;
        if (canInput && normalInputCheck && !player.death && !player.clear)
        {
            bool CanCombo=false;
            switch (skill.endMotionType)
            {
                case Data_Skill.MotionType.LEFT:
                    if(!player.leftComboIndex.HasValue) break;
                    player.ChangeWeaponData(Player.CurrentWeaponData.Main);
                    animator.SetInteger("ComboIndex",player.leftComboIndex.Value);
                    CanCombo = true;
                    break;
                case Data_Skill.MotionType.RIGHT:
                    if(!player.rightComboIndex.HasValue) break;
                    player.ChangeWeaponData(Player.CurrentWeaponData.Main);
                    animator.SetInteger("ComboIndex",player.rightComboIndex.Value);
                    CanCombo = true;
                    break;
            }

            if (CanCombo)
            {
                player.ChangeState(1);
                finished = true;
                return;
            }
        }
        //End
        if (skill.motions[skillIndex].endRatio < stateInfo.normalizedTime)
        {
            finished = true;
            if (skill.motions.Count > skillIndex + 1)
            {
                animator.SetTrigger("Transition");
            }
            else
            {
                player.ChangeState(0);
                player.ChangeWeaponData(Player.CurrentWeaponData.Main);
                if(player.motionData.endMotionType != MotionData_Attack.MotionType.RIGHT) 
                    Canvas_Player_World.instance.Data_Default();
            }

            return;
        }
    }
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (finished) return;
        float ratio = Mathf.Clamp01(stateInfo.normalizedTime / skill.motions[0].trails[0].range.x);
        if (ratio < 0.99f)
        {
            if (player.target != null)
            {
                Vector3 rotVec = player.target.transform.position-player.transform.position;
                rotVec.y = 0;
                Quaternion targetRot = Quaternion.Lerp(beginRot,Quaternion.LookRotation(rotVec),ratio);
                player.Move(IsCloseToTarget()?player.transform.position:
                    player.transform.position+animator.deltaPosition*skill.motions[skillIndex].moveSpeed,targetRot);
            }
            else
            {
                player.Move(player.transform.position+animator.deltaPosition*skill.motions[skillIndex].moveSpeed,animator.rootRotation);
            }
        }
        else
        {
            if (player.target != null)
            {
                Vector3 rotVec = player.target.transform.position-player.transform.position;
                rotVec.y = 0;
                Quaternion targetRot = Quaternion.Lerp(player.transform.rotation,
                    Quaternion.LookRotation(rotVec), 5 * Time.deltaTime);
                player.Move(IsCloseToTarget()?player.transform.position:
                    player.transform.position+animator.deltaPosition*skill.motions[skillIndex].moveSpeed,targetRot);
            }
            else
            {
                player.Move(player.transform.position+animator.deltaPosition*skill.motions[skillIndex].moveSpeed,animator.rootRotation);
            }
        }
    }
    

    public void UpdateTrail(SkillData data,AnimatorStateInfo stateInfo,
        Prefab_Prop weapon_L,Prefab_Prop weapon_R,Prefab_Prop shield)
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

        if (lastTrailData != null && !trail_L && !trail_R && !trail_S)
        {
            player.SuperArmor(false);
        }
        UpdateProp(weapon_L,trail_L);
        UpdateProp(weapon_R,trail_R);
        UpdateProp(shield,trail_S);
        void UpdateProp(Prefab_Prop prop,bool trailState)
        {
            if (prop != null)
            {
                
                bool change =prop.trailEffect.active != trailState;
                if (change)
                {
                    if (trailState)
                    {
                        prop.Trail_On();
                        
                    }
                    else prop.Trail_Off();
                }
            }
        }
        
    }
}

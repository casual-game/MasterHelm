using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Dest.Math;
using FIMSpace;
using HighlightPlus;
using Pathfinding;
using RootMotion.Dynamics;
using Random = UnityEngine.Random;
public partial class Player : MonoBehaviour
{
    [FoldoutGroup("Audio")] public Data_Audio 
        audio_action_footstep_rock,audio_action_roll,audio_action_firering,audio_action_timing,
        audio_attack_normal,audio_attack_strong,audio_attack_skill,audio_attack_additional,audio_attack_timing,
        audio_attack_damaged_normal,audio_attack_damaged_strong,
        audio_mat_armor,audio_mat_fabric_smooth,audio_mat_fabric_compact,
        audio_Hit_Gore,audio_Hit_Impact,audio_Hit_Notice,audio_Hit_Special,audio_Hit_Finish,audio_Hit_Spark,
        audio_Start,audio_StageClear,audio_AreaClear,audio_Ready,audio_BossFin,audio_Bow_Ready,audio_Bow_Shoot;
    private float voiceDelay = 1.5f,lastVoiceTime=0;
    private int footstepIndex = 0;
    private LayerMask layer_water;
    public void Setting_Sound()
    {
        SoundManager.instance.Add(audio_action_footstep_rock);
        SoundManager.instance.Add(audio_action_roll);
        SoundManager.instance.Add(audio_attack_normal);
        SoundManager.instance.Add(audio_attack_strong);
        SoundManager.instance.Add(audio_attack_skill);
        SoundManager.instance.Add(audio_attack_additional);
        SoundManager.instance.Add(audio_attack_damaged_normal);
        SoundManager.instance.Add(audio_attack_damaged_strong);
        SoundManager.instance.Add(audio_attack_timing);
        SoundManager.instance.Add(audio_mat_armor);
        SoundManager.instance.Add(audio_mat_fabric_smooth);
        SoundManager.instance.Add(audio_mat_fabric_compact);
        SoundManager.instance.Add(audio_action_firering);
        SoundManager.instance.Add(audio_action_timing);
        SoundManager.instance.Add(audio_Hit_Gore);
        SoundManager.instance.Add(audio_Hit_Impact);
        SoundManager.instance.Add(audio_Hit_Spark);
        
        SoundManager.instance.Add(audio_Start);
        SoundManager.instance.Add(audio_StageClear);
        SoundManager.instance.Add(audio_Ready);
        SoundManager.instance.Add(audio_BossFin);
        
        SoundManager.instance.Add(audio_Bow_Ready);
        SoundManager.instance.Add(audio_Bow_Shoot);
        SoundManager.instance.Add(audio_AreaClear);
        layer_water = LayerMask.NameToLayer("Water");
    }
    public void Footstep_L()
    {
        Footstep_Core(T_Foot_L);
    }
    public void Footstep_R()
    {
        Footstep_Core(T_Foot_R);
    }
    private void Footstep_Core(Transform t)
    {
        if (Canvas_Player.LS_Scale <= 0.1f) return;
        bool canPlay = !animator.IsInTransition(0);
        float volume = 1;
        float pitch = 1;
        RaycastHit hit;
        bool detectWater = Physics.Raycast(t.position+Vector3.up*0.5f,Vector3.down,out hit,
            1.0f,layer_water,QueryTriggerInteraction.Collide);
        if (detectWater&&hit.collider.gameObject.transform.position.y>transform.position.y-0.05f)
        {
            if (canPlay)
            {
                //물소리
            }
            particle_WaterRipples.Play();
            particle_WaterRipples.transform.position = new Vector3(t.transform.position.x,
                hit.point.y-0.04f, T_Foot_R.transform.position.z);
        }
        else
        {
            //바닥 소리
            audio_action_footstep_rock.Play();
            particle_Footstep.transform.position = t.transform.position;
            particle_Footstep.Play();
        }

        footstepIndex = (footstepIndex + 1) % 2;
        if (footstepIndex == 0) audio_mat_fabric_compact.Play(0.35f);
    }

    //애니메이션 이벤트
    public void AttackSound()
    {
        Data_Audio audio;
        switch (currentWeaponData)
        {
            case CurrentWeaponData.Main:
                audio = data_Weapon_Main.swingSound;
                break;
            case CurrentWeaponData.Skill_L:
                audio = data_Weapon_SkillL.swingSound;
                break;
            case CurrentWeaponData.Skill_R:
                audio = data_Weapon_SkillR.swingSound;
                break;
            default:
                audio = null;
                break;
        }
        if(audio!=null) audio.Play();
        if (isStrong)
        {
            if(!isSkill) audio_attack_strong.Play();
            else audio_attack_additional.Play();
        }
        else if (Time.unscaledTime > lastVoiceTime + voiceDelay)
        {
            lastVoiceTime = Time.unscaledTime;
            audio_attack_normal.Play();
        }
    }
    public void HitSound(bool hitStrong)
    {
        if (hitStrong)
        {
            audio_attack_damaged_strong.Play();
        }
        else if (Time.unscaledTime > lastVoiceTime + voiceDelay)
        {
            lastVoiceTime = Time.unscaledTime;
            audio_attack_damaged_normal.Play();
        } 
    }
}

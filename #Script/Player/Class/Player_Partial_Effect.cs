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
using Sirenix.Utilities;

public partial class Player : MonoBehaviour
{
    private ParticleSystem particle_parrying, particle_guard,particle_smoke,particle_roll,particle_charge;
    private Renderer[] renderer_weapon, renderer_skillL, renderer_skillR,renderer_Bow;
    private void Setting_Particle()
    {
        particleParent = transform.Find("Particle");
        particle_roll = particleParent.Find("Roll").GetComponent<ParticleSystem>();
        particle_guard = particleParent.Find("Guard").GetComponent<ParticleSystem>();
        particle_parrying = particleParent.Find("Parrying").GetComponent<ParticleSystem>();
        particle_smoke= particleParent.Find("Smoke").GetComponent<ParticleSystem>();
        particle_Dash = particleParent.Find("Dash").GetComponent<ParticleSystem>();
        particle_Footstep= particleParent.Find("Footstep").GetComponent<ParticleSystem>();
        particle_WaterRipples= particleParent.Find("WaterRipples").GetComponent<ParticleSystem>();
        particle_FireRing= particleParent.Find("FireRing").GetComponent<ParticleSystem>();
        particle_FireRingColor = particle_FireRing.transform.GetChild(0).GetComponent<ParticleSystem>();
        particle_charge= particleParent.Find("Charge").GetComponent<ParticleSystem>();
        particle_charge.transform.SetParent(T_Back);
        particle_FireRing.transform.parent = Manager_Main.instance._folder_;

        #region Renderer 설정
        //임시 부위별 렌더러 설정
        SkinnedMeshRenderer[] _rs_player = transform.Find("Deco").GetComponentsInChildren<SkinnedMeshRenderer>(false);
        MeshRenderer[] _rs_weaponL = prefab_weaponL == null ? null : prefab_weaponL.GetComponentsInChildren<MeshRenderer>(true);
        MeshRenderer[] _rs_weaponR = prefab_weaponR == null ? null : prefab_weaponR.GetComponentsInChildren<MeshRenderer>(true);
        MeshRenderer[] _rs_shield = prefab_shield == null ? null : prefab_shield.GetComponentsInChildren<MeshRenderer>(true);
        SkinnedMeshRenderer[] _rs_bow = prefab_bow == null ? null : prefab_bow.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        
        MeshRenderer[] _rs_skillL_weaponL = prefab_weaponL_SkillL == null ? null : prefab_weaponL_SkillL.GetComponentsInChildren<MeshRenderer>(true);
        MeshRenderer[] _rs_skillL_weaponR = prefab_weaponL_SkillR == null ? null : prefab_weaponR_SkillL.GetComponentsInChildren<MeshRenderer>(true);
        
        MeshRenderer[] _rs_skillR_weaponL = prefab_weaponR_SkillL == null ? null : prefab_weaponL_SkillR.GetComponentsInChildren<MeshRenderer>(true);
        MeshRenderer[] _rs_skillR_weaponR = prefab_weaponR_SkillR == null ? null : prefab_weaponR_SkillR.GetComponentsInChildren<MeshRenderer>(true);
        //임시 리스트에 병합
        List<Renderer> l_renderer_weapon = new List<Renderer>();
        if(_rs_player!=null) l_renderer_weapon.AddRange(_rs_player);
        if(_rs_weaponL!=null) l_renderer_weapon.AddRange(_rs_weaponL);
        if(_rs_weaponR!=null) l_renderer_weapon.AddRange(_rs_weaponR);
        if(data_Weapon_Main.useShield && _rs_shield!=null) l_renderer_weapon.AddRange(_rs_shield);
        List<Renderer> l_renderer_skillL = new List<Renderer>();
        if(_rs_player!=null) l_renderer_skillL.AddRange(_rs_player);
        if(_rs_skillL_weaponL!=null) l_renderer_skillL.AddRange(_rs_skillL_weaponL);
        if(_rs_skillL_weaponR!=null) l_renderer_skillL.AddRange(_rs_skillL_weaponR);
        if(data_Weapon_SkillL.useShield && _rs_shield!=null) l_renderer_skillL.AddRange(_rs_shield);
        List<Renderer> l_renderer_skillR = new List<Renderer>();
        if(_rs_player!=null) l_renderer_skillR.AddRange(_rs_player);
        if(_rs_skillR_weaponL!=null) l_renderer_skillR.AddRange(_rs_skillR_weaponL);
        if(_rs_skillR_weaponR!=null) l_renderer_skillR.AddRange(_rs_skillR_weaponR);
        if(data_Weapon_SkillR.useShield && _rs_shield!=null) l_renderer_skillR.AddRange(_rs_shield);
        List<Renderer> l_renderer_Bow = new List<Renderer>();
        if(_rs_player!=null) l_renderer_Bow.AddRange(_rs_player);
        if(_rs_bow!=null) l_renderer_Bow.AddRange(_rs_bow);
        //배열에 적용
        renderer_weapon = l_renderer_weapon.ToArray();
        renderer_skillL = l_renderer_skillL.ToArray();
        renderer_skillR = l_renderer_skillR.ToArray();
        renderer_Bow = l_renderer_Bow.ToArray();
        #endregion

    }
    public void Highlight_ChangeRenderer(CurrentWeaponData weaponData)
    {
        switch (weaponData)
        {
            case CurrentWeaponData.Main:
                highlight.SetTargets(transform,renderer_weapon);
                break;
            case CurrentWeaponData.Skill_L:
                highlight.SetTargets(transform,renderer_skillL);
                break;
            case CurrentWeaponData.Skill_R:
                highlight.SetTargets(transform,renderer_skillR);
                break;
            case CurrentWeaponData.Bow:
                highlight.SetTargets(transform,renderer_Bow);
                break;
        }
        highlight.highlighted = true;
    }

    public void Particle_Smoke()
    {
        particle_smoke.Play();
    }

    public void Particle_ChargeBegin()
    {
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Clear_Stage);
        particle_charge.Play();
        CamArm.instance.Production_Begin(1.15f);
    }
    public void Particle_ChargeFin()
    {
        particle_charge.Stop();
        Particle_Smoke();
        Particle_FireRing(transform.position + Vector3.up);
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Smash);
        CamArm.instance.SpeedLine_Special();
    }
    public void Particle_Start()
    {
        Particle_Smoke();
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Smash);
        CamArm.instance.SpeedLine_Play(false);
    }
    public void Particle_Roll()
    {
        particle_roll.Play();
    }
    
    public void Particle_Dash(float dist=-0.893f)
    {
        Vector3 pos = particle_Dash.transform.localPosition;
        pos.y = dist;
        particle_Dash.transform.localPosition = pos;
        particle_Dash.Play();
    }
    public void Particle_FireRing(Vector3 pos)
    {
        //if (currentWeaponData == CurrentWeaponData.Bow) return;
        Color color = Color.white;
        switch (currentWeaponData)
        {
            case CurrentWeaponData.Main:
                if (prefab_weaponR != null) color = prefab_weaponR.color;
                else if (prefab_weaponL != null) color = prefab_weaponL.color;
                else if (prefab_shield != null) color = prefab_shield.color;
                break;
            case CurrentWeaponData.Skill_L:
                if (prefab_weaponR_SkillL != null) color = prefab_weaponR_SkillL.color;
                else if (prefab_weaponL_SkillL != null) color = prefab_weaponL_SkillL.color;
                else if (prefab_shield_SkillL != null) color = prefab_shield_SkillL.color;
                break;
            case CurrentWeaponData.Skill_R:
                if (prefab_weaponR_SkillR != null) color = prefab_weaponR_SkillR.color;
                else if (prefab_weaponL_SkillR != null) color = prefab_weaponL_SkillR.color;
                else if (prefab_shield_SkillR != null) color = prefab_shield_SkillR.color;
                break;
            case CurrentWeaponData.Bow:
                color = prefab_bow.color;
                break;
        }

        var _main = particle_FireRingColor.main;
        _main.startColor = new ParticleSystem.MinMaxGradient(color);
        particle_FireRing.transform.position = pos;
        particle_FireRing.transform.rotation = Quaternion.Euler(90,0,0);
        particle_FireRing.Play();
        
    }
    public void Particle_JustParrying()
    {
        //파티클 위치 설정
        particle_parrying.Play();
        Vector3 newLocalPos = particle_parrying.transform.localPosition;
        newLocalPos.y = -1.461f;
        particle_parrying.transform.localPosition = newLocalPos;
    }
    public void Particle_Guard(Vector3 enemyPos)
    {
        //sfx
        HitSound(false);
        audio_Hit_Impact.Play();
        //vfx
        Vector3 numPos = transform.position - enemyPos;
        numPos = transform.position + numPos.normalized * 0.5f;
        
        highlight.HitFX(Manager_Main.instance.mainData.player_RollColor, 0.5f);
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Guard);
        
        //파티클 위치 설정
        particle_guard.Play();
        Vector3 newLocalPos = particle_guard.transform.localPosition;
        newLocalPos.y = -1.281f;
        particle_guard.transform.localPosition = newLocalPos;
        Manager_Main.instance.Text_Damage_Specific("guard");
    }

    public void Particle_SuperArmorHit()
    {
        //sfx
        audio_Hit_Impact.Play();
        //vfx
        Particle_Blood_Normal();
        Particle_Smoke();
        Manager_Pooler.instance.GetParticle("Spark", 
            Player.instance.transform.position + Vector3.up * 1.2f,Quaternion.identity);
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Hit);
        highlight.HitFX(Manager_Main.instance.mainData.blink_Damage,0.5f,1.5f);
        Manager_Main.instance.Text_Damage_Specific("super armor");
    }
    public void Particle_Hit_Normal()
    {
        //sfx
        HitSound(false);
        audio_Hit_Gore.Play();
        audio_Hit_Impact.Play();
        //vfx
        Particle_Blood_Normal();
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Hit);
        //CamArm.instance.Hit(0.05f,0.5f,Manager_Main.instance.mainData.hitFX_Hit_Normal);
        highlight.HitFX(Manager_Main.instance.mainData.blink_Damage,0.5f,1.5f);
        
        //파티클 위치 설정
        particle_guard.Play();
        Vector3 newLocalPos = particle_guard.transform.localPosition;
        newLocalPos.y = -0.65f;
        particle_guard.transform.localPosition = newLocalPos;
    }
    public void Particle_Hit_Strong()
    {
        //sfx
        HitSound(true);
        audio_Hit_Gore.Play();
        audio_Hit_Impact.Play();
        //vfx
        Particle_Blood_Smash();
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Smashed);
        highlight.HitFX(Manager_Main.instance.mainData.blink_Damage,0.5f,1.5f);
        
        //파티클 위치 설정
        particle_Dash.Play();
        particle_parrying.Play();
        Vector3 newLocalPos = particle_parrying.transform.localPosition;
        newLocalPos.y = -0.65f;
        particle_parrying.transform.localPosition = newLocalPos;
    }
    public void Particle_GuardBreak()
    {
        //sfx
        HitSound(true);
        audio_Hit_Gore.Play();
        audio_Hit_Impact.Play();
        audio_Hit_Notice.Play();
        //vfx
        Particle_Blood_Smash();
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Smashed);
        highlight.HitFX(Manager_Main.instance.mainData.blink_Damage,1.0f,1.5f);
        
        //파티클
        particle_Dash.Play();
        particle_parrying.Play();
        particle_smoke.Play();
        Vector3 newLocalPos = particle_parrying.transform.localPosition;
        newLocalPos.y = -0.65f;
        particle_parrying.transform.localPosition = newLocalPos;
    }

    public void Particle_GuardBreakRevenge()
    {
        audio_action_timing.Play();
        Particle_FireRing(transform.position + Vector3.up*0.5f);
        Manager_Main.instance.Text_Highlight(transform.position + Vector3.up,"REVENGE");
        CamArm.instance.SpeedLine_Play(false,target.transform);
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Revenge);
        Manager_Main.instance.Text_Damage_Specific("evade");
    }
    public void Particle_GuardBreakEvade()
    {
        audio_action_timing.Play();
        audio_attack_timing.Play();
        Particle_FireRing(T_Head.position);
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Revenge);
        CamArm.instance.SpeedLine_Play(false,rollVec);
        Manager_Main.instance.Text_Big(transform.position + Vector3.up,"EVADE");
        Manager_Main.instance.Text_Damage_Specific("evade");
    }
    
    private void Particle_Blood_Normal()
    {
        GameObject blood = Manager_Blood.instance.Blood_Hit();
        blood.transform.position = transform.position + Vector3.up * 1.5f;
        if (blood != null)
        {
            blood.transform.rotation = Quaternion.LookRotation(-transform.forward)*Quaternion.Euler(0,-90,0);
            blood.SetActive(true);
        }
    }
    private void Particle_Blood_Smash()
    {
        GameObject blood = Manager_Blood.instance.Blood_Smash();
        blood.transform.position = transform.position + Vector3.up * 1.5f;
        if (blood != null)
        {
            blood.transform.rotation = Quaternion.LookRotation(-transform.forward)*Quaternion.Euler(0,-90,0);
            blood.SetActive(true);
        }
        
    }
}

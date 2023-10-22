using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TrailsFX;
using UnityEngine;
using UnityEngine.Serialization;

public partial class Hero : MonoBehaviour
{
    private void Setting_Effect()
    {
        trailEffect = transform.Find("SkinnedMesh").GetComponent<TrailEffect>();
        trailEffect.active = false;
        s_blink_hit_normal = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, c_hit_begin); })
            .Append(_outlinable.FrontParameters.FillPass
                .DOColor(GameManager.s_publiccolor, c_hit_fin, 0.3f).SetEase(Ease.InQuad));
        s_blink_hit_strong = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, c_hit_begin); })
            .Append(_outlinable.FrontParameters.FillPass
                .DOColor(GameManager.s_publiccolor, c_hit_fin, 0.5f).SetEase(Ease.InCirc));
        s_blink_evade = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, c_evade_begin); })
            .Append(_outlinable.FrontParameters.FillPass
                .DOColor(GameManager.s_publiccolor, c_evade_fin, 0.45f).SetEase(Ease.InQuad));

        var trailModule = p_charge.trails;
        trailModule.colorOverTrail = weaponPack_Normal.mainGradient;

        parr_despawn = p_despawn.GetComponentsInChildren<ParticleSystem>();
    }
    
    //Public
    [HideInInspector]
    public TrailEffect trailEffect;

    [FoldoutGroup("Particle")] 
    public ParticleSystem p_charge_begin, p_charge_fin, p_charge_Impact,p_charge,p_spawn,p_despawn
        ,p_smoke,p_roll,p_change,p_footstep_l,p_footstep_r,p_blood_normal,p_blood_strong;

    private ParticleSystem[] parr_despawn;

    [FoldoutGroup("Color")][ColorUsage(true,true)] 
    public Color c_hit_begin,c_hit_fin,c_evade_begin,c_evade_fin;
    
    //Private
    private Sequence s_blink_hit_normal,s_blink_hit_strong,s_blink_evade;

    private void Set_Particle(ref ParticleSystem[] parr,float simulationSpeed,float startDelay)
    {
        foreach (var p in parr)
        {
            var main = p.main;
            main.simulationSpeed = simulationSpeed;
            main.startDelay = startDelay;
        }
    }
    //Effect
    public void Effect_AttackParticle(int index)
    {
        if (_animator.IsInTransition(0)) return;
        var mainParticle = weapondata[_currentWeaponPack].attackParticles[index];
        if (mainParticle == null) return;

        Transform t = transform;
        mainParticle.transform.SetPositionAndRotation(t.position, t.rotation);
        mainParticle.Play();
    }
    public void Effect_Footstep_L()
    {
        p_footstep_l.Play();
    }
    public void Effect_Footstep_R()
    {
        p_footstep_r.Play();
    }
    public void Effect_Roll()
    {
        Transform t = transform;
        p_roll.transform.SetPositionAndRotation(t.position + t.forward * 0.5f,t.rotation);
        p_roll.Play();
    }
    public void Effect_FastRoll()
    {
        if(!s_blink_evade.IsInitialized()) s_blink_evade.Play();
        else s_blink_evade.Restart();
    }
    public void Effect_Change()
    {
        Transform t = transform;
        p_change.transform.SetPositionAndRotation(t.position + Vector3.up,t.rotation);
        p_change.Play();
    }
    public void Effect_Smoke(float fwd = 0)
    {
        Transform t = transform;
        p_smoke.transform.position = t.position + t.forward*fwd;
        p_smoke.Play();
    }
    public void Effect_Hit_Normal()
    {
        if(!s_blink_hit_normal.IsInitialized()) s_blink_hit_normal.Play();
        else s_blink_hit_normal.Restart();
        
        Transform t = transform;
        Vector3 currentPos = t.position;
        p_blood_normal.Play();
        
        //Blood
        Vector3 bloodPos = currentPos + Vector3.up * 0.8f;
        Quaternion bloodRot = Quaternion.Euler(0,Random.Range(0,360),0);
        BloodManager.instance.Blood_Normal(ref bloodPos,ref bloodRot);
    }
    public void Effect_Hit_Strong(bool isBloodBottom)
    {
        if(!s_blink_hit_strong.IsInitialized()) s_blink_hit_strong.Play();
        else s_blink_hit_strong.Restart();
        Transform t = transform;
        Vector3 currentPos = t.position;
        p_blood_normal.Play();
        p_blood_strong.Play();
        //Blood
        Vector3 bloodPos = currentPos + Vector3.up * 0.8f;
        Quaternion bloodRot;
        if (isBloodBottom)
        {
            bloodRot = Quaternion.Euler(0,Random.Range(0,360),0);
            BloodManager.instance.Blood_Strong_Bottom(ref bloodPos,ref bloodRot);
        }
        else
        {
            bloodRot = t.rotation;
            BloodManager.instance.Blood_Strong_Front(ref bloodPos,ref bloodRot);
        }
    }
    
}

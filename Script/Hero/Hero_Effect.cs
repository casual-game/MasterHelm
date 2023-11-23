using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using TrailsFX;
using UnityEngine;
using UnityEngine.Serialization;

public partial class Hero : MonoBehaviour
{
    private void Setting_Effect()
    {
        _meshRoot = transform.GetChild(0);
        trailEffect = GetComponentInChildren<SkinnedMeshRenderer>().GetComponent<TrailEffect>();
        trailEffect.active = false;
        var trailModuleM = p_charge_main.trails;
        trailModuleM.colorOverTrail = weaponPack_Normal.colorOverTrail;
        trailModuleM.colorOverLifetime = weaponPack_Normal.colorOverLifetime;
        var trailModuleL = p_charge_strongL.trails;
        trailModuleL.colorOverTrail = weaponPack_StrongL.colorOverTrail;
        trailModuleL.colorOverLifetime = weaponPack_StrongL.colorOverLifetime;
        var trailModuleR = p_charge_strongR.trails;
        trailModuleR.colorOverTrail = weaponPack_StrongR.colorOverTrail;
        trailModuleR.colorOverLifetime = weaponPack_StrongR.colorOverLifetime;

        mat_superarmor = new Material(mat_superarmor);
        var smr = GetComponentInChildren<SkinnedMeshRenderer>();
        Material[] newmats = new Material[2];
        newmats[0] = smr.material;
        newmats[1] = mat_superarmor;
        smr.materials = newmats;
        UpdateWeaponMat(weaponPack_Normal,mat_superarmor);
        UpdateWeaponMat(weaponPack_StrongL,mat_superarmor);
        UpdateWeaponMat(weaponPack_StrongR,mat_superarmor);
        if(shield!=null) UpdatePropMat(shield,mat_superarmor);
        id_superarmor = Shader.PropertyToID(GameManager.s_maincolor);
        void UpdateWeaponMat(Data_WeaponPack weaponPack,Material mat)
        {
            var wp = weapondata[weaponPack];
            if (wp.weaponL != null) UpdatePropMat(wp.weaponL,mat);
            if (wp.weaponR != null) UpdatePropMat(wp.weaponR,mat);
        }
        void UpdatePropMat(Prefab_Prop prop, Material mat)
        {
            var mr = prop.GetComponent<MeshRenderer>();
            Material[] propmats = new Material[2];
            propmats[0] = mr.material;
            propmats[1] = mat;
            mr.materials = propmats;
        }
    }
    
    //Public
    private TrailEffect trailEffect;
    [FoldoutGroup("Particle")] public Material mat_superarmor;
    [FoldoutGroup("Particle")] 
    public ParticleSystem p_charge_begin, p_charge_fin, p_charge_Impact,p_superarmor;

    [FoldoutGroup("Particle")][SerializeField]
    private ParticleSystem p_charge_main,p_charge_strongL,p_charge_strongR;

    [FoldoutGroup("Particle")] 
    public ParticleSystem p_spawn,p_despawn
        ,p_smoke,p_roll,p_change,p_footstep_l,p_footstep_r,p_blood_normal,p_blood_strong;

    [FoldoutGroup("Color")][ColorUsage(true,true)] 
    public Color c_hit_begin,c_hit_fin,c_evade_begin,c_evade_fin,c_superarmor_deactivated,c_superarmor_activated;

    //Private
    private Tween t_blink,t_punch;
    private Sequence s_trail,s_superarmor;
    private Transform _meshRoot;
    private bool _superarmor = false;

    private int id_superarmor;
    //Tween
    public void Tween_Blink_Hit(float timeScale)
    {
        t_blink.Complete();
        t_blink = Tween.Custom(c_hit_begin, c_hit_fin, duration: 0.5f,
                onValueChange: newVal => _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, newVal)
                ,ease: Ease.InCirc);
        t_blink.timeScale = 1.2f;
    }
    public void Tween_Blink_Evade(float timeScale)
    {
        t_blink.Complete();
        t_blink = Tween.Custom(c_evade_begin, c_evade_fin, duration: 0.45f,
            onValueChange: newVal => _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, newVal)
            ,ease: Ease.InQuad,useUnscaledTime:true);
        t_blink.timeScale = 1.2f;
    }
    public void Tween_Punch_Down(float speed)
    {
        t_punch.Complete();
        _meshRoot.localScale = GameManager.V3_One;
        t_punch = Tween.PunchScale(_meshRoot, strength: new Vector3(0.15f, 0.15f, -0.15f),
            duration: 0.75f, frequency: 7, easeBetweenShakes: Ease.OutQuad,useUnscaledTime:true);
        t_punch.timeScale = speed;
    }
    public void Tween_Punch_Up(float speed)
    {
        t_punch.Complete();
        _meshRoot.localScale = GameManager.V3_One;
        t_punch = Tween.PunchScale(_meshRoot, strength: new Vector3(-0.15f, -0.15f, 0.15f),
            duration: 0.75f, frequency: 7, easeBetweenShakes: Ease.OutQuad,useUnscaledTime:true);
        t_punch.timeScale = speed;
    }
    public void Tween_Punch_Down_Compact(float speed)
    {
        t_punch.Complete();
        _meshRoot.localScale = GameManager.V3_One;
        t_punch = Tween.PunchScale(_meshRoot, strength: new Vector3(0.125f,0.125f,-0.125f),
            duration: 0.45f, frequency: 7, easeBetweenShakes: Ease.OutSine,useUnscaledTime:true);
        t_punch.timeScale = speed;
    }
    public void Tween_Punch_Up_Compact(float speed)
    {
        t_punch.Complete();
        _meshRoot.localScale = GameManager.V3_One;
        t_punch = Tween.PunchScale(_meshRoot, strength: new Vector3(-0.125f,-0.125f,0.125f),
            duration: 0.45f, frequency: 7, easeBetweenShakes: Ease.OutSine,useUnscaledTime:true);
        t_punch.timeScale = speed;
    }
    public void Tween_Trail(float duration)
    {
        s_trail.Stop();
        trailEffect.active = true;
        s_trail = Sequence.Create().ChainDelay(duration, true)
            .ChainCallback(target: this, target => target.trailEffect.active = false);
    }
    public static void Blink(float duration)
    {
        instance.t_blink.Complete();
        instance.t_blink = Tween.Custom(instance.c_evade_begin, instance.c_evade_fin,duration,
            onValueChange: newVal => instance._outlinable.FrontParameters.FillPass
                .SetColor(GameManager.s_publiccolor, newVal),ease: Ease.InQuad);
    }
    //Effect
    public void Effect_AttackParticle(int index)
    {
        if (_animator.IsInTransition(0) || !(HeroMoveState is MoveState.NormalAttack or MoveState.StrongAttack)) return;
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
    public void Effect_SuperArmor(bool activate)
    {
        if (!activate && !_superarmor) return;
        _superarmor = activate;
        
        if(activate)
        {
            s_superarmor.Stop();
            Transform t = transform;
            p_change.transform.SetPositionAndRotation(t.position + Vector3.up,t.rotation);
            p_change.Play();
            p_superarmor.Play();
            s_superarmor = Sequence.Create()
                .Chain(Tween.MaterialColor(mat_superarmor, id_superarmor, c_superarmor_activated, 0.25f));
        }
        else
        {
            p_superarmor.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            s_superarmor = Sequence.Create()
                .Chain(Tween.MaterialColor(mat_superarmor, id_superarmor, c_superarmor_deactivated, 0.25f));
        }
    }
    public void Effect_Smoke(float fwd = 0)
    {
        Transform t = transform;
        p_smoke.transform.position = t.position + t.forward*fwd;
        p_smoke.Play();
    }
    public void Effect_Hit_Normal()
    {
        Tween_Blink_Hit(1.75f);
        p_blood_normal.Play();
    }
    public void Effect_Hit_Strong(bool isBloodBottom)
    {
        Tween_Blink_Hit(1.0f);
        p_blood_normal.Play();
        p_blood_strong.Play();
    }
    public void Effect_Hit_SuperArmor()
    {
        p_blood_normal.Play();
        p_blood_strong.Play();
    }
    public void Effect_Land()
    {
        Tween_Punch_Up(1.25f);
        Effect_Smoke();
    }
    //Particle
    public void Particle_Charge_Main()
    {
        if(p_charge_strongL.isPlaying) p_charge_strongL.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        if(p_charge_strongR.isPlaying) p_charge_strongR.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        p_charge_main.Play();
    }
    public void Particle_Charge_L()
    {
        if(p_charge_main.isPlaying) p_charge_main.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        if(p_charge_strongR.isPlaying) p_charge_strongR.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        p_charge_strongL.Play();
    }
    public void Particle_Charge_R()
    {
        if(p_charge_strongL.isPlaying) p_charge_strongL.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        if(p_charge_main.isPlaying) p_charge_main.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        p_charge_strongR.Play();
    }


    
}

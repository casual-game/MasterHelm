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
        var smr = GetComponentInChildren<SkinnedMeshRenderer>();
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

        customMaterialController = GetComponent<CustomMaterialController>();
        List<Renderer> renderers = new List<Renderer>();
        renderers.Add(smr);
        AddWeaponPackRenderer(weaponPack_Normal);
        AddWeaponPackRenderer(weaponPack_StrongL);
        AddWeaponPackRenderer(weaponPack_StrongR);
        AddPropRenderer(shield);
        customMaterialController.Setting(renderers);
        
        void AddWeaponPackRenderer(Data_WeaponPack weaponPack)
        {
            if (weapondata[weaponPack].weaponL != null) AddPropRenderer(weapondata[weaponPack].weaponL);
            if (weapondata[weaponPack].weaponR != null) AddPropRenderer(weapondata[weaponPack].weaponR);
        }
        void AddPropRenderer(Prefab_Prop prop)
        {
            MeshRenderer renderer = prop.GetComponentInChildren<MeshRenderer>();
            renderers.Add(renderer);
        }
    }
    //Public
    private TrailEffect trailEffect;
    //[FoldoutGroup("Particle")] public Material mat_superarmor;
    [FoldoutGroup("Particle")] 
    public ParticleSystem p_charge_begin, p_charge_fin, p_charge_Impact;

    [FoldoutGroup("Particle")][SerializeField]
    private ParticleSystem p_charge_main,p_charge_strongL,p_charge_strongR;

    [FoldoutGroup("Particle")] 
    public ParticleSystem p_spawn,p_despawn,p_roll,p_change,p_footstep_l,p_footstep_r;

    [FoldoutGroup("Color")][ColorUsage(true,true)] 
    public Color c_hit_begin,c_hit_fin,c_evade_begin,c_evade_fin;

    //Private
    private Tween t_blink,t_punch;
    private Sequence s_trail;
    private Transform _meshRoot;
    private CustomMaterialController customMaterialController;
    private bool _superarmor = false;
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
        if (_currentTrailData == null || !_currentTrailData.useCustomParticle) return;
        if(_currentTrailData.customParticle_Sound != null) 
            SoundManager.Play(_currentTrailData.customParticle_Sound);
        Transform t = transform;
        if(_currentTrailData.customParticle_Particle != null) 
            ParticleManager.Play(_currentTrailData.customParticle_Particle,
                t.position + _currentTrailData.customParticle_Particle.addPos ,t.rotation);
        switch (_currentTrailData.customParticle_ShakeRatio)
        {
            default:
                break;
            case 1:
                CamArm.instance.Tween_ShakeSmooth();
                break;
            case 2:
                CamArm.instance.Tween_ShakeNormal_Core();
                break;
            case 3:
                CamArm.instance.Tween_ShakeStrong_Core();
                break;
        }
    }
    public void Effect_Footstep_L()
    {
        p_footstep_l.Play();
        Sound_Footstep(p_footstep_l.transform.position);
    }
    public void Effect_Footstep_R()
    {
        p_footstep_r.Play();
        Sound_Footstep(p_footstep_r.transform.position);
    }
    public void Effect_Roll()
    {
        Transform t = transform;
        p_roll.transform.SetPositionAndRotation(t.position + t.forward * 0.5f,t.rotation);
        p_roll.Play();
    }
    public void Effect_Smoke(float fwd = 0)
    {
        Transform t = transform;
        ParticleManager.Play(ParticleManager.instance.pd_smoke,t.position + t.forward*fwd + Vector3.up*0.1f,t.rotation,1);
    }
    public void Effect_Hit_Normal(Vector3 hitVec)
    {
        Tween_Blink_Hit(1.75f);
        var t = transform;
        Vector3 thisPos = t.position;
        Vector3 pos = thisPos + hitVec * 0.375f + Vector3.up*1.25f;
        Quaternion rot = Quaternion.LookRotation(-hitVec);
        ParticleManager.Play(ParticleManager.instance.pd_blood_hero,pos,rot, 1.2f);
        if(!_superarmor && Get_HeroMoveState() != MoveState.Roll) GameManager.Instance.DamagedText_Norm(thisPos);
    }
    public void Effect_Hit_Strong(Vector3 hitVec)
    {
        Tween_Blink_Hit(1.0f);
        var t = transform;
        Vector3 thisPos = t.position;
        Vector3 pos = thisPos + hitVec * 0.375f + Vector3.up*1.25f;
        Quaternion rot = Quaternion.LookRotation(-hitVec);
        ParticleManager.Play(ParticleManager.instance.pd_blood_hero,pos,rot, 1.2f);
        ParticleManager.Play(ParticleManager.instance.pd_impactwave,pos,rot, 1.5f);
        if(!_superarmor && Get_HeroMoveState() != MoveState.Roll) GameManager.Instance.DamagedText_Norm(thisPos);
    }
    //애니메이션 이벤트
    public void FallDown()
    {
        _falledTime = Time.time;
        Transform t = transform;
        ParticleManager.Play(ParticleManager.instance.pd_smoke,
            t.position + t.forward*-0.2f + Vector3.up*0.1f,t.rotation,1);
        SoundManager.Play(SoundContainer_Ingame.instance.sound_falldown);
        SoundManager.Play(SoundContainer_Ingame.instance.sound_friction_cloth);
        SoundManager.Play(sound_chain,0.25f);
    }
    public void Effect_Land()
    {
        Tween_Punch_Up(1.25f);
        Effect_Smoke();
        Sound_Footstep_Turn();
        
        SoundManager.Play(SoundContainer_Ingame.instance.sound_falldown);
        SoundManager.Play(SoundContainer_Ingame.instance.sound_friction_cloth);
        SoundManager.Play(sound_chain,0.25f);
    }
    //Particle
    public void Particle_Charge_Main()
    {
        p_charge_main.Play();
        Sound_Voice_Short();
    }
    public void Particle_Charge_L()
    {
        p_charge_strongL.Play();
        Sound_Voice_Short();
    }
    public void Particle_Charge_R()
    {
        p_charge_strongR.Play();
        Sound_Voice_Short();
    }
    //CustomEffect
    public void Activate_SuperArmor()
    {
        _superarmor = true;
        Transform t = transform;
        p_change.transform.SetPositionAndRotation(t.position + Vector3.up,t.rotation);
        p_change.Play();
        customMaterialController.Activate(GameManager.s_burn);
        SoundManager.Play(sound_combat_superarmor);
    }
    public void Activate_Feather()
    {
        customMaterialController.Activate(GameManager.s_feather);
    }
    public void Deactivate_CustomMaterial()
    {
        _superarmor = false;
        customMaterialController.Deactivate();
        SoundManager.Stop(sound_combat_superarmor);
    }
    public bool Get_SuperArmor()
    {
        return _superarmor;
    }

    
}

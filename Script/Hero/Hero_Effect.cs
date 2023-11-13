using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using TrailsFX;
using UnityEngine;
using UnityEngine.Serialization;

public partial class Hero : MonoBehaviour
{
    public float timescale=1.0f;
    public bool testbool = true;
    public bool isUp = false;
    private void Setting_Effect()
    {
        _meshRoot = transform.GetChild(0);
        trailEffect = GetComponentInChildren<SkinnedMeshRenderer>().GetComponent<TrailEffect>();
        trailEffect.active = false;
        var trailModule = p_charge.trails;
        trailModule.colorOverTrail = weaponPack_Normal.mainGradient;
    }
    
    //Public
    [HideInInspector]
    public TrailEffect trailEffect;
    [FoldoutGroup("Particle")] 
    public ParticleSystem p_charge_begin, p_charge_fin, p_charge_Impact,p_charge,p_spawn,p_despawn
        ,p_smoke,p_roll,p_change,p_footstep_l,p_footstep_r,p_blood_normal,p_blood_strong;
    [FoldoutGroup("Color")][ColorUsage(true,true)] 
    public Color c_hit_begin,c_hit_fin,c_evade_begin,c_evade_fin;
    
    //Private
    private Tween s_blink,s_punch;
    private Transform _meshRoot;
    //Tween
    public void Tween_Blink_Hit(float timeScale)
    {
        s_blink.Complete();
        s_blink = Tween.Custom(c_hit_begin, c_hit_fin, duration: 0.5f,
                onValueChange: newVal => _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, newVal)
                ,ease: Ease.InCirc);
        s_blink.timeScale = timescale;
    }
    public void Tween_Blink_Evade(float timeScale)
    {
        s_blink.Complete();
        s_blink = Tween.Custom(c_evade_begin, c_evade_fin, duration: 0.45f,
            onValueChange: newVal => _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, newVal)
            ,ease: Ease.InQuad);
        s_blink.timeScale = timescale;
    }
    public void Tween_Punch_Down(float speed)
    {
        s_punch.Complete();
        _meshRoot.localScale = GameManager.V3_One;
        s_punch = Tween.PunchScale(_meshRoot, strength: new Vector3(0.15f, 0.15f, -0.15f),
            duration: 0.75f, frequency: 7, easeBetweenShakes: Ease.OutQuad,useUnscaledTime:true);
        s_punch.timeScale = speed;
    }
    public void Tween_Punch_Up(float speed)
    {
        s_punch.Complete();
        _meshRoot.localScale = GameManager.V3_One;
        s_punch = Tween.PunchScale(_meshRoot, strength: new Vector3(-0.15f, -0.15f, 0.15f),
            duration: 0.75f, frequency: 7, easeBetweenShakes: Ease.OutQuad,useUnscaledTime:true);
        s_punch.timeScale = speed;
    }
    public void Tween_Punch_Down_Compact(float speed)
    {
        s_punch.Complete();
        _meshRoot.localScale = GameManager.V3_One;
        s_punch = Tween.PunchScale(_meshRoot, strength: new Vector3(0.125f,0.125f,-0.125f),
            duration: 0.45f, frequency: 7, easeBetweenShakes: Ease.OutSine,useUnscaledTime:true);
        s_punch.timeScale = speed;
    }
    public void Tween_Punch_Up_Compact(float speed)
    {
        s_punch.Complete();
        _meshRoot.localScale = GameManager.V3_One;
        s_punch = Tween.PunchScale(_meshRoot, strength: new Vector3(-0.125f,-0.125f,0.125f),
            duration: 0.45f, frequency: 7, easeBetweenShakes: Ease.OutSine,useUnscaledTime:true);
        s_punch.timeScale = speed;
    }

    public static void Blink(float duration)
    {
        instance.s_blink.Complete();
        instance.s_blink = Tween.Custom(instance.c_evade_begin, instance.c_evade_fin,duration,
            onValueChange: newVal => instance._outlinable.FrontParameters.FillPass
                .SetColor(GameManager.s_publiccolor, newVal),ease: Ease.InQuad);
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
        Tween_Blink_Hit(1.75f);
        
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
        Tween_Blink_Hit(1.0f);
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
    public void Effect_Land()
    {
        Tween_Punch_Up(1.25f);
        Effect_Smoke();
    }
    
    #if UNITY_EDITOR
    public void TestPunch()
    {
        if (isUp)
        {
            if(testbool) Tween_Punch_Up(timescale);
            else Tween_Punch_Up_Compact(timescale);
        }
        else
        {
            if(testbool) Tween_Punch_Down(timescale);
            else Tween_Punch_Down_Compact(timescale);
        }
    }
    #endif
}

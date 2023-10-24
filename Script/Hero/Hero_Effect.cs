using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        s_blink_hit = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, c_hit_begin); })
            .Append(_outlinable.FrontParameters.FillPass
                .DOColor(GameManager.s_publiccolor, c_hit_fin, 0.5f).SetEase(Ease.InCirc));
        s_blink_evade = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, c_evade_begin); })
            .Append(_outlinable.FrontParameters.FillPass
                .DOColor(GameManager.s_publiccolor, c_evade_fin, 0.45f).SetEase(Ease.InQuad));
        s_punch_up = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _meshRoot.localScale = GameManager.V3_One; })
            .Append(_meshRoot.DOPunchScale(new Vector3(-0.15f,-0.15f,0.15f), 0.75f,7)
                .SetEase(Ease.InOutBack));
        
        s_punch_down = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _meshRoot.localScale = GameManager.V3_One; })
            .Append(_meshRoot.DOPunchScale(new Vector3(0.15f,0.15f,-0.15f), 0.75f,7)
                .SetEase(Ease.InOutBack));
        s_punch_up_compact = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _meshRoot.localScale = GameManager.V3_One; })
            .Append(_meshRoot.DOPunchScale(new Vector3(-0.125f,-0.125f,0.125f), 0.45f,7)
                .SetEase(Ease.InOutBack));
        
        s_punch_down_compact = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _meshRoot.localScale = GameManager.V3_One; })
            .Append(_meshRoot.DOPunchScale(new Vector3(0.125f,0.125f,-0.125f), 0.45f,7)
                .SetEase(Ease.InOutBack));
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
    private Sequence s_blink_hit,s_blink_evade
        ,s_punch_up,s_punch_down,s_punch_up_compact,s_punch_down_compact;
    private Transform _meshRoot;
    //Tween
    public void Tween_Blink_Hit(float timeScale)
    {
        s_blink_hit.timeScale = timeScale;
        if(!s_blink_hit.IsInitialized()) s_blink_hit.Play();
        else s_blink_hit.Restart();
    }
    public void Tween_Blink_Evade(float timeScale)
    {
        s_blink_evade.timeScale = timeScale;
        if(!s_blink_evade.IsInitialized()) s_blink_evade.Play();
        else s_blink_evade.Restart();
    }
    public void Tween_Punch_Down(float speed)
    {
        if (s_punch_up.IsPlaying()) s_punch_up.Pause();
        if (s_punch_up_compact.IsPlaying()) s_punch_up_compact.Pause();
        if (s_punch_down_compact.IsPlaying()) s_punch_down_compact.Pause();
        
        s_punch_down.timeScale = speed;
        if(!s_punch_down.IsInitialized()) s_punch_down.Play();
        else s_punch_down.Restart();
    }
    public void Tween_Punch_Up(float speed)
    {
        if (s_punch_down.IsPlaying()) s_punch_down.Pause();
        if (s_punch_up_compact.IsPlaying()) s_punch_up_compact.Pause();
        if (s_punch_down_compact.IsPlaying()) s_punch_down_compact.Pause();
        
        s_punch_up.timeScale = speed;
        if(!s_punch_up.IsInitialized()) s_punch_up.Play();
        else s_punch_up.Restart();
    }
    public void Tween_Punch_Down_Compact(float speed)
    {
        if (s_punch_up.IsPlaying()) s_punch_up.Pause();
        if (s_punch_down.IsPlaying()) s_punch_down.Pause();
        if (s_punch_up_compact.IsPlaying()) s_punch_up_compact.Pause();
        
        
        s_punch_down_compact.timeScale = speed;
        if(!s_punch_down_compact.IsInitialized()) s_punch_down_compact.Play();
        else s_punch_down_compact.Restart();
    }
    public void Tween_Punch_Up_Compact(float speed)
    {
        if (s_punch_up.IsPlaying()) s_punch_up.Pause();
        if (s_punch_down.IsPlaying()) s_punch_down.Pause();
        if (s_punch_down_compact.IsPlaying()) s_punch_down_compact.Pause();
        
        s_punch_up_compact.timeScale = speed;
        if(!s_punch_up_compact.IsInitialized()) s_punch_up_compact.Play();
        else s_punch_up_compact.Restart();
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

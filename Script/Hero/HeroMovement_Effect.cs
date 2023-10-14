using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TrailsFX;
using UnityEngine;

public partial class HeroMovement : MonoBehaviour
{
    private int footstepIndex = 0;
    [HideInInspector]public TrailEffect trailEffect;
    private ParticleSystem p_smoke,p_roll,p_change, p_footstep_1,p_footstep_2,p_blood_normal,p_blood_strong;
    private Sequence s_blink_hit_normal,s_blink_hit_strong,s_blink_evade;
    [FoldoutGroup("Particle")] public ParticleSystem p_charge_begin, p_charge_fin, p_charge_Impact,p_charge;
    [FoldoutGroup("Color")][ColorUsage(true,true)] 
    public Color c_hit_begin,c_hit_fin,c_evade_begin,c_evade_fin;
    
    private void Setting_Effect()
    {
        trailEffect = transform.Find("SkinnedMesh").GetComponent<TrailEffect>();
        trailEffect.active = false;
        Transform particleT = transform.parent.Find("Particle");
        p_footstep_1 = particleT.Find("Footstep_1").GetComponent<ParticleSystem>();
        p_footstep_2 = particleT.Find("Footstep_2").GetComponent<ParticleSystem>();
        p_smoke = particleT.Find("Smoke").GetComponent<ParticleSystem>();
        p_roll = particleT.Find("Roll").GetComponent<ParticleSystem>();
        p_change = particleT.Find("Change").GetComponent<ParticleSystem>();
        p_blood_normal = particleT.Find("Blood_Normal").GetComponent<ParticleSystem>();
        p_blood_strong = particleT.Find("Blood_Strong").GetComponent<ParticleSystem>();

        s_blink_hit_normal = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
            .OnStart(() => { outlinable.OutlineParameters.FillPass.SetColor(GameManager.s_publiccolor, c_hit_begin); })
            .Append(outlinable.OutlineParameters.FillPass
                .DOColor(GameManager.s_publiccolor, c_hit_fin, 0.3f).SetEase(Ease.InQuad));
        s_blink_hit_strong = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
            .OnStart(() => { outlinable.OutlineParameters.FillPass.SetColor(GameManager.s_publiccolor, c_hit_begin); })
            .Append(outlinable.OutlineParameters.FillPass
                .DOColor(GameManager.s_publiccolor, c_hit_fin, 0.5f).SetEase(Ease.InCirc));
        s_blink_evade = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
            .OnStart(() => { outlinable.OutlineParameters.FillPass.SetColor(GameManager.s_publiccolor, c_evade_begin); })
            .Append(outlinable.OutlineParameters.FillPass
                .DOColor(GameManager.s_publiccolor, c_evade_fin, 0.45f).SetEase(Ease.InQuad));

        var trailModule = p_charge.trails;
        trailModule.colorOverTrail = weaponPack_Normal.mainGradient;
    }

    public void Effect_AttackParticle(int index)
    {
        if (animator.IsInTransition(0)) return;
        var mainParticle = weapondata[_currentWeaponPack].attackParticles[index];
        if (mainParticle == null) return;

        Transform t = transform;
        mainParticle.transform.SetPositionAndRotation(t.position, t.rotation);
        mainParticle.Play();
    }
    public void Effect_Footstep_L()
    {
        footstepIndex = (footstepIndex + 1) % 2;
        Transform t = transform;
        Vector3 pos = animator.GetBoneTransform(HumanBodyBones.LeftFoot).position +t.forward*-0.3f;
        Quaternion rot = t.rotation;
        if (footstepIndex == 0)
        {
            p_footstep_1.transform.SetPositionAndRotation(pos,rot);
            p_footstep_1.Play();
        }
        else
        {
            p_footstep_2.transform.SetPositionAndRotation(pos,rot);
            p_footstep_2.Play();
        }
    }
    public void Effect_Footstep_R()
    {
        footstepIndex = (footstepIndex + 1) % 2;
        Transform t = transform;
        Vector3 pos = animator.GetBoneTransform(HumanBodyBones.RightFoot).position +t.forward*-0.3f;
        Quaternion rot = t.rotation;
        if (footstepIndex == 0)
        {
            p_footstep_1.transform.SetPositionAndRotation(pos,rot);
            p_footstep_1.Play();
        }
        else
        {
            p_footstep_2.transform.SetPositionAndRotation(pos,rot);
            p_footstep_2.Play();
        }
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
        p_blood_normal.transform.position = currentPos + Vector3.up*0.75f;
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
        p_blood_normal.transform.position = currentPos + Vector3.up*0.75f;
        p_blood_strong.transform.position = currentPos + Vector3.up*0.75f;
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

using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class HeroMovement : MonoBehaviour
{
    private int footstepIndex = 0;
    private ParticleSystem p_roll, p_footstep_1,p_footstep_2;
    [FoldoutGroup("Particle")] public ParticleSystem p_charge_begin, p_charge_fin, p_charge_Impact;

    private void Setting_Effect()
    {
        Transform particleT = transform.parent.Find("Particle");
        p_footstep_1 = particleT.Find("Footstep_1").GetComponent<ParticleSystem>();
        p_footstep_2 = particleT.Find("Footstep_2").GetComponent<ParticleSystem>();
        p_roll = particleT.Find("Roll").GetComponent<ParticleSystem>();
    }
    //Smokes
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
    //
}

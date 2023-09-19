using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class HeroMovement : MonoBehaviour
{
    public float testf1,testf2;
    //Public
    public enum MoveState { Locomotion = 0,Roll = 1}
    public MoveState moveState = MoveState.Locomotion;
    //Private
    private Animator animator;
    private AIPath aiPath;
    private Hero hero;
    //Effect
    private int footstepIndex = 0;
    private ParticleSystem p_roll, p_footstep_1,p_footstep_2;
    
    //Animator State Machine에 사용되는 변수
    [HideInInspector] public float animatorParameters_footstep;
    [HideInInspector] public float rotateCurrentVelocity,rotAnimCurrentVelocity;
    [Range(0, 1)] public float ratio_speed; //애니메이터 파라미터 Easing에 사용됨
    private void Start()
    {
        Setting();
    }
    void Setting()
    {
        animator = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
        hero = GetComponentInParent<Hero>();

        Transform particleT = transform.parent.Find("Particle");
        p_footstep_1 = particleT.Find("Footstep_1").GetComponent<ParticleSystem>();
        p_footstep_2 = particleT.Find("Footstep_2").GetComponent<ParticleSystem>();
        p_roll = particleT.Find("Roll").GetComponent<ParticleSystem>();
        
        GameManager.instance.E_BTN_Action_Begin.AddListener(E_BTN_Action_Begin);
        GameManager.instance.E_BTN_Action_Fin.AddListener(E_BTN_Action_Fin);
    }

    //Public
    public void Move(Vector3 nextPos,Quaternion nextRot)
    {
        aiPath.FinalizeMovement(nextPos,nextRot);
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
    #region Action
    private float action_BeginTime = -100;
    public void E_BTN_Action_Begin()
    {
        action_BeginTime = Time.unscaledTime;
    }
    public void E_BTN_Action_Fin()
    {
        bool canRoll = Time.unscaledTime - action_BeginTime < hero.dash_roll_delay
                       && moveState == MoveState.Locomotion
                       && !animator.GetBool(GameManager.s_roll);
        if (canRoll)
        {
            animator.SetBool(GameManager.s_roll,true);
        }
        action_BeginTime = -100;
    }
    #endregion
    
}

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
	//변수 ==============================================================================================================
    [FoldoutGroup("Pointer")] public Targeter particle_target;
    [FoldoutGroup("Pointer")] public Material pointerMat;
    [FoldoutGroup("Pointer")] public AnimationCurve pointerCurve = AnimationCurve.EaseInOut(0,0,0.5f, 0.5f);
    [FoldoutGroup("Pointer")] public float pointerDistance = 2.25f;
    [FoldoutGroup("Pointer")] public float pointerSpeed = 20;
    [FoldoutGroup("Pointer")] public Sprite sprite_guard, sprite_bow;
    [FoldoutGroup("Pointer")] public Color pointer_LengthColor,pointer_NormalColor;

    [HideInInspector] public Transform pointer_Transform;
    [HideInInspector] public Image image_pointer;
    [HideInInspector] public Image image_length;
    [HideInInspector] public Enemy target=null;
    private bool pointer_Activated = false;
    private float pointer_ChangedTime = 0;
    private float pointerActivatingSpeed = 1.0f;
    private void Set_Pointer()
    {
        pointer_Transform = transform.Find("Pointer");
        image_pointer = pointer_Transform.Find("Image_Pointer").GetComponent<Image>();
        image_length = pointer_Transform.Find("Image_Length").GetComponent<Image>();
        particle_target = GetComponentInChildren<Targeter>(true);
        particle_target.Setting();
        particle_target.SetParent(null,0);
        pointer_Transform.SetParent(Manager_Main.instance._folder_);
        
        Canvas_Player.instance.OnABPressed.AddListener(Pointer_Pressed);
        Canvas_Player.instance.OnABReleased.AddListener(Pointer_Released);
        Canvas_Player.instance.OnRBPressed.AddListener(Pointer_Pressed);
        Canvas_Player.instance.OnRBReleased.AddListener(Pointer_Released);
    }
    //이벤트-------------------------------------------------------------------------------------------------------------
    public void Pointer_Pressed()
    {
        if(death) return;
        Activate_Pointer();
        if (Canvas_Player.AB_Pressed)
        {
            pointerActivatingSpeed = 1.0f;
            particle_target.Enforce(true);
        }
        else pointerActivatingSpeed = 0.4f;
        Canvas_Player.instance.OnLateUpdate.AddListener(Pointer_PressedUpdate);
        
    }
    public void Pointer_Released()
    {
        if(death) return;
        Deactivate_Pointer();
        particle_target.Enforce(false);
        Canvas_Player.instance.OnLateUpdate.RemoveListener(Pointer_PressedUpdate);
        Canvas_Player.instance.OnLateUpdate.RemoveListener(Pointer_PressedUpdate);
    }
    private void Pointer_PressedUpdate()
    {
        if(death) return;
        pointer_Transform.position = transform.position;
        if (Canvas_Player.AB_Pressed)
        {
            if (Canvas_Player.AS_Scale > 0.1f || target == null)
            {
                float deg_cam = DEG(CamArm.Degree());
                float deg_js = Canvas_Player.AS_Scale>0.1f?DEG(Canvas_Player.Degree_Action())
                    :(Canvas_Player.LS_Scale>0.1f?DEG(Canvas_Player.Degree_Left())
                        :transform.rotation.eulerAngles.y-deg_cam);
                UpdatePointer(deg_cam,deg_js,Canvas_Player.AS_Scale);
            }
            else
            {
                Vector3 dist = target.transform.position - transform.position;
                dist.y = 0;
                float deg = Mathf.Atan2(dist.z, dist.x)*Mathf.Rad2Deg;
                UpdatePointer(0,deg,Canvas_Player.AS_Scale);
            }
        }
        else if (Canvas_Player.RB_Pressed)
        {
            float deg_cam = DEG(CamArm.Degree());
            float deg_js;
            if (Canvas_Player.RS_Scale > 0.1f)
            {
                deg_js = DEG(Canvas_Player.Degree_Right());
                UpdatePointer(deg_cam,deg_js,Canvas_Player.RS_Scale);
            }
            else if (target!=null && particle_target.IsEnforced())
            {
                //Enforced되어있는 기존 타겟을 유지
            }
            else if (Canvas_Player.LS_Scale>0.1f)
            {
                deg_js = DEG(Canvas_Player.Degree_Left());
                UpdatePointer(deg_cam,deg_js,Canvas_Player.RS_Scale);
            }
            else
            {
                deg_js = transform.rotation.eulerAngles.y-deg_cam;
                UpdatePointer(deg_cam,deg_js,Canvas_Player.RS_Scale);
            }
            
        } 
        particle_target.Enforce(Canvas_Player.AB_Pressed || Canvas_Player.RS_Scale>0.1f);
    }
    //Pointer-----------------------------------------------------------------------------------------------------------
    private void UpdatePointer(float deg_cam,float deg_js,float scale)
    {
        if(death) return;
		//Target 설정
        Enemy _target=null;
        float dist = Mathf.Infinity;
        Vector3 pointerPos;
        pointerPos = image_pointer.transform.position;
        pointerPos.y = 0;
        if (scale > 0.1f)
        {
            foreach (var e in Enemy.enemies)
            {
                Vector3 ePos = e.transform.position;
                ePos.y = 0;
                float _dist= Vector3.Distance(ePos, pointerPos);
                if (_dist < dist)
                {
                    dist = _dist;
                    _target = e;
                }
            }
        }
        else if (target != null && Canvas_Player.AB_Pressed && !Canvas_Player.RB_Pressed)
        {
            _target = target;
        }
        else
        {
            foreach (var e in Enemy.enemies)
            {
                Vector3 ePos = e.transform.position;
                ePos.y = 0;
                float _dist= Vector3.Distance(ePos, transform.position);
                if (_dist < dist)
                {
                    dist = _dist;
                    _target = e;
                }
            }
        }
        //
        if (scale>0.1f)
        {
            //Image_Length 회전
            image_length.transform.rotation = Quaternion.Lerp(image_length.transform.rotation,
                Quaternion.Euler(-90,0,deg_js + deg_cam+180),pointerSpeed*Time.deltaTime);
        }
        else
        {
            if (target != null)
            {
                Vector3 lookVec = target.transform.position - transform.position;
                lookVec.y = 0;
                float deg = Quaternion.LookRotation(lookVec).eulerAngles.y+180;
                //Image_Length 회전
                image_length.transform.rotation = Quaternion.Lerp(image_length.transform.rotation,
                    Quaternion.Euler(-90,0,deg),pointerSpeed*Time.deltaTime);
            }
            else if (Canvas_Player.LS_Scale > 0.1f)
            {
                deg_js = Canvas_Player.Degree_Left();
                //Image_Length 회전
                image_length.transform.rotation = Quaternion.Lerp(image_length.transform.rotation,
                    Quaternion.Euler(-90, 0, deg_js + deg_cam + 180), pointerSpeed * Time.deltaTime);
            }
        }
        
        SetTarget(_target);
        //타겟 particle
        if (target != null)
        {
            if(particle_target.transform.parent != target.transform)
            {
                particle_target.SetParent(target,target.targetedHeight);
                particle_target.transform.localPosition = Vector3.up*target.targetedHeight;
                particle_target.Activate(true);
            }
        }
        else
        {
            particle_target.SetParent(null,0);
            particle_target.Activate(false);
        }
        
        //Pointer 이동,회전
        image_pointer.transform.position = image_length.transform.position+ new Vector3(0,0.01f,0)+
                                                  image_length.transform.rotation* Vector3.up 
                                                  * (Canvas_Player.AS_Scale>0.1f?Canvas_Player.AS_Scale:0.9f) 
                                                  * pointerDistance;
        image_pointer.transform.localRotation = Quaternion.Euler(0, 0, Time.time*90 % 360);
    }
    public void SetClosestTarget()
    {
        if(death) return;
        float dist = Mathf.Infinity;
        Enemy _target = null;
        Vector3 pointerPos = transform.position;
        pointerPos.y = 0;
        foreach (var e in Enemy.enemies)
        {
            Vector3 ePos = e.transform.position;
            ePos.y = 0;
            float _dist= Vector3.Distance(ePos, pointerPos);
            if (_dist < dist)
            {
                dist = _dist;
                _target = e;
            }
        }
        target = _target;
        if (target != null && particle_target.transform.parent != _target.transform)
        {
            particle_target.Enforce_StopEmmediately();
            particle_target.SetParent(_target,_target.targetedHeight);
            particle_target.transform.localPosition = Vector3.up*_target.targetedHeight;
            particle_target.Activate(true);
        }
        else if(target == null)
        {
            particle_target.SetParent(null,0);
            particle_target.Activate(false);
        }
    }
    private void SetTarget(Enemy _target)
    {
        if(death) return;
        target = _target;
    }

    private void Activate_Pointer()
    {
        if(death) return;
        if (!pointer_Activated)
        {
            if (!guard)
            {
                image_length.color = pointer_LengthColor;
                image_pointer.color = pointer_LengthColor;
            }
            pointer_Activated = true;
            pointer_ChangedTime = Time.unscaledTime;
            StopCoroutine("CPointerChanger");
            StartCoroutine("CPointerChanger");
            
            return;
        }
        pointer_Activated = true;
    }
    private void Deactivate_Pointer()
    {
        if (pointer_Activated)
        {
            pointer_Activated = false;
            pointer_ChangedTime = Time.unscaledTime;
            StopCoroutine("CPointerChanger");
            StartCoroutine("CPointerChanger");
            return;
        }
        pointer_Activated = false;
    }
    public void PointerMode_Guard(bool activateGuard)
    {
        if(death) return;
        guard = activateGuard;
        image_length.sprite = sprite_guard;
        
        if (guard)
        {
            image_length.color = pointer_LengthColor;
            image_pointer.color = pointer_NormalColor;
        }
    }
    public void PointerMode_Bow()
    {
        if(death) return;
        image_length.sprite = sprite_bow;
        image_length.color = pointer_LengthColor;
        image_pointer.color =pointer_NormalColor;
    }
    private IEnumerator CPointerChanger()
    {
        
        if (pointer_Activated)
        {
            float changeSpeed = 1.25f*pointerActivatingSpeed;
            while (pointerMat.GetFloat("_FadeAmount")>0.01f)
            {
                float val = Mathf.Clamp01(0.5f - (Time.unscaledTime - pointer_ChangedTime) * changeSpeed);
                pointerMat.SetFloat("_FadeAmount", pointerCurve.Evaluate(val));
                yield return null;
            }
            pointerMat.SetFloat("_FadeAmount", 0);
        }
        else
        {
            float changeSpeed = 0.75f;
            while (pointerMat.GetFloat("_FadeAmount")<0.49f)
            {
                float val = Mathf.Clamp01((Time.unscaledTime - pointer_ChangedTime) * changeSpeed);
                pointerMat.SetFloat("_FadeAmount", pointerCurve.Evaluate(val));
                yield return null;
            }
            pointerMat.SetFloat("_FadeAmount", 0.5f);
            image_length.color = pointer_LengthColor;
            image_pointer.color = pointer_LengthColor;
        }
    }
    
    //Leaning-----------------------------------------------------------------------------------------------------------
    private bool leaning_Activated = false;
    private float leaning_ActivatedTime = 0;
    private float leaning_GlobalRatio = 0.0f;
    public void SetLeaning(bool activate)
    {
        leaning_Activated = activate;
        leaning_ActivatedTime = Time.unscaledTime;
        StopCoroutine("CLeaningChanger");
        StartCoroutine("CLeaningChanger");
    }
    public void UpdateLeaning(bool isAccelerating, float speed)
    {
        leaningAnimator.User_DeliverAccelerationSpeed(speed*0.5f);
        leaningAnimator.User_DeliverIsAccelerating(isAccelerating);
    }
    private IEnumerator CLeaningChanger()
    {
        
        if (!leaning_Activated)
        {
            float changeSpeed = 5.0f;
            while (leaning_GlobalRatio>0.01f)
            {
                float val = Mathf.Clamp01(1 - (Time.unscaledTime - leaning_ActivatedTime) * changeSpeed);
                leaning_GlobalRatio= val;
                UpdateData();
                yield return null;
            }
            leaning_GlobalRatio = 0;
            leaningAnimator.enabled = false;
        }
        else
        {
            leaningAnimator.enabled = true;
            float changeSpeed = 1.25f;
            while (leaning_GlobalRatio<0.99f)
            {
                float val = Mathf.Clamp01((Time.unscaledTime - leaning_ActivatedTime) * changeSpeed);
                leaning_GlobalRatio=val;
                UpdateData();
                yield return null;
            }

            leaning_GlobalRatio = 1;
        }

        void UpdateData()
        {
            leaningAnimator.Parameters.SideSwayPower = 1.3f * leaning_GlobalRatio;
            leaningAnimator.Parameters.ClampSideSway = 25.0f * leaning_GlobalRatio;
            leaningAnimator.Parameters.ForwardSwayPower = 0.4f * leaning_GlobalRatio;
            leaningAnimator.Parameters.BrakeRapidity = 0.485f * leaning_GlobalRatio;
        }
    }

    
}

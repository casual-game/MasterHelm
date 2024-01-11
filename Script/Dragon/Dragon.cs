using System;
using System.Collections;
using System.Collections.Generic;
using AmazingAssets.AdvancedDissolve;
using EPOOutline;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class Dragon : MonoBehaviour
{
    public static Dragon instance;
    private Animator anim;
    public SkinnedMeshRenderer body;
    public Transform sitPoint;
    public AnimationCurve flightCurve,flightRotCurve,flightAddvecCurve;

    [HideInInspector] public float degDiff;
    [HideInInspector] public Transform destination = null;
    
    private Material _mat;
    private Tween _tDissolve;
    private Sequence _seq;
    private OutlineTarget _outlineTarget;
    private bool isFirst = true;
    public void Setting()
    {
        if (!isFirst) return;
        isFirst = false;
        anim = GetComponent<Animator>();
        _mat = body.material;
        _outlineTarget = GetComponent<Outlinable>().OutlineTargets[0];
        instance = this;
    }
    public void Activate()
    {
        gameObject.SetActive(true);
        _tDissolve.Stop();
        AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(
            _mat, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, 1);
        _outlineTarget.CutoutThreshold = 1;
        _tDissolve = Tween.Custom(1, 0, 0.75f, onValueChange: dissolve =>
        {
            AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(
                _mat, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, dissolve);
            _outlineTarget.CutoutThreshold = dissolve;
        });

    }
    public void Deactivate(float delay)
    {
        _tDissolve.Stop();
        AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(
            _mat, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, 0);
        _outlineTarget.CutoutThreshold = 0;
        _tDissolve = Tween.Custom(0, 1, 0.75f, onValueChange: dissolve =>
        {
            AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(
                _mat, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, dissolve);
            _outlineTarget.CutoutThreshold = dissolve;
        },startDelay: delay).OnComplete(()=> gameObject.SetActive(false));
    }
    
    [Button]
    public void Call(Vector3 pos)
    {
        _seq.Complete();
        destination = null;
        transform.rotation = Quaternion.Euler(0,-90,0);
        Vector3 beginPos = pos + new Vector3(7.5f, 5, 0);
        Vector3 landPos = pos;
        
        Activate();
        anim.SetTrigger(GameManager.s_call);
        
        _seq = Sequence.Create();
        _seq.Chain(Tween.Position(transform, beginPos, landPos, 1.0f, Ease.InOutSine))
            .Group(Tween.Delay(0.65f, () => anim.SetTrigger(GameManager.s_transition)))
            .ChainDelay(2.0f)
            .ChainCallback(() =>
            {
                anim.SetTrigger(GameManager.s_flight);
                Deactivate(4.0f);
                anim.SetFloat(GameManager.s_flight_y, 1);
                anim.SetFloat(GameManager.s_flight_x, 0);
            })
            .Chain(Tween.Custom(0.85f, 0.5f, 1.5f, onValueChange: 
                val => anim.SetFloat(GameManager.s_flight_y, val),ease:Ease.Linear))
            .Group(Tween.Custom(0.0f, -0.375f, 1.5f, onValueChange: 
            val => anim.SetFloat(GameManager.s_flight_x, val),ease:Ease.InSine));
    }
    [Button]
    public void MoveDestination(Room_Area currentRoom,Room_Area targetRoom)
    {
        destination = targetRoom.startPoint;
        _seq.Complete();
        Transform t = transform;
        Vector3 pos = Hero.instance.transform.position +
                      (currentRoom.startPoint.rotation * Quaternion.Euler(0, 45, 0)) * Vector3.back * 1.5f;
        Vector3 beginPos = pos + new Vector3(7.5f, 5, 0);
        t.SetPositionAndRotation(beginPos,Quaternion.Euler(0,-90,0));
        
        Activate();
        anim.SetTrigger(GameManager.s_call);
        //플레이어 위치로 이동
        _seq = Sequence.Create();
        _seq.Chain(Tween.Position(t, beginPos, pos, 1.0f, Ease.InOutSine))
            .Group(Tween.Delay(0.65f, () => anim.SetTrigger(GameManager.s_transition)))
            .ChainDelay(1.0f);
        //회전
        Vector3 endPos = targetRoom.startPoint.position + targetRoom.startPoint.rotation * Vector3.left* 1.5f;
        Vector3 distvec = endPos - pos;
        distvec.y = 0;
        degDiff = Mathf.Atan2(distvec.x, distvec.z) * Mathf.Rad2Deg - t.rotation.eulerAngles.y;
        while (degDiff < -180) degDiff += 360;
        while (degDiff > 180) degDiff -= 360;
        if (Mathf.Abs(degDiff)>90)
        {
            if(degDiff < -90) _seq.ChainCallback(() => anim.SetTrigger(GameManager.s_turn_l));
            else if(degDiff > 90) _seq.ChainCallback(() => anim.SetTrigger(GameManager.s_turn_r));
            anim.SetBool(GameManager.s_turn, false);
            _seq.ChainDelay(0.5f);
        }
        else if (Mathf.Abs(degDiff)>20f)
        {
            if(degDiff<-20) _seq.ChainCallback(() => anim.SetTrigger(GameManager.s_turn_l));
            if(degDiff>20) _seq.ChainCallback(() => anim.SetTrigger(GameManager.s_turn_r));
            anim.SetBool(GameManager.s_turn, true);
            _seq.Chain(Tween.Rotation(t, t.rotation * Quaternion.Euler(0, degDiff, 0), Mathf.Abs(degDiff)*0.0125f, Ease.InOutSine));
        }
        else
        {
            _seq.ChainCallback(() => anim.SetTrigger(GameManager.s_flight));
            _seq.Chain(Tween.Rotation(t, t.rotation * Quaternion.Euler(0, degDiff, 0), Mathf.Abs(degDiff)*0.01f, Ease.OutSine));
        }
        //목적지까지 비행 (Flight)
        _seq.ChainCallback(() =>
        {
            anim.SetFloat(GameManager.s_flight_x, 0);
            anim.SetFloat(GameManager.s_flight_y, 0);
            _seq.ChainCallback(() => anim.SetTrigger(GameManager.s_flight));
        });
        float speed = 6.5f;
        float distance = distvec.magnitude;
        float duration = distance / speed;
        Vector3 lookVec = distvec;
        lookVec.y = 0;
        _seq.ChainDelay(0.5f);
        _seq.ChainCallback(() => CamArm.instance.Set_FollowTarget(false));
        _seq.ChainCallback(() =>
            {
                CamArm.instance.Tween_Zoom(duration*0.15f,duration*0.3f,duration*0.4f,0,3.5f);
                CamArm.instance.Tween_Angle(duration*0.15f,duration*0.3f,duration*0.4f,-12.5f);
            })
            .Chain(Tween.Custom(0, 1, duration, onValueChange: ratio =>
            {
                //이동
                float height = Mathf.Clamp01(-4 * ratio * ratio + 4 * ratio);
                anim.SetFloat(GameManager.s_flight_y, 0.65f - ratio);
                Vector3 movePos = Vector3.Lerp(pos, endPos, flightCurve.Evaluate(ratio));
                movePos.y = height * distance * 0.2f;
                Vector3 addVec = Vector3.down*2.5f*flightAddvecCurve.Evaluate(ratio);
                movePos += addVec;
                //회전
                var moveRot = Quaternion.Lerp(t.rotation, Quaternion.LookRotation(lookVec), 3 * Time.deltaTime);
                //적용
                t.SetPositionAndRotation(movePos, moveRot);
            }))
            .Group(Tween.Delay(duration -0.5f, () =>
            {
                destination = null;
                FlyAway(2.0f);
                CamArm.instance.Set_FollowTarget(true);
                anim.SetTrigger(GameManager.s_transition);
            }))
            .Group(Tween.Custom(0, 1, duration, onValueChange: ratio =>
            {
                if (!CamArm.instance.Get_FollowTarget())
                {
                    Transform camT = CamArm.instance.transform;
                    Quaternion camRot = Quaternion.Lerp(currentRoom.startPoint.rotation,
                        targetRoom.startPoint.rotation, flightRotCurve.Evaluate(ratio));
                    Vector3 camPos = Vector3.Lerp(camT.position,
                        t.position + t.forward*2, 5 * Time.deltaTime);
                    CamArm.instance.transform.SetPositionAndRotation(camPos,camRot);
                }
            }));

    }

    public void FlyAway(float delay)
    {
        _seq.Stop();
        _seq = Sequence.Create()
            .ChainDelay(delay)
            .ChainCallback(() =>
            {
                anim.SetTrigger(GameManager.s_flight);
                Deactivate(4.0f);
                anim.SetFloat(GameManager.s_flight_y, 1);
                anim.SetFloat(GameManager.s_flight_x, 0);
            })
            .Chain(Tween.Custom(0.85f, 0.25f, 1.5f, onValueChange:
                val => anim.SetFloat(GameManager.s_flight_y, val), ease: Ease.Linear))
            .Group(Tween.Custom(0.0f, -0.375f, 1.5f, onValueChange:
                val => anim.SetFloat(GameManager.s_flight_x, val), ease: Ease.InSine));
    }

    [Button]
    public void MoveFin()
    {
        anim.SetTrigger(GameManager.s_transition);
    }

    public void Desmount()
    {
        if (destination == null)
        {
            CamArm.instance.UI_Round(false, 1);
            Hero.instance.Desmount();
        }
    }

    public void Mount()
    {
        if (destination != null) Hero.instance.Mount();
    }

    public void Effect_Land()
    {
        Transform t = transform;
        ParticleManager.Play(ParticleManager.instance.pd_smoke,t.position + Vector3.up*0.1f,t.rotation,1.0f);
    }
}

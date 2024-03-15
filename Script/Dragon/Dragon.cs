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
    public UI_IngameResult uiIngameResult;
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
    //반복 함수
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
    //핵심 함수
    public void Call()
    {
        //초기화
        _seq.Complete();
        destination = null;
        var mountData = Get_MountData(1.5f, GameManager.Instance.Get_Room());
        Vector3 endPos = mountData.pos;
        Vector3 startPos = endPos + mountData.rot*new Vector3(0, 5, -7.5f);
        transform.SetPositionAndRotation(startPos,mountData.rot);
        //생성
        Activate();
        anim.SetTrigger(GameManager.s_call);
        //시퀸스
        _seq = Sequence.Create();
        _seq.Chain(Tween.Position(transform, startPos, endPos, 1.0f, Ease.InOutSine))
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
    public void MoveDestination(Room_Area currentRoom,Room_Area targetRoom)
    {
        bool moved = false;
        destination = targetRoom.startPoint;
        _seq.Complete();
        Transform t = transform;
        //마운트 위치로 이동
        var mountData = Get_MountData(1.5f, currentRoom, Hero.instance.transform);
        Vector3 mountPos = mountData.pos;
        Vector3 beginPos = mountPos + mountData.rot*new Vector3(0, 5, -7.5f);
        t.SetPositionAndRotation(beginPos,mountData.rot);
        Activate();
        anim.SetTrigger(GameManager.s_call);
        _seq = Sequence.Create();
        _seq.Chain(Tween.Position(t, beginPos, mountPos, 1.0f, Ease.InOutSine))
            .Group(Tween.Delay(0.65f, () => anim.SetTrigger(GameManager.s_transition)))
            .ChainDelay(1.0f);
        //회전(마운트는 애니매이션 이벤트로 실행.)
        var dismountData = Get_DismountData(1.5f, targetRoom);
        Vector3 distvec = dismountData.pos - mountPos;
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
        float speed = 7.5f;
        float distance = distvec.magnitude;
        float duration = distance / speed;
        Vector3 lookVec = distvec;
        lookVec.y = 0;
        _seq.ChainDelay(0.5f);
        _seq.ChainCallback(() => CamArm.instance.Set_FollowTarget(false));
        _seq.ChainCallback(() =>
            {
                CamArm.instance.Tween_Zoom(duration*0.15f,duration*0.3f,duration*0.4f,0,3.5f,false);
                CamArm.instance.Tween_Angle(duration*0.15f,duration*0.3f,duration*0.4f,-12.5f);
            })
            .Chain(Tween.Custom(0, 1, duration, onValueChange: ratio =>
            {
                if (!moved)
                {
                    //이동
                    anim.SetFloat(GameManager.s_flight_y, 0.65f - ratio);
                    Vector3 movePos = Vector3.Lerp(mountPos, dismountData.pos, flightCurve.Evaluate(ratio));
                    Vector3 addVec = Vector3.up*3.5f*flightAddvecCurve.Evaluate(ratio);
                    movePos += addVec;
                    //회전
                    var moveRot = Quaternion.Lerp(t.rotation, Quaternion.LookRotation(lookVec), 3 * Time.deltaTime);
                    //적용
                    t.SetPositionAndRotation(movePos, moveRot);
                    //애니메이션
                    bool lateEnough = ratio>0.875f && transform.position.y-dismountData.pos.y<0.5f;
                    if (lateEnough)
                    {
                        moved = true;
                        Arrival();
                    }
                }
            }).OnComplete(() =>
            {
                if (!moved)
                {
                    moved = true;
                    Arrival();
                } 
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

        void Arrival()
        {
            anim.SetTrigger(GameManager.s_transition);
            destination = null;
            CamArm.instance.Set_FollowTarget(true);
            FlyAway(2.0f);
        }
    }
    public void FinalFlight(Room_Area currentRoom,CamArm targetCam)
    {
        bool moved = false;
        destination = targetCam.transform;
        _seq.Complete();
        Transform t = transform;
        //마운트 위치로 이동
        var mountData = Get_MountData(1.5f, currentRoom, Hero.instance.transform);
        Vector3 mountPos = mountData.pos;
        Vector3 beginPos = mountPos + mountData.rot*new Vector3(0, 5, -7.5f);
        t.SetPositionAndRotation(beginPos,mountData.rot);
        Activate();
        anim.SetTrigger(GameManager.s_call);
        _seq = Sequence.Create();
        _seq.Chain(Tween.Position(t, beginPos, mountPos, 1.0f, Ease.InOutSine))
            .Group(Tween.Delay(0.65f, () => anim.SetTrigger(GameManager.s_transition)))
            .ChainDelay(1.0f);
        //회전(마운트는 애니매이션 이벤트로 실행.)
        var dismountData = Get_DismountData(1.5f, targetCam);
        Vector3 distvec = dismountData.pos - mountPos;
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
            _seq.Chain(Tween.Rotation(t, t.rotation * Quaternion.Euler(0, degDiff, 0), 
                Mathf.Abs(degDiff)*0.0125f, Ease.InOutSine));
            _seq.ChainDelay(0.5f);
        }
        else
        {
            _seq.Chain(Tween.Rotation(t, t.rotation * Quaternion.Euler(0, degDiff, 0), 
                Mathf.Abs(degDiff)*0.01f, Ease.OutSine));
        }
        //목적지까지 비행 (Flight)
        Vector3 flightBegin = mountPos;
        Vector3 flightFin = mountPos;
        _seq.ChainCallback(() =>
        {
            anim.SetFloat(GameManager.s_flight_x, -1.0f);
            anim.SetFloat(GameManager.s_flight_y, 0.5f);
            anim.SetTrigger(GameManager.s_flight);
            flightBegin = transform.position;
            flightFin = flightBegin + Vector3.up * 3;
            Hero.instance.Get_Animator().SetBool(GameManager.s_finish,true);
            foreach (var p in Hero.instance.finishBeginParticles) p.Play();
        });
        _seq.Group(Tween.Delay(0.1f, ()=>
        {
            Hero.instance.Effect_Smoke();
            foreach (var p in Hero.instance.finishFinParticles) p.Play();
        }));
        _seq.ChainDelay(0.375f);
        _seq.Chain(Tween.Custom(0, 1, 0.25f, onValueChange: ratio =>
        {
            transform.position = Vector3.Lerp(flightBegin, flightFin, ratio);
        }));
        
        _seq.Group(Tween.Delay(0.5f, ()=>
        {
            uiIngameResult.Success_Begin();
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
    public void MoveFin()
    {
        anim.SetTrigger(GameManager.s_transition);
    }
    //애니메이션 이벤트
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

    public (Vector3 pos, Quaternion rot) Get_MountData(float dist,Room_Area area)
    {
        Transform point = area.startPoint;
        Quaternion _r = point.rotation * Quaternion.Euler(0,135,0);
        Vector3 _p = point.position + point.rotation * Quaternion.Euler(0, 45, 0)*Vector3.back*dist;

        return (_p, _r);
    }
    public (Vector3 pos, Quaternion rot) Get_DismountData(float dist,Room_Area area)
    {
        Transform point = area.startPoint;
        Quaternion _r = point.rotation * Quaternion.Euler(0,135,0);
        Vector3 _p = point.position + point.rotation * Quaternion.Euler(0, 0, 0)*Vector3.back*dist;

        return (_p, _r);
    }
    public (Vector3 pos, Quaternion rot) Get_DismountData(float dist,CamArm cam)
    {
        Transform point = cam.mainCam.transform;
        Quaternion _r = Quaternion.Inverse(Quaternion.LookRotation(point.forward));
        Vector3 _p = point.position;

        return (_p, _r);
    }
    public (Vector3 pos, Quaternion rot) Get_MountData(float dist,Room_Area area,Transform target)
    {
        Quaternion roomRot = area.startPoint.rotation;
        Quaternion _r = roomRot * Quaternion.Euler(0,135,0);
        Vector3 _p = target.position + roomRot * Quaternion.Euler(0, 45, 0)*Vector3.back*dist;

        return (_p, _r);
    }
}

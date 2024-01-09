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
    private Animator anim;
    public SkinnedMeshRenderer body;
    public GameObject eyes;

    [HideInInspector] public float degDiff;
    [HideInInspector] public Transform destination = null;
    
    private Material _mat;
    private Tween _tDissolve;
    private Sequence _seq;
    private OutlineTarget _outlineTarget;
    private bool isFirst = true;
    public void OnEnable()
    {
        if (!isFirst) return;
        isFirst = false;
        anim = GetComponent<Animator>();
        _mat = body.material;
        _outlineTarget = GetComponent<Outlinable>().OutlineTargets[0];
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
        _seq.Stop();
        destination = null;
        transform.rotation = Quaternion.Euler(0,-90,0);
        Vector3 beginPos = pos + new Vector3(7.5f, 5, 0);
        Vector3 landPos = pos;
        Vector3 finPos = pos + new Vector3(-7.5f, 7.5f, 0);
        
        Activate();
        anim.SetTrigger(GameManager.s_call);
        
        _seq = Sequence.Create();
        _seq.Chain(Tween.Position(transform, beginPos, landPos, 1.0f, Ease.InOutSine))
            .Group(Tween.Delay(0.65f, () => anim.SetTrigger(GameManager.s_transition)))
            .ChainDelay(2.0f)
            .ChainCallback(() =>
            {
                anim.SetTrigger(GameManager.s_flight);
                Deactivate(2.0f);
                anim.SetFloat(GameManager.s_flight_y, 1);
                anim.SetFloat(GameManager.s_flight_x, 0);
            })
            .Chain(Tween.Custom(0.85f, 0.5f, 1.5f, onValueChange: 
                val => anim.SetFloat(GameManager.s_flight_y, val),ease:Ease.Linear))
            .Group(Tween.Custom(0.0f, -0.375f, 1.5f, onValueChange: 
            val => anim.SetFloat(GameManager.s_flight_x, val),ease:Ease.InSine));
        //.Chain(Tween.Position(transform, landPos, finPos, 1.5f, Ease.OutSine, startDelay: 0.375f));
    }
    [Button]
    public void MoveDestination(Transform d)
    {
        destination = GameManager.Instance.Room2.startPoint;
        _seq.Stop();
        Transform t = transform;
        Vector3 pos = Hero.instance.transform.position;
        Vector3 beginPos = pos + new Vector3(7.5f, 5, 0);
        Vector3 landPos = pos;
        t.SetPositionAndRotation(beginPos,Quaternion.Euler(0,-90,0));
        
        Activate();
        anim.SetTrigger(GameManager.s_call);
        //플레이어 위치로 이동
        _seq = Sequence.Create();
        _seq.Chain(Tween.Position(t, beginPos, landPos, 1.0f, Ease.InOutSine))
            .Group(Tween.Delay(0.65f, () => anim.SetTrigger(GameManager.s_transition)))
            .ChainDelay(2.0f);
        //회전
        Vector3 distvec = destination.position - landPos;
        distvec.y = 0;
        degDiff = Mathf.Atan2(distvec.x, distvec.z) * Mathf.Rad2Deg - t.rotation.eulerAngles.y;
        while (degDiff < -180) degDiff += 360;
        while (degDiff > 180) degDiff -= 360;
        print(Mathf.Abs(degDiff/180.0f) +", "+degDiff);
        if (Mathf.Abs(degDiff)>90)
        {
            if(degDiff < -90) _seq.ChainCallback(() => anim.SetTrigger(GameManager.s_turn_l));
            else if(degDiff > 90) _seq.ChainCallback(() => anim.SetTrigger(GameManager.s_turn_r));
            anim.SetBool(GameManager.s_turn, false);
            _seq.ChainDelay(0.5f);
            _seq.ChainCallback(() => anim.SetTrigger(GameManager.s_flight));
        }
        else if (Mathf.Abs(degDiff)>20f)
        {
            if(degDiff<-20) _seq.ChainCallback(() => anim.SetTrigger(GameManager.s_turn_l));
            if(degDiff>20) _seq.ChainCallback(() => anim.SetTrigger(GameManager.s_turn_r));
            anim.SetBool(GameManager.s_turn, true);
            _seq.Chain(Tween.Rotation(t, t.rotation * Quaternion.Euler(0, degDiff, 0), Mathf.Abs(degDiff)*0.0125f, Ease.InOutSine));
            _seq.ChainCallback(() => anim.SetTrigger(GameManager.s_flight));
        }
        else
        {
            _seq.ChainCallback(() => anim.SetTrigger(GameManager.s_flight));
            _seq.Chain(Tween.Rotation(t, t.rotation * Quaternion.Euler(0, degDiff, 0), Mathf.Abs(degDiff)*0.01f, Ease.OutSine));
            _seq.ChainCallback(() => anim.SetTrigger(GameManager.s_flight));
        }
        //목적지까지 이동
        _seq.ChainCallback(() =>
        {
            anim.SetFloat(GameManager.s_flight_x, 0);
            anim.SetFloat(GameManager.s_flight_y, 0);
        });
    }
    [Button]
    public void MoveFin()
    {
        anim.SetTrigger(GameManager.s_transition);
    }
}

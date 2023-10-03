using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AmazingAssets.AdvancedDissolve;
using Cysharp.Threading.Tasks;
using EPOOutline;
using Sirenix.OdinInspector;
using UnityEngine;

public class Prefab_Prop : MonoBehaviour
{
    private float _currentActivateRatio, _targetActivateRatio;
    private bool _targetActivation;
    private Material _material;
    private float _activateSpeed = 1.5f, _deactivateSpeed = 1.0f;
    private ParticleSystem _createParticle;
    private float _activateRatio = 0;
    private OutlineTarget _outlineTarget;
    private Transform equipT;

#if UNITY_EDITOR
    [BoxGroup("에디터 전용 기능")] public Outlinable debugOutlinable;

    [BoxGroup("에디터 전용 기능")]
    [HorizontalGroup("에디터 전용 기능/H")]
    [Button]
    public void DebugActivate()
    {
        UT_ActivateProp(debugOutlinable).Forget();
    }

    [BoxGroup("에디터 전용 기능")]
    [HorizontalGroup("에디터 전용 기능/H")]
    [Button]
    public void DebugDeactivate()
    {
        UT_DeactivateProp(debugOutlinable).Forget();
    }

    public void Awake()
    {
        Setting();
    }
    #endif
    public void Equip(Transform parent)
    {
        Transform t = transform;
        t.SetParent(parent);
        t.SetLocalPositionAndRotation(equipT.localPosition,equipT.localRotation);
        t.localScale = equipT.localScale;
        UT_ActivateProp(debugOutlinable).Forget();
    }
    public void Unarm()
    {
        transform.SetParent(null);
        UT_DeactivateProp(debugOutlinable).Forget();
    }
    public void Setting_Hero()
    {
        Setting();
        equipT = transform.Find("Equip_Hero");
    }
    private void Setting()
    {
        _material = GetComponent<MeshRenderer>().material;
        _createParticle = GetComponentInChildren<ParticleSystem>();
        _outlineTarget = new OutlineTarget(GetComponent<Renderer>());
        _outlineTarget.CutoutTextureName = "_AdvancedDissolveCutoutStandardMap1";
    }

    private async UniTaskVoid UT_ActivateProp(Outlinable outlinable)
    {
        gameObject.SetActive(true);
        _targetActivation = true;
        if (!outlinable.OutlineTargets.Contains(_outlineTarget))
        {
            outlinable.TryAddTarget(_outlineTarget);
            
        }
        
        _createParticle.Play();
        
        while (_activateRatio<1 && _targetActivation)
        {
            _activateRatio += Time.deltaTime*_activateSpeed;
            float ratio = 1-GameManager.instance.curve_inout.Evaluate(Mathf.Clamp01(_activateRatio));
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,ratio);
            _outlineTarget.CutoutThreshold = ratio;
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }

        if (_targetActivation)
        {
            _activateRatio = 1;
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,0);
            _outlineTarget.CutoutThreshold = 0;
        }
    }
    private async UniTaskVoid UT_DeactivateProp(Outlinable outlinable)
    {
        _targetActivation = false;
        while (_activateRatio>0 &&!_targetActivation)
        {
            _activateRatio -= Time.deltaTime*_deactivateSpeed;
            float ratio = 1-GameManager.instance.curve_inout.Evaluate(Mathf.Clamp01(_activateRatio));
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,ratio);
            _outlineTarget.CutoutThreshold = ratio;
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }
        
        if (!_targetActivation)
        {
            await UniTask.WaitUntil(() => !_createParticle.isPlaying);
            _activateRatio = 0;
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1);
            _outlineTarget.CutoutThreshold = 1;
            gameObject.SetActive(false);
            if(outlinable.OutlineTargets.Contains(_outlineTarget)) outlinable.RemoveTarget(_outlineTarget);
        }
    }
}

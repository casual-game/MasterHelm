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
    private float _activateRatio = 0;
    private OutlineTarget _outlineTarget;
    private Outlinable outlinable;
    private Transform attachT,detachT;
    private Transform attachCopyT, detachCopyT;//장착,장착해제 시 사용할 loaclData 저장되어있는 T. 각자의 Setting에서 설정해주어야 한다.
    private bool canKeep = false;
    //외부 사용 가능 함수
    public void Attach()
    {
        if(!canKeep) UT_ActivateProp_CantKeep().Forget();
        else UT_ActivateProp_CanKeep().Forget();
    }
    public void Detach()
    {
        if(!canKeep) UT_DeactivateProp_CantKeep().Forget();
        else UT_DeactivateProp_CanKeep().Forget();
    }
    public void Setting_Hero(Outlinable outlinable,bool canKeep,Transform attachT,Transform detachT)
    {
        Setting();
        attachCopyT = transform.Find("AttachCopyT_Hero");
        detachCopyT = transform.Find("DetachCopyT_Hero");
        this.attachT = attachT;
        this.detachT = detachT;
        this.outlinable = outlinable;
        this.canKeep = canKeep;

        if (canKeep)
        {
            outlinable.TryAddTarget(_outlineTarget);
            UT_DeactivateProp_CanKeep().Forget();
        }
        else
        {
            gameObject.SetActive(false);
            transform.SetParent(detachT);
        }
    }
    
    //내부 전용 함수
    private void Setting()
    {
        _material = GetComponent<MeshRenderer>().material;
        _outlineTarget = new OutlineTarget(GetComponent<Renderer>());
        _outlineTarget.CutoutTextureName = "_AdvancedDissolveCutoutStandardMap1";
    }


    //UniTask - Keep불가능
    private async UniTaskVoid UT_ActivateProp_CantKeep()
    {
        gameObject.SetActive(true);
        _targetActivation = true;
        CopyTransform(attachT,attachCopyT);
        if (!outlinable.OutlineTargets.Contains(_outlineTarget))
        {
            outlinable.TryAddTarget(_outlineTarget);
        }
        
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
    private async UniTaskVoid UT_DeactivateProp_CantKeep()
    {
        _targetActivation = false;
        transform.SetParent(detachT);
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
            _activateRatio = 0;
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1);
            _outlineTarget.CutoutThreshold = 1;

            gameObject.SetActive(false);
            if(outlinable.OutlineTargets.Contains(_outlineTarget)) outlinable.RemoveTarget(_outlineTarget);
        }
    }
    //UniTask - Keep가능
    private async UniTaskVoid UT_ActivateProp_CanKeep()
    {
        _targetActivation = true;
        //위치 변경
        if (_targetActivation)
        {
            _activateRatio = 0;
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1);
            _outlineTarget.CutoutThreshold = 1;
            CopyTransform(attachT,attachCopyT);
        }
        //다시 생성
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
    private async UniTaskVoid UT_DeactivateProp_CanKeep()
    {
        _targetActivation = false;
        //일단 사라짐
        while (_activateRatio>0 &&!_targetActivation)
        {
            _activateRatio -= Time.deltaTime*_deactivateSpeed;
            float ratio = 1-GameManager.instance.curve_inout.Evaluate(Mathf.Clamp01(_activateRatio));
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,ratio);
            _outlineTarget.CutoutThreshold = ratio;
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }
        //위치 변경
        if (!_targetActivation)
        {
            _activateRatio = 0;
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1);
            _outlineTarget.CutoutThreshold = 1;
            CopyTransform(detachT,detachCopyT);
        }
        //다시 생성
        while (_activateRatio<1 && !_targetActivation)
        {
            _activateRatio += Time.deltaTime*_activateSpeed;
            float ratio = 1-GameManager.instance.curve_inout.Evaluate(Mathf.Clamp01(_activateRatio));
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,ratio);
            _outlineTarget.CutoutThreshold = ratio;
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }
        if (!_targetActivation)
        {
            _activateRatio = 1;
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,0);
            _outlineTarget.CutoutThreshold = 0;
        }
    }

    private void CopyTransform(Transform targetT,Transform copyT)
    {
        Transform t = transform;
        t.SetParent(targetT);
        t.SetLocalPositionAndRotation(copyT.localPosition,copyT.localRotation);
        t.localScale = copyT.localScale; 
    }
    
}

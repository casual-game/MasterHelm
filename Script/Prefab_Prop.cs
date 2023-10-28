using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AmazingAssets.AdvancedDissolve;
using Cysharp.Threading.Tasks;
using EPOOutline;
using Sirenix.OdinInspector;
using TrailsFX;
using UnityEngine;

public class Prefab_Prop : MonoBehaviour
{
    private float _currentActivateRatio, _targetActivateRatio;
    private bool _targetActivation;
    private Material _material;
    private float _activateSpeed = 2.0f, _deactivateSpeed = 1.25f;
    private float _activateRatio = 0;
    private OutlineTarget _outlineTarget;
    private Outlinable _outlinable;
    private Transform _attachT,_detachT,_nullFolder;
    private Transform _attachCopyT, _detachCopyT;//장착,장착해제 시 사용할 loaclData 저장되어있는 T. 각자의 Setting에서 설정해주어야 한다.
    private bool _canKeep = false;
    private TrailEffect[] _trails;
    private ParticleSystem _p_spawn;
    public List<Collider> _interact_currentTargets,_interact_savedTargets,_interact_interactedTargets;
    private bool _isHero;
    private int _syncInt = 0,_syncedInt=0;
    //외부 사용 가능 함수
    public void SetTrail(bool active)
    {
        foreach (var trail in _trails)
        {
            if(trail.active!=active) trail.active = active;
        }
    }
    public bool GetTrail()
    {
        if (!gameObject.activeSelf) return false;
        return _trails[0].active;
    }
    public void Attach()
    {
        if(!_canKeep) UT_ActivateProp_CantKeep().Forget();
        else UT_ActivateProp_CanKeep().Forget();
    }
    public void Detach()
    {
        if(!_canKeep) UT_DeactivateProp_CantKeep().Forget();
        else UT_DeactivateProp_CanKeep().Forget();
    }
    //CanKeep 일때만 사용..
    public void Spawn()
    {
        gameObject.SetActive(true);
        UT_DeactivateProp_CanKeep(false).Forget();
    }
    public void Despawn()
    {
        UT_DeactivateProp_CantKeep(false).Forget();
    }
    //Collision
    
    public void Collision_Interact(TrailData trailData)//공격 판정 계산할때 호출한다.
    {
        foreach (var coll in _interact_savedTargets) Interact(coll);
        foreach (var coll in _interact_currentTargets) Interact(coll);
        void Interact(Collider coll)
        {
            if (!_interact_interactedTargets.Contains(coll))
            {
                if (_isHero)
                {
                    coll.TryGetComponent<Monster>(out var monster);
                    monster.Core_Hit_Strong(transform,trailData);
                    _interact_interactedTargets.Add(coll);
                }
            }
        }
        _interact_savedTargets.Clear();
    }
    public void Collision_Reset()//공격 모션에서 물리계산 타이밍이 아닌 경우, 매 프레임 호출한다.
    {
        _interact_interactedTargets.Clear();
        for(int i = _interact_savedTargets.Count-1; i>=0; i--)
        {
            bool canRemove = !_interact_currentTargets.Contains(_interact_savedTargets[i]);
            if(canRemove)_interact_savedTargets.RemoveAt(i);
        }
    }
    //Setting
    public void Setting_Hero(Outlinable outlinable,bool canKeep,Transform attachT,Transform detachT,Transform nullFolder = null)
    {
        Setting();
        _attachCopyT = transform.Find("AttachCopyT_Hero");
        _detachCopyT = transform.Find("DetachCopyT_Hero");
        _attachT = attachT;
        _detachT = detachT;
        _nullFolder = nullFolder;
        _outlinable = outlinable;
        _canKeep = canKeep;
        _isHero = true;
        if (canKeep)
        {
            outlinable.TryAddTarget(_outlineTarget);
            gameObject.SetActive(false);
            transform.SetParent(detachT);
        }
        else
        {
            gameObject.SetActive(false);
            transform.SetParent(detachT);
        }
    }
    public void Setting_Monster(Outlinable outlinable,bool canKeep,Transform attachT,Transform detachT,Transform nullFolder = null)
    {
        Setting();
        _attachCopyT = transform.Find("AttachCopyT_Monster");
        _detachCopyT = transform.Find("DetachCopyT_Monster");
        _attachT = attachT;
        _detachT = detachT;
        _nullFolder = nullFolder;
        _outlinable = outlinable;
        _canKeep = canKeep;
        _isHero = false;
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
        _trails = GetComponents<TrailEffect>();
        _p_spawn = GetComponentInChildren<ParticleSystem>();
        _interact_savedTargets = new List<Collider>(14);
        _interact_interactedTargets = new List<Collider>(14);
        foreach (var trail in _trails)
        {
            trail.active = false;
            //trail.Clear();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_isHero)
        {
            if (!other.CompareTag(GameManager.s_monster)) return;
            if(!_interact_savedTargets.Contains(other))_interact_savedTargets.Add(other);
            if(!_interact_currentTargets.Contains(other))_interact_currentTargets.Add(other);
        }
        else
        {
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (_isHero)
        {
            if (!other.CompareTag(GameManager.s_monster)) return;
            if(_interact_currentTargets.Contains(other))_interact_currentTargets.Remove(other);
        }
        else
        {
            
        }
    }
    //UniTask - Keep불가능
    private async UniTaskVoid UT_ActivateProp_CantKeep()
    {
        gameObject.SetActive(true);
        if(_p_spawn.isPlaying) _p_spawn.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _targetActivation = true;
        CopyTransform(_attachT,_attachCopyT);
        if (!_outlinable.OutlineTargets.Contains(_outlineTarget))
        {
            _outlinable.TryAddTarget(_outlineTarget);
        }
        while (_activateRatio<1 && _targetActivation)
        {
            _activateRatio += Time.deltaTime*_activateSpeed;
            float ratio = 1-GameManager.Instance.curve_inout.Evaluate(Mathf.Clamp01(_activateRatio));
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
    private async UniTaskVoid UT_DeactivateProp_CantKeep(bool useParticle = true)
    {
        _targetActivation = false;
        transform.SetParent(_detachT);
        if(useParticle) _p_spawn.Play();
        while (_activateRatio>0 &&!_targetActivation)
        {
            _activateRatio -= Time.deltaTime*_deactivateSpeed;
            float ratio = 1-GameManager.Instance.curve_inout.Evaluate(Mathf.Clamp01(_activateRatio));
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
            if(!_canKeep && _outlinable.OutlineTargets.Contains(_outlineTarget)) _outlinable.RemoveTarget(_outlineTarget);
        }
    }
    //UniTask - Keep가능
    private async UniTaskVoid UT_ActivateProp_CanKeep()
    {
        if(_p_spawn.isPlaying) _p_spawn.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _targetActivation = true;
        //위치 변경
        if (_targetActivation)
        {
            _activateRatio = 0;
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1);
            _outlineTarget.CutoutThreshold = 1;
            CopyTransform(_attachT,_attachCopyT);
        }
        //다시 생성
        while (_activateRatio<1 && _targetActivation)
        {
            _activateRatio += Time.deltaTime*_activateSpeed;
            float ratio = 1-GameManager.Instance.curve_inout.Evaluate(Mathf.Clamp01(_activateRatio));
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
    private async UniTaskVoid UT_DeactivateProp_CanKeep(bool useParticle = true)
    {
        _targetActivation = false;
        transform.SetParent(_nullFolder);
        if(useParticle) _p_spawn.Play();
        //일단 사라짐
        while (_activateRatio>0 &&!_targetActivation)
        {
            _activateRatio -= Time.deltaTime*_deactivateSpeed;
            float ratio = 1-GameManager.Instance.curve_inout.Evaluate(Mathf.Clamp01(_activateRatio));
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
            CopyTransform(_detachT,_detachCopyT);
        }
        //다시 생성
        while (_activateRatio<1 && !_targetActivation)
        {
            _activateRatio += Time.deltaTime*_activateSpeed;
            float ratio = 1-GameManager.Instance.curve_inout.Evaluate(Mathf.Clamp01(_activateRatio));
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

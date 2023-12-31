using System;
using System.Collections;
using System.Collections.Generic;
using AmazingAssets.AdvancedDissolve;
using Cysharp.Threading.Tasks;
using EPOOutline;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class Hero : MonoBehaviour
{
    private void Setting_Spawn()
    {
        _dissolveRatio = 1;
        _material = GetComponentInChildren<SkinnedMeshRenderer>().material;
        _shadow = transform.Find(GameManager.s_shadow);
        _shadowScale = _shadow.localScale;
        _outlineTarget = _outlinable.OutlineTargets[0];
        
        AdvancedDissolveProperties.Cutout.Standard.
            UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1.0f);
        _outlineTarget.CutoutThreshold = 1;
        gameObject.SetActive(false);
    }
    //Private
    private bool _spawned = false;
    private float _dissolveRatio;
    private float _dissolveSpeed_Spawn = 1.5f,_dissolveSpeed_Despawn = 2.0f;
    private Transform _shadow;
    private Vector3 _shadowScale;
    private Material _material;
    private OutlineTarget _outlineTarget;
    
    public void Test_Spawn()
    {
      Spawn(transform.position,transform.rotation).Forget();   
    }
    [Button]
    public void Test_Despawn_Death()
    {
        Despawn().Forget();   
    }
    public void Test_Despawn_Move()
    {
        Despawn_Move().Forget();
    }

    public void Mount()
    {
        _animator.SetInteger(GameManager.s_state_type,1);
        _animator.SetBool(GameManager.s_force,true);
        _animator.SetTrigger(GameManager.s_forcetransition);
    }

    public void Desmount()
    {
        _animator.SetInteger(GameManager.s_state_type,2);
        _animator.SetBool(GameManager.s_force,true);
        _animator.SetTrigger(GameManager.s_forcetransition);
    }
    
    
    public async UniTaskVoid Spawn(Vector3 nextPos,Quaternion nextRot)
    {
        AdvancedDissolveProperties.Cutout.Standard.
            UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1);
        _outlineTarget.CutoutThreshold = 1;
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        Move_Warp(nextPos,nextRot);
        _spawned = true;
        _animator.Rebind();
        Tween_Blink_Evade(1.0f);
        Sound_Voice_Short();
        SoundManager.Play(sound_spawn);
        //무기 설정
        var weaponpack = weapondata[weaponPack_Normal];
        if(weaponpack.weaponL!=null) weaponpack.weaponL.Spawn();
        if(weaponpack.weaponR!=null) weaponpack.weaponR.Spawn();
        shield.Spawn();
        //파티클
        p_spawn.Play();
        Tween_Punch_Up_Compact(1.0f);
        //await UniTask.Delay(TimeSpan.FromSeconds(0.2f), DelayType.DeltaTime);
        CamArm.instance.Tween_Radial(0.5f,0.1f);
        GameManager.Instance.Shockwave(transform.position + Vector3.up);
        while (_spawned && _dissolveRatio>0)
        {
            _dissolveRatio -= Time.deltaTime*_dissolveSpeed_Spawn;
            float ratio = Mathf.Clamp01(_dissolveRatio);
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,ratio);
            _outlineTarget.CutoutThreshold = ratio;
            _shadow.localScale = _shadowScale*(1 - ratio);
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }

        if (_spawned)
        {
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,0);
            _outlineTarget.CutoutThreshold = 0;
            _shadow.localScale = _shadowScale; 
        }
    }
    public async UniTaskVoid Despawn()
    {
        _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        GameManager.Instance.Shockwave(transform.position + Vector3.up);
        Equipment_Equip(null,false,2,2);
        //기본 설정
        _spawned = false;
        _animator.SetInteger(GameManager.s_state_type,0);
        _animator.SetBool(GameManager.s_force,true);
        _animator.SetTrigger(GameManager.s_forcetransition);
        //Blood,Effect
        Tween_Punch_Down(0.75f);
        Tween_Blink_Hit(1.0f);
        await UniTask.Delay(TimeSpan.FromSeconds(3.0f), DelayType.UnscaledDeltaTime);
        //무기 설정
        var weaponpack = weapondata[weaponPack_Normal];
        if(weaponpack.weaponL!=null) weaponpack.weaponL.Despawn();
        if(weaponpack.weaponR!=null) weaponpack.weaponR.Despawn();
        shield.Despawn();
        //파티클
        p_despawn.Play();
        while (!_spawned && _dissolveRatio<1)
        {
            _dissolveRatio += Time.unscaledDeltaTime*_dissolveSpeed_Despawn;
            float ratio = Mathf.Clamp01(_dissolveRatio);
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,ratio);
            _outlineTarget.CutoutThreshold = ratio;
            _shadow.localScale = _shadowScale*(1 - ratio);
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }
        if (!_spawned)
        {
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1);
            _outlineTarget.CutoutThreshold = 1;
            _shadow.localScale = GameManager.V3_Zero;
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), DelayType.UnscaledDeltaTime);
        }
        if (!_spawned) gameObject.SetActive(false);
    }
    public async UniTaskVoid Despawn_Move()
    {
        _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        Equipment_Equip(null,false,2,2);
        //기본 설정
        _spawned = false;
        //무기 설정
        var weaponpack = weapondata[weaponPack_Normal];
        if(weaponpack.weaponL!=null) weaponpack.weaponL.Despawn();
        if(weaponpack.weaponR!=null) weaponpack.weaponR.Despawn();
        shield.Despawn();
        //파티클
        p_despawn.Play();
        while (!_spawned && _dissolveRatio<1)
        {
            _dissolveRatio += Time.unscaledDeltaTime*_dissolveSpeed_Despawn;
            float ratio = Mathf.Clamp01(_dissolveRatio);
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,ratio);
            _outlineTarget.CutoutThreshold = ratio;
            _shadow.localScale = _shadowScale*(1 - ratio);
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }
        if (!_spawned)
        {
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1);
            _outlineTarget.CutoutThreshold = 1;
            _shadow.localScale = GameManager.V3_Zero;
            await UniTask.Delay(TimeSpan.FromSeconds(2.0f), DelayType.UnscaledDeltaTime);
        }
        if (!_spawned) gameObject.SetActive(false);
    }
}

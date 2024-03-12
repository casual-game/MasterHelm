using System;
using System.Collections;
using System.Collections.Generic;
using AmazingAssets.AdvancedDissolve;
using Cysharp.Threading.Tasks;
using EPOOutline;
using PrimeTween;
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
    }
    //Private
    [HideInInspector] public bool _spawned = false;
    private float _dissolveRatio;
    private float _dissolveSpeed_Spawn = 1.5f,_dissolveSpeed_Despawn = 2.0f;
    private Transform _shadow;
    private Vector3 _shadowScale;
    public Material _material;
    private OutlineTarget _outlineTarget;
    private Sequence _seqMount;
    
    public void Spawn()
    {
        frameMain.Setting(heroData.HP, heroData.MP_Slot_Capacity);
        Spawn(transform.position, transform.rotation).Forget();
    }
    [Button]
    public void Death()
    {
        _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        
        Equipment_Equip(null,false,2,2);
        //기본 설정
        _spawned = false;
        _animator.SetInteger(GameManager.s_125ms,0);
        _animator.SetInteger(GameManager.s_state_type,0);
        _animator.SetBool(GameManager.s_force,true);
        _animator.SetTrigger(GameManager.s_forcetransition);
        //Blood,Effect
        Tween_Punch_Down(0.75f);
        Tween_Blink_Hit(1.0f);
        CamArm.instance.Tween_ShakeDeath_Hero();
        //무기 설정
        Core_Cancel();
    }
    public void Mount()
    {
        _spawned = false;
        transform.SetParent(null);
        Transform t = transform, dt = Dragon.instance.transform;
        Quaternion dtRot = dt.rotation;
        Vector3 startPos = t.position, endPos = dt.position + dtRot * Vector3.left;
        Quaternion startRot = t.rotation, endRot = dtRot * Quaternion.Euler(0, 90, 0);
        _seqMount.Stop();
        _seqMount = Sequence.Create();
        _seqMount.Chain(Tween.Custom(0, 1, 0.3f, onValueChange: ratio =>
        {
            Vector3 pos = Vector3.Lerp(startPos, endPos, ratio);
            Quaternion rot = Quaternion.Lerp(startRot, endRot, ratio);
            Move_Warp(pos, rot);
        }));
        _seqMount.ChainCallback(() =>
        {
            _animator.SetInteger(GameManager.s_125ms,3);
            _animator.SetInteger(GameManager.s_state_type, 1);
            _animator.SetBool(GameManager.s_force, true);
            _animator.SetTrigger(GameManager.s_forcetransition);
            transform.SetParent(Dragon.instance.sitPoint);
        });
    }
    public void MountInstantly()
    {
        _spawned = false;
        Transform dt = Dragon.instance.transform;
        Quaternion dtRot = dt.rotation;
        Vector3 endPos = dt.position + dtRot * Vector3.left;
        Quaternion endRot = dtRot * Quaternion.Euler(0, 90, 0);
        Move_Warp(endPos, endRot);
        _animator.SetInteger(GameManager.s_125ms,1);
        _animator.SetInteger(GameManager.s_state_type, 3);
        _animator.SetBool(GameManager.s_force, true);
        _animator.SetTrigger(GameManager.s_forcetransition);
        transform.SetParent(Dragon.instance.sitPoint);
    }
    public void Desmount()
    {
        Transform t = transform;
        t.SetParent(null);
        //Sequence
        Vector3 startPos = t.position;
        Quaternion startRot = t.rotation;
        Vector3 endPos = Dragon.instance.sitPoint.position + Dragon.instance.transform.rotation*Vector3.left*1.5f;
        Quaternion endRot = Dragon.instance.sitPoint.rotation * Quaternion.Euler(0, -89, 0);
        _seqMount.Stop();
        _seqMount = Sequence.Create();
        _seqMount.Chain(Tween.Custom(0, 1, 0.2f, onValueChange: ratio =>
        {
            Vector3 pos = Vector3.Lerp(startPos,endPos,ratio);
            Quaternion rot = Quaternion.Lerp(startRot,endRot,ratio);
            Move_Warp(pos,rot);
        },Ease.OutCirc));
        
        _animator.Rebind();
        _animator.SetBool(GameManager.s_force,false);
        Tween_Blink_Evade(1.0f);
        Tween_Punch_Up_Compact(1.0f);
        p_spawn.Play();
        Sound_Voice_Short();
        SoundManager.Play(sound_spawn);
       
        
        CamArm.instance.Tween_ShakeNormal_Core();
        GameManager.Instance.Shockwave(transform.position + Vector3.up);
        CamArm.instance.Set_FollowTarget(true);
    }
    
    
    public async UniTaskVoid Spawn(Vector3 nextPos,Quaternion nextRot)
    {
        _animator.updateMode = AnimatorUpdateMode.Normal;
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

    public void SpawnInstantly()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        _animator.Rebind();
        var weaponpack = weapondata[weaponPack_Normal];
        if(weaponpack.weaponL!=null) weaponpack.weaponL.Spawn();
        if(weaponpack.weaponR!=null) weaponpack.weaponR.Spawn();
        shield.Spawn();
        AdvancedDissolveProperties.Cutout.Standard.
            UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,0.0f);
        _outlineTarget.CutoutThreshold = 0;
        _shadow.localScale = _shadowScale; 
    }
        
    public async UniTaskVoid Despawn()
    {
        _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        GameManager.Instance.Shockwave(transform.position + Vector3.up);
        Equipment_Equip(null,false,2,2);
        //기본 설정
        _spawned = false;
        _animator.SetInteger(GameManager.s_125ms,0);
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
}

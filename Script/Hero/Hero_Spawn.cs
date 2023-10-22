using System.Collections;
using System.Collections.Generic;
using AmazingAssets.AdvancedDissolve;
using Cysharp.Threading.Tasks;
using EPOOutline;
using Sirenix.OdinInspector;
using UnityEngine;

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
    private bool _spawned = false;
    private float _dissolveRatio;
    private float _dissolveSpeed_Spawn = 1.5f,_dissolveSpeed_Despawn = 1.0f;
    private Transform _shadow;
    private Vector3 _shadowScale;
    private Material _material;
    private OutlineTarget _outlineTarget;
    [Button]
    public void Test1()
    {
      Spawn(Vector3.zero,transform.rotation).Forget();   
    }
    [Button]
    public void Test2()
    {
        Despawn().Forget();   
    }
    async UniTaskVoid Spawn(Vector3 relativePos,Quaternion rot)
    {
        gameObject.SetActive(true);
        Move_Nav(relativePos,rot);
        _spawned = true;
        _animator.Rebind();
        //무기 설정
        var weaponpack = weapondata[weaponPack_Normal];
        if(weaponpack.weaponL!=null) weaponpack.weaponL.Spawn();
        if(weaponpack.weaponR!=null) weaponpack.weaponR.Spawn();
        shield.Spawn();
        //파티클
        p_spawn.Play();
        
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
        AdvancedDissolveProperties.Cutout.Standard.
            UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,0);
        _outlineTarget.CutoutThreshold = 0;
        _shadow.localScale = _shadowScale;
    }
    
    async UniTaskVoid Despawn()
    {
        gameObject.SetActive(true);
        _spawned = false;
        _animator.Rebind();
        //무기 설정
        var weaponpack = weapondata[weaponPack_Normal];
        if(weaponpack.weaponL!=null) weaponpack.weaponL.Despawn();
        if(weaponpack.weaponR!=null) weaponpack.weaponR.Despawn();
        shield.Despawn();
        //파티클
        p_despawn.Play();
        
        while (!_spawned && _dissolveRatio<1)
        {
            _dissolveRatio += Time.deltaTime*_dissolveSpeed_Despawn;
            float ratio = Mathf.Clamp01(_dissolveRatio);
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,ratio);
            _outlineTarget.CutoutThreshold = ratio;
            _shadow.localScale = _shadowScale*(1 - ratio);
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }
        AdvancedDissolveProperties.Cutout.Standard.
            UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1);
        _outlineTarget.CutoutThreshold = 1;
        _shadow.localScale = Vector3.zero;
    }
}

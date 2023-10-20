using System;
using System.Collections;
using System.Collections.Generic;
using AmazingAssets.AdvancedDissolve;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EPOOutline;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public partial class Monster : MonoBehaviour
{
    public void Setting_Monster()
    {
        _agent = GetComponent<NavMeshAgent>();
        var smr = GetComponentInChildren<SkinnedMeshRenderer>();
        _material = smr.material;
        _meshRoot = smr.rootBone;
        _outlinable = GetComponent<Outlinable>();
        _animator = GetComponent<Animator>();
        _outlineTarget = _outlinable.OutlineTargets[0];
        _dissolveRatio = 1.0f;
        
        AdvancedDissolveProperties.Cutout.Standard.
            UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1);
        _outlineTarget.CutoutThreshold = 1;
        
        Setting_UI();
        Setting_Effect();
    }
    
    //Private
    private bool _isAlive = false;
    private float _dissolveRatio = 1.0f;
    private float _dissolveSpeed = 1.3f;
    private Material _material;
    private NavMeshAgent _agent;
    private Outlinable _outlinable;
    private OutlineTarget _outlineTarget;
    protected MonsterAnim_Base _animBase;
    protected Animator _animator;
    protected Transform _meshRoot;
    
    
    //Editor
    #if UNITY_EDITOR
    [FoldoutGroup("Debug")] public Prefab_Prop debugWepaonL, debugWaeponR, debugShield;
    public void Start()
    {
        Setting_Monster();
        Transform t = transform;
        Prefab_Prop l = debugWepaonL==null?null:Instantiate(debugWepaonL), 
                    r = debugWaeponR==null?null:Instantiate(debugWaeponR), 
                    s = debugShield==null?null:Instantiate(debugShield);
        if(l!=null) l.Setting_Monster(_outlinable,false,t_hand_l,null,GameManager.Folder_MonsterProp);
        if(r!=null) r.Setting_Monster(_outlinable,false,t_hand_r,null,GameManager.Folder_MonsterProp);
        if(s!=null) s.Setting_Monster(_outlinable,false,t_shield,null,GameManager.Folder_MonsterProp);
        Spawn(Vector3.zero, t.rotation,l,r,s).Forget();
    }

    [Button]
    public void DebugSpawn()
    {
        Spawn(Vector3.zero,transform.rotation,_weaponL,_weaponR,_shield).Forget();
    }

    [Button]
    public void DebugDespawn()
    {
        Despawn().Forget();
    }
    #endif
    
    //Unitask
    async UniTaskVoid Spawn(Vector3 relativePos,Quaternion rot,Prefab_Prop weaponL,Prefab_Prop weaponR,Prefab_Prop shield)
    {
        _animator.Rebind();
        _animator.SetBool(GameManager.s_spawn,Random.Range(0,2)==0);
        _isAlive = true;
        p_spawn.Play();
        Move_Nav(relativePos,rot);
        ActivateUI();
        _weaponL = weaponL;
        _weaponR = weaponR;
        _shield = shield;
        Equip();
        while (_isAlive && _dissolveRatio>0)
        {
            _dissolveRatio -= Time.deltaTime*_dissolveSpeed;
            float ratio = Mathf.Clamp01(_dissolveRatio);
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,ratio);
            _outlineTarget.CutoutThreshold = ratio;
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }
        AdvancedDissolveProperties.Cutout.Standard.
            UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,0);
        _outlineTarget.CutoutThreshold = 0;
    }
    async UniTaskVoid Despawn()
    {
        _isAlive = false;
        Unequip();
        DeactivateUI();
        while (!_isAlive && _dissolveRatio<1)
        {
            _dissolveRatio += Time.deltaTime*_dissolveSpeed;
            float ratio = Mathf.Clamp01(_dissolveRatio);
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,ratio);
            _outlineTarget.CutoutThreshold = ratio;
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }
        AdvancedDissolveProperties.Cutout.Standard.
            UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1);
        _outlineTarget.CutoutThreshold = 1;

        if (!ui_Sequence_Deactivated.IsComplete()) await ui_Sequence_Deactivated.AwaitForComplete();
    }
    
    //Setter
    public void Set_AnimBase(MonsterAnim_Base animBase)
    {
        _animBase = animBase;
    }
    
    //Move
    public void Move_Nav(Vector3 relativePos,Quaternion nextRot)
    {
        transform.rotation = nextRot;
        _agent.Move(relativePos);
    }
    public void Move_Normal(Vector3 nextPos,Quaternion nextRot)
    {
        transform.SetPositionAndRotation(nextPos, nextRot);
    }
}

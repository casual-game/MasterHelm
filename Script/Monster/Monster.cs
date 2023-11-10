using System;
using System.Collections;
using System.Collections.Generic;
using AmazingAssets.AdvancedDissolve;
using Cysharp.Threading.Tasks;
using EPOOutline;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public partial class Monster : MonoBehaviour
{
    public void Setting_Monster(Prefab_Prop _weaponl,Prefab_Prop _weaponr,Prefab_Prop _shield)
    {
        _shadow = transform.Find(GameManager.s_shadow);
        _shadowScale = _shadow.localScale;
        _agent = GetComponent<NavMeshAgent>();
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        _material = smr.material;
        _meshRoot = smr.rootBone;
        _outlinable = GetComponent<Outlinable>();
        _animator = GetComponent<Animator>();
        _outlineTarget = _outlinable.OutlineTargets[0];
        _dissolveRatio = 1.0f;
        
        AdvancedDissolveProperties.Cutout.Standard.
            UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,1);
        _outlineTarget.CutoutThreshold = 1;
        
        
        Prefab_Prop l = _weaponl==null?null:Instantiate(_weaponl), 
            r = _weaponr==null?null:Instantiate(_weaponr), 
            s = _shield==null?null:Instantiate(_shield);
        if(l!=null) l.Setting_Monster(_outlinable,false,t_hand_l,null,GameManager.Folder_MonsterProp);
        if(r!=null) r.Setting_Monster(_outlinable,false,t_hand_r,null,GameManager.Folder_MonsterProp);
        if(s!=null) s.Setting_Monster(_outlinable,false,t_shield,null,GameManager.Folder_MonsterProp);
        
        Transform t = transform;
        Spawn(GameManager.V3_Zero, t.rotation,l,r,s).Forget();
        
        Setting_UI();
        Setting_Effect();
        
        Monsters.Add(this);
    }
    //Static
    public static List<Monster> Monsters = new List<Monster>();
    [FoldoutGroup("MainData")] public int hp = 100;
    
    //Private,Protected
    private float _dissolveRatio = 1.0f;
    private Material _material;
    private NavMeshAgent _agent;
    private Outlinable _outlinable;
    private OutlineTarget _outlineTarget;
    private Transform _shadow;
    private Vector3 _shadowScale;
    protected MonsterAnim_Base _animBase;
    protected Animator _animator;
    protected Transform _meshRoot;
    
    protected bool _isAlive = false, _isReady = false;
    protected int currenthp;
    protected float deathDealy = 0.5f;
    protected float dissolveSpeed = 1.3f;
    protected SkinnedMeshRenderer smr;
    //Public
    public enum AnimationState
    {
        Locomotion = 0
    }
    public enum HitState {Ground=0,Air=1,Recovery=2}
    public HitState _hitState
    {
        get;
        private set;
    }
    //Editor
    #if UNITY_EDITOR
    [FoldoutGroup("Debug")] public Prefab_Prop debugWepaonL, debugWaeponR, debugShield;
    public void Start()
    {
        Setting_Monster(debugWepaonL,debugWaeponR,debugShield);
    }

    [Button]
    public void DebugSpawn()
    {
        Spawn(GameManager.V3_Zero,transform.rotation,_weaponL,_weaponR,_shield).Forget();
    }

    [Button]
    public void DebugDespawn()
    {
        Despawn().Forget();
    }
    #endif
    
    //Unitask
    protected async UniTaskVoid Spawn(Vector3 relativePos,Quaternion rot,Prefab_Prop weaponL,Prefab_Prop weaponR,Prefab_Prop shield)
    {
        _agent.enabled = true;
        _outlineTarget.CutoutTextureName = GameManager.s_advanceddissolvecutoutstandardmap1;
        _isReady = false;
        gameObject.SetActive(true);
        _animator.Rebind();
        _animator.SetBool(GameManager.s_spawn,Random.Range(0,2)==1);
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
            _dissolveRatio -= Time.deltaTime*dissolveSpeed;
            float ratio = Mathf.Clamp01(_dissolveRatio);
            AdvancedDissolveProperties.Cutout.Standard.
                UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,ratio);
            _outlineTarget.CutoutThreshold = ratio;
            _shadow.localScale = _shadowScale*(1 - ratio);
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }
        _outlineTarget.CutoutTextureName = GameManager.s_basemap;
        _outlineTarget.CutoutThreshold = 0.5f;
        AdvancedDissolveProperties.Cutout.Standard.
            UpdateLocalProperty(_material,AdvancedDissolveProperties.Cutout.Standard.Property.Clip,0);
        _shadow.localScale = _shadowScale;
    }
    protected async UniTaskVoid Despawn()
    {
        _agent.enabled = false;
        _outlineTarget.CutoutTextureName = GameManager.s_advanceddissolvecutoutstandardmap1;
        _outlineTarget.CutoutThreshold = 0.0f;
        _isReady = false;
        _isAlive = false;
        _animator.SetBool(GameManager.s_death,true);
        await UniTask.Delay(TimeSpan.FromSeconds(1.25f), DelayType.DeltaTime);
        DeactivateUI();
        await UniTask.Delay(TimeSpan.FromSeconds(deathDealy), DelayType.DeltaTime);
        Unequip();
        p_spawn.Play();
        while (!_isAlive && _dissolveRatio<1)
        {
            _dissolveRatio += Time.deltaTime*dissolveSpeed;
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
        _shadow.localScale = GameManager.V3_Zero;

        seq_ui.Complete();
        t_punch.Complete();
        t_blink.Complete();
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), DelayType.DeltaTime);
        gameObject.SetActive(false);
    }
    
    //Setter
    public void Set_AnimBase(MonsterAnim_Base animBase)
    {
        _animBase = animBase;
    }
    public void Set_AnimationState(AnimationState animationState)
    {
        _animator.SetInteger(GameManager.s_state_type, (int)animationState);
        _animator.SetTrigger(GameManager.s_state_change);
    }
    public void Set_IsReadyTrue()
    {
        _isReady = true;
    }
    //Getter
    public bool Get_IsAlive()
    {
        return _isAlive;
    }
    public bool Get_IsReady()
    {
        return _isReady;
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
    
    //Core
    
    public virtual void Core_Hit(Transform attacker,Transform prop,TrailData trailData)
    {
        
    }

    public void Core_HitState(HitState hitState)
    {
        _hitState = hitState;
    }
}

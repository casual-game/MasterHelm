using System;
using System.Collections;
using System.Collections.Generic;
using AmazingAssets.AdvancedDissolve;
using Cysharp.Threading.Tasks;
using EPOOutline;
using HighlightPlus;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public partial class Monster : MonoBehaviour
{
    public virtual void Setting_Monster(AnimatorOverrideController animatorOverrideController)
    {
        _shadow = transform.Find(GameManager.s_shadow);
        _shadowScale = _shadow.localScale;
        
        smr = GetComponentInChildren<SkinnedMeshRenderer>();
        _material = smr.material;
        _meshRoot = smr.rootBone;
        _outlinable = GetComponent<Outlinable>();
        _highlightEffect = GetComponent<HighlightEffect>();
        _animator = GetComponent<Animator>();
        _animator.runtimeAnimatorController = animatorOverrideController;
        _outlineTarget = _outlinable.OutlineTargets[0];
        _dissolveRatio = 1.0f;
        
        _outlineTarget.renderer.material.SetFloat(GameManager.s_dissolveamount,1);
        _outlineTarget.CutoutThreshold = 1;
        
        //장비 설정
        Prefab_Prop l = _weaponL==null?null:Instantiate(_weaponL), 
            r = _weaponR==null?null:Instantiate(_weaponR), 
            s = _shield==null?null:Instantiate(_shield);
        List<Renderer> renderers = new List<Renderer>();
        if (l != null)
        {
            l.Setting_Monster(_outlinable,false,t_hand_l,null,this,GameManager.Folder_MonsterProp);
            renderers.AddRange(l.GetComponentsInChildren<Renderer>());
        }

        if (r != null)
        {
            r.Setting_Monster(_outlinable,false,t_hand_r,null,this,GameManager.Folder_MonsterProp);
            renderers.AddRange(r.GetComponentsInChildren<Renderer>());
        }

        if (s != null)
        {
            s.Setting_Monster(_outlinable,false,t_shield,null,this,GameManager.Folder_MonsterProp);
            renderers.AddRange(s.GetComponentsInChildren<Renderer>());
        }
        renderers.Add(GetComponentInChildren<SkinnedMeshRenderer>());
        _highlightEffect.SetTargets(transform,renderers.ToArray());
        _highlightEffect.highlighted = true;
        //생성
        Transform t = transform;
        _weaponL = l;
        _weaponR = r;
        _shield = s;
        //세팅
        Setting_UI();
        Setting_Effect();
        Setting_AI();
        Setting_Sound();
        Monsters.Add(this);
        DespawnEmmediately();
    }
    //Static,Public
    public static List<Monster> Monsters = new List<Monster>();
    [FoldoutGroup("MainData")] public Data_MonsterInfo monsterInfo;
    [FoldoutGroup("MainData")] public Prefab_Prop _weaponL, _weaponR, _shield;
    [FoldoutGroup("Anim")] public AnimationClip animIdle,animStrafeFwd,animStrafeLeft,animStrafeRight,animRun;
    //Private,Protected
    private float _dissolveRatio = 1.0f;
    private Material _material;
    private OutlineTarget _outlineTarget;
    private Transform _shadow;
    private Vector3 _shadowScale;
    protected Outlinable _outlinable;
    protected HighlightEffect _highlightEffect;
    protected MonsterAnim_Base _animBase;
    protected Animator _animator;
    protected Transform _meshRoot;
    
    protected bool _isAlive = false, _isReady = false;
    protected int currenthp;
    protected float deathDealy = 0.5f;
    protected float dissolveSpeed = 1.3f;
    protected SkinnedMeshRenderer smr;
    
    //Unitask
    public async UniTaskVoid Spawn(Vector3 relativePos,Quaternion rot)
    {
        _agent.enabled = true;
        _outlineTarget.CutoutTextureName = GameManager.s_dissolvemap;
        _isReady = false;
        gameObject.SetActive(true);
        _animator.Rebind();
        _isAlive = true;
        p_spawn.Play();
        SoundManager.Play(sd_spawn);
        Voice_Attack();
        transform.rotation = rot;
        Move_Nav(relativePos);
        ActivateUI();
        
        Equipment_Equip();
        while (_isAlive && _dissolveRatio>0)
        {
            _dissolveRatio -= Time.deltaTime*dissolveSpeed;
            float ratio = Mathf.Clamp01(_dissolveRatio);
            _outlineTarget.renderer.material.SetFloat(GameManager.s_dissolveamount,ratio);
            _outlineTarget.CutoutThreshold = ratio;
            _shadow.localScale = _shadowScale*(1 - ratio);
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }
        _outlineTarget.CutoutTextureName = GameManager.s_basemap;
        _outlineTarget.CutoutThreshold = 0.5f;
        _outlineTarget.renderer.material.SetFloat(GameManager.s_dissolveamount,0);
        _shadow.localScale = _shadowScale;
    }
    public async UniTaskVoid Despawn()
    {
        _outlineTarget.CutoutTextureName = GameManager.s_dissolvemap;
        _outlineTarget.CutoutThreshold = 0.0f;
        _isReady = false;
        _isAlive = false;
        _animator.SetBool(GameManager.s_force,true);
        await UniTask.Delay(TimeSpan.FromSeconds(1.25f), DelayType.DeltaTime);
        DeactivateUI();
        await UniTask.Delay(TimeSpan.FromSeconds(deathDealy), DelayType.DeltaTime);
        Equipment_Unequip();
        p_spawn.Play();
        SoundManager.Play(sd_spawn);
        while (!_isAlive && _dissolveRatio<1)
        {
            _dissolveRatio += Time.deltaTime*dissolveSpeed;
            float ratio = Mathf.Clamp01(_dissolveRatio);
            _outlineTarget.renderer.material.SetFloat(GameManager.s_dissolveamount,ratio);
            _outlineTarget.CutoutThreshold = ratio;
            _shadow.localScale = _shadowScale*(1 - ratio);
            await UniTask.Yield(this.GetCancellationTokenOnDestroy());
        }
        _outlineTarget.renderer.material.SetFloat(GameManager.s_dissolveamount,1);
        _outlineTarget.CutoutThreshold = 1;
        _shadow.localScale = GameManager.V3_Zero;

        seq_ui.Complete();
        t_punch.Complete();
        t_blink.Complete();
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), DelayType.DeltaTime);
        _agent.enabled = false;
        gameObject.SetActive(false);
        GameManager.Instance.AI_Enqueue(this);
    }

    public void DespawnEmmediately()
    {
        _agent.enabled = false;
        _outlineTarget.CutoutTextureName = GameManager.s_dissolvemap;
        _outlineTarget.CutoutThreshold = 0.0f;
        _isReady = false;
        _isAlive = false;
        _animator.SetBool(GameManager.s_force,true);
        DeactivateUI();
        Equipment_Unequip();
        p_spawn.Play();
        _outlineTarget.renderer.material.SetFloat(GameManager.s_dissolveamount,1);
        _outlineTarget.CutoutThreshold = 1;
        _shadow.localScale = GameManager.V3_Zero;
        seq_ui.Complete();
        t_punch.Complete();
        t_blink.Complete();
        gameObject.SetActive(false);
    }
    //Setter
    public void Set_AnimBase(MonsterAnim_Base animBase)
    {
        _animBase = animBase;
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

    public void Move_Nav(Vector3 relativePos)
    {
        _agent.Move(relativePos);
    }
    public void Move_Normal(Vector3 nextPos,Quaternion nextRot)
    {
        transform.SetPositionAndRotation(nextPos, nextRot);
    }
    
}

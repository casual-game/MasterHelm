using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WeaponPack", menuName = "Data/WeaponPack", order = 1)]
public class Data_WeaponPack : ScriptableObject
{
    //기본 설정
    [TitleGroup("기본 설정")] [FoldoutGroup("기본 설정/Data")] public Prefab_Prop wepaon_L, weapon_R;
    [TitleGroup("기본 설정")] [FoldoutGroup("기본 설정/Data")] public bool useShield = false;
    [TitleGroup("기본 설정")] [FoldoutGroup("기본 설정/Data")] public Gradient colorOverTrail,colorOverLifetime;
    [TitleGroup("기본 설정")] [FoldoutGroup("기본 설정/Data")] public bool cancelableEffect = true;
    //[TitleGroup("기본 설정")] [FoldoutGroup("기본 설정/Data")] public List<ParticleSystem> attackEffects = new List<ParticleSystem>();
    //[TitleGroup("기본 설정")] [FoldoutGroup("기본 설정/Data")] public List<SoundData>  effectSounds = null;
    
    //메인 무기
    [FormerlySerializedAs("PlayerAttackMotionDatas_Main")] [TitleGroup("일반 공격으로 사용")] 
    [FoldoutGroup("일반 공격으로 사용/Data")][LabelText("일반 공격 정보")]
    public List<PlayerAttackMotionData> PlayerAttackMotionDatas_Normal = new List<PlayerAttackMotionData>();
    //스킬
    [TitleGroup("강 공격으로 사용")] [FoldoutGroup("강 공격으로 사용/Data")]
    public bool isLeftIsMirror = false;

    [TitleGroup("강 공격으로 사용")] [FoldoutGroup("강 공격으로 사용/Data")]
    public float weaponOffRatio = 1.0f;
    [TitleGroup("강 공격으로 사용")] [FoldoutGroup("강 공격으로 사용/Data")] [LabelText("강 공격 정보 (Left 기준)")]
    public PlayerAttackMotionData playerAttackMotionData_Strong;

}
[System.Serializable]
public class PlayerAttackMotionData
{
    [PropertySpace(16)][DetailedInfoBox("클립 설정에 대하여...",
        "클립 설정에 대하여...\n \n" +
        "1. 공격 클립은 필수입니다.\n" +
        "2. 마지막 공격은 차지 클립이 필요 없습니다.\n" +
        "3. 스킬 공격도 차지 클립이 필요 없습니다.")]
    [LabelText("공격 클립")] public AnimationClip clip_attack;
    [LabelText("차지 클립")] public AnimationClip clip_charge;
    [LabelText("종료 상태")] public PlayerAttackType playerAttackType_End;
    [PropertySpace(8)]
    public float playSpeed = 1.0f;
    public float moveSpeed = 1.0f;
    public float transitionRatio = 0.41f;
    public float chargeComboDelay = 0.0f;
    [InfoBox("ChargeWaitDuration: 마지막 기본공격에서는 구르기 딜레이 시간으로 사용됩니다.")]
    public float chargeWaitDuration = 0.5f;
    
    [PropertySpace(16)]
    public List<TrailData> TrailDatas = new List<TrailData>();
}
[System.Serializable]
public class TrailData
{
    [FoldoutGroup("TrailData")] [TitleGroup("TrailData/공격 정보 설정")]public bool useCustomParticle;
    [FoldoutGroup("TrailData")] [TitleGroup("TrailData/공격 정보 설정")][ShowIf("useCustomParticle")] public SoundData customParticle_Sound;
    [FoldoutGroup("TrailData")] [TitleGroup("TrailData/공격 정보 설정")][ShowIf("useCustomParticle")] public ParticleData customParticle_Particle;

    [FoldoutGroup("TrailData")] [TitleGroup("TrailData/공격 정보 설정")] 
    [ShowIf("useCustomParticle")][Range(0,4)] public int customParticle_ShakeRatio = 0;
    [FoldoutGroup("TrailData")] [TitleGroup("TrailData/공격 정보 설정")] public SoundData soundData;
    [TitleGroup("TrailData/공격 정보 설정")] public AttackType attackType_ground;
    [TitleGroup("TrailData/공격 정보 설정")] public bool isAirSmash;
    [TitleGroup("TrailData/공격 정보 설정")] public Vector2Int damage = new Vector2Int(10,15);
    [TitleGroup("TrailData/공격 정보 설정")] public int regain = 2;
    [TitleGroup("TrailData/공격 정보 설정")] public int charge_mp = 1;
    
    
    
    
    [TitleGroup("TrailData/타이밍 설정")] public bool weaponL, weaponR, shield;
    [TitleGroup("TrailData/타이밍 설정")] [MinMaxSlider(0,1,true)]
    public Vector2 trailRange = new Vector2(0,1);

    [TitleGroup("TrailData/타이밍 설정")]
    [Button("$GetHitscanButtonName")]
    public void ChangeHitScan()
    {
        isHitScan = !isHitScan;
    }
    private string GetHitscanButtonName()
    {
        if (isHitScan) return "HitScan 사용중";
        else return "실시간 동적 충돌계산 사용중";
    }
    [HideInInspector] public bool isHitScan = false;
    [TitleGroup("TrailData/타이밍 설정")][ShowIf("$isHitScan")]
    public Vector3 hitscan_pos, hitscan_rot, hitscan_scale;
    [TitleGroup("TrailData/타이밍 설정")][HideIf("$isHitScan")]
    [MinMaxSlider(0,1,true)]
    public Vector2 collisionRange = new Vector2(0,1);
}
[System.Serializable]
public class TrailData_Monster : TrailData
{
    [TitleGroup("TrailData/공격 정보 설정")][LabelText("회피 방식")][EnumToggleButtons] public EvadeType evadeType;
    [TitleGroup("TrailData/공격 정보 설정")][LabelText("공격 판정")][EnumToggleButtons]  public AttackMotionType attackMotionType = AttackMotionType.Center;
    [TitleGroup("TrailData/공격 정보 설정")][EnumToggleButtons,HideLabel] public HitType hitType = HitType.Normal;
    [FoldoutGroup("TrailData")] [TitleGroup("TrailData/공격 정보 설정")] public float playSpeed = 1.0f, moveSpeed = 1.0f;
    [TitleGroup("TrailData/공격 정보 설정")] public bool rotateToHero = true;
    [TitleGroup("TrailData/공격 정보 설정")] [ShowIf("rotateToHero")] public float rotateDuration = 1.0f;
}
public enum PlayerAttackType
{
    LeftState =0, RightState =1
}
public enum EvadeType
{
    Backward =0, LeftSide =1 ,RightSide = 2
}

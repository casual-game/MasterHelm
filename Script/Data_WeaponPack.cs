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
    [TitleGroup("기본 설정")] [FoldoutGroup("기본 설정/Data")] [LabelText("이펙트 적용 그라디언트")]
    public Gradient mainGradient;

    [TitleGroup("기본 설정")] [FoldoutGroup("기본 설정/Data")]
    public List<ParticleSystem> attackEffects = new List<ParticleSystem>();
    //메인 무기
    [FormerlySerializedAs("PlayerAttackMotionDatas_Main")] [TitleGroup("일반 공격으로 사용")] [FoldoutGroup("일반 공격으로 사용/Data")][LabelText("일반 공격 정보")]
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
    [FormerlySerializedAs("playerAttackEndType")]
    [PropertySpace(16)]
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
    [TitleGroup("AttackData")] public AttackType attackType_ground;
    [TitleGroup("AttackData")] public AttackType attackType_extra;
    [TitleGroup("AttackData")] public bool isAirSmash;
    [TitleGroup("TrailData")] public bool weaponL, weaponR, shield;
    [MinMaxSlider(0,1,true)][TitleGroup("TrailData")] 
    public Vector2 trailRange = new Vector2(0,1);
    
    
    
    [TitleGroup("InteractType")]public bool isHitScan = false;
    [HideIf("$isHitScan")][TitleGroup("InteractType")][MinMaxSlider(0,1,true)] 
    public Vector2 collisionRange = new Vector2(0,1);
    [ShowIf("$isHitScan")] [TitleGroup("InteractType")]
    public Vector3 hitscan_pos, hitscan_rot, hitscan_scale;
}
public enum PlayerAttackType
{
    LeftState =0, RightState =1
}

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
    [TitleGroup("기본 설정")] [FoldoutGroup("기본 설정/Data")] [LabelText("핵심 이펙트")]
    public ParticleSystem mainEffect;
    //메인 무기
    [FormerlySerializedAs("PlayerAttackMotionDatas_Main")] [TitleGroup("일반 공격으로 사용")] [FoldoutGroup("일반 공격으로 사용/Data")][LabelText("일반 공격 정보")]
    public List<PlayerAttackMotionData> PlayerAttackMotionDatas_Normal = new List<PlayerAttackMotionData>();
    //스킬
    [TitleGroup("강 공격으로 사용")] [FoldoutGroup("강 공격으로 사용/Data")]
    public bool isLeftIsMirror = false;

    [TitleGroup("강 공격으로 사용")] [FoldoutGroup("강 공격으로 사용/Data")]
    public float weaponOffRatio = 0.5f;
    [TitleGroup("강 공격으로 사용")] [FoldoutGroup("강 공격으로 사용/Data")] [LabelText("강 공격 정보 (Left 기준)")]
    public PlayerAttackMotionData playerAttackMotionData_Strong;

}
[System.Serializable]
public class PlayerAttackMotionData
{
    [FormerlySerializedAs("playerAttackEndType")]
    [PropertySpace(16)]
    [LabelText("시작 상태")] public PlayerAttackType playerAttackType_Start;
    [LabelText("종료 상태")] public PlayerAttackType playerAttackType_End;
    [PropertySpace(8)]
    public float playSpeed = 1.0f;
    public float moveSpeed = 1.0f;
    public float chargeWaitDuration = 0.5f;
    [FormerlySerializedAs("chargeTransitionRatio")] public float inputRatio = 0.41f;
    [PropertySpace(16)]
    public List<TrailData> TrailDatas = new List<TrailData>();
}
[System.Serializable]
public class TrailData
{
    [MinMaxSlider(0,1,true)] public Vector2 trailRange;
    public bool weaponL, weaponR, shield;
}
public enum PlayerAttackType
{
    LeftState =0, RightState =1
}

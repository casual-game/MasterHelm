using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WeaponPack", menuName = "Data/WeaponPack", order = 1)]
public class Data_WeaponPack : ScriptableObject
{
    public Prefab_Prop wepaon_L, weapon_R;
    public bool useShield = false;
    
    [TitleGroup("메인 무기")] [FoldoutGroup("메인 무기/Data")] [LabelText("이펙트 적용 그라디언트")]
    public Gradient mainGradient;
    [TitleGroup("메인 무기")] [FoldoutGroup("메인 무기/Data")] [LabelText("핵심 이펙트")]
    public ParticleSystem mainEffect;
    [TitleGroup("메인 무기")] [FoldoutGroup("메인 무기/Data")][LabelText("메인 공격 정보")]
    public List<PlayerAttackMotionData> PlayerAttackMotionDatas = new List<PlayerAttackMotionData>();

    

}
[System.Serializable]
public class PlayerAttackMotionData
{
    [PropertySpace(16)]
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

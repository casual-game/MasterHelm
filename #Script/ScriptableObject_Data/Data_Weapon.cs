using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Waepon", menuName = "Data/Data_Weapon", order = 1)]
public class Data_Weapon : ScriptableObject
{
    [TitleGroup("Setting")] [BoxGroup("Setting/s",false)] public ElementalAttributes elementalAttributes;
    [TitleGroup("Setting")] [BoxGroup("Setting/s", false)] public Data_Audio swingSound;
    [TitleGroup("Setting")] [BoxGroup("Setting/s",false)] public bool useShield = false;
    [TitleGroup("Setting")] [BoxGroup("Setting/s",false)] public bool canParry = false;

    [TitleGroup("Setting")] [BoxGroup("Setting/s", false)] public float manaChargeSpeed = 0.25f;
    [TitleGroup("Setting")] [BoxGroup("Setting/s",false)] public Prefab_Prop prefab_Left=null;
    [TitleGroup("Setting")] [BoxGroup("Setting/s",false)] public Prefab_Prop prefab_Right=null;
    [TitleGroup("Setting")] [BoxGroup("Setting/s", false)] public Data_Skill_L SkillL;
    [TitleGroup("Setting")] [BoxGroup("Setting/s", false)] public Data_Skill_R SkillR;

    [TitleGroup("Main Animation")] [BoxGroup("Main Animation/Locomotion")] [FoldoutGroup("Main Animation/Locomotion/Data")] 
    public AnimationClip idle;
    [TitleGroup("Main Animation")] [BoxGroup("Main Animation/Locomotion")] [FoldoutGroup("Main Animation/Locomotion/Data")] 
    public AnimationClip moveStart_Fwd,moveStart_L90,moveStart_L180,moveStart_R90,moveStart_R180;
    [TitleGroup("Main Animation")] [BoxGroup("Main Animation/Locomotion")]
    [FoldoutGroup("Main Animation/Locomotion/Data")] public AnimationClip move;

    [TitleGroup("Main Animation")] [BoxGroup("Main Animation/Attack")]
    public List<AttackData> normalAttacks = new List<AttackData>();

    [TitleGroup("Main Animation")] [BoxGroup("Main Animation/Roll")]
    public RollData roll_Normal, roll_Special;
    
}
[System.Serializable]
public class AttackData
{
    [FoldoutGroup("Data")][LabelText("공격 준비")] public MotionData motion_AttackReady;
    [FoldoutGroup("Data")][LabelText("차지")] public MotionData motion_Charge;
    [Space]
    [FoldoutGroup("Data")][LabelText("공격")] public MotionData_Attack motion_Attack;
    [FoldoutGroup("Data")][LabelText("차지공격")] public MotionData_Attack motion_ChargeAttack;


}

[System.Serializable]
public class MotionData
{
    public AnimationClip clip;
    public float moveSpeed = 0.8f;
    public float animSpeed = 1.0f;
    public AnimationCurve animSpeedCurve = AnimationCurve.Constant(0.0f,1.0f,1.0f);
}

[System.Serializable]
public class MotionData_Attack: MotionData
{
    [MinMaxSlider(0, 1, true)] public Vector2 canInput;
    public List<TrailData> trails;
    public float endRatio=0.85f;
    public float dashEffectRatio = -0.1f;
    public enum MotionType{LEFT=0,RIGHT=1}
    public enum HitType{ Left=0,Right=1,Front=2,Back=3,Front2=4,Smash=5 }
    public MotionType endMotionType = MotionType.LEFT;
    public HitType hitType = HitType.Front;


}
[System.Serializable]
public class RollData
{
    public AnimationClip clip;
    public float endRatio= 0.8f;
    public float moveSpeed = 0.75f;
    public float animSpeed = 1.0f;
    public AnimationCurve animCurve = AnimationCurve.Constant(0,1,1);
    public AnimationCurve rotCurve = AnimationCurve.Linear(0,0,1,1);
}
[System.Serializable]
public class TrailData
{
    [MinMaxSlider(0, 1, true)] public Vector2 range;
    [LabelText("왼쪽,오른쪽,방패 Trail 설정: ")][HorizontalGroup("trail")]public bool left;
    [HideLabel][HorizontalGroup("trail")]public bool right,shield;
}

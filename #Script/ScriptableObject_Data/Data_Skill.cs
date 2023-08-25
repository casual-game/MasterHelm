using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
public class Data_Skill : ScriptableObject
{
    public enum MotionType{LEFT=0,RIGHT=1}
    public MotionType endMotionType = MotionType.LEFT;
    public float weaponOffRatio = 0.6f;
    public List<SkillData> motions = new List<SkillData>();
}
[System.Serializable]
public class SkillData
{
    [MinMaxSlider(0, 1, true)] public Vector2 canInput;
    public AnimationClip clip;
    public float animSpeed = 1.0f;
    public AnimationCurve speedCurve= AnimationCurve.Constant(0,1,1);
    public float moveSpeed = 0.6f;
    public float endRatio = 0.95f;
    public List<TrailData> trails = new List<TrailData>();
    public float dashRatio = -1;
    public enum HitType{ Left=0,Right=1,Front=2,Back=3,Front2=4,Smash_L=5,Smash_R=6 }
    public HitType hitType = HitType.Front;

    [FoldoutGroup("Special Effects",false)]
    [TitleGroup("Special Effects/스킬 스윙 이펙트")] public Vector3 sp_slash_pos, sp_slash_rot, sp_slash_scale = Vector3.one;

    [TitleGroup("Special Effects/스킬 스윙 이펙트")] public float sp_slash_speed = 1.0f, sp_slash_delay = 0.0f;
    [TitleGroup("Special Effects/스킬 타격 이펙트")] public float sp_hit_scale = 1.0f;
}

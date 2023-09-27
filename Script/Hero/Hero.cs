using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public partial class Hero : MonoBehaviour
{
    
    [TitleGroup("움직임")][FoldoutGroup("움직임/기타")] public float crouchingSpeed = 5.0f;
    [TitleGroup("움직임")][FoldoutGroup("움직임/기타")] public float ladderClimbMotionSpeed = 2.0f;
    [TitleGroup("움직임")][FoldoutGroup("움직임/일반")] public AnimationCurve moveCurve;
    [TitleGroup("움직임")][FoldoutGroup("움직임/일반")] public float moveMotionSpeed_normal = 0.75f;
    [TitleGroup("움직임")][FoldoutGroup("움직임/일반")] public float acceleration_normal = 1.5f;
    [TitleGroup("움직임")][FoldoutGroup("움직임/일반")] public float deceleration_normal = 2.5f;
    [TitleGroup("움직임")][FoldoutGroup("움직임/일반")] public float turnDuration_normal = 0.25f;
    [TitleGroup("움직임")][FoldoutGroup("움직임/웅크리기")] public float moveMotionSpeed_crouch = 0.65f;
    [TitleGroup("움직임")][FoldoutGroup("움직임/웅크리기")] public float acceleration_crouch = 1.5f;
    [TitleGroup("움직임")][FoldoutGroup("움직임/웅크리기")] public float deceleration_crouch = 2.5f;
    [TitleGroup("움직임")][FoldoutGroup("움직임/웅크리기")] public float turnDuration_crouch = 0.25f;
    [TitleGroup("움직임")] [FoldoutGroup("움직임/구르기")] public float moveMotionSpeed_roll = 0.5f; 
    [TitleGroup("움직임")] [FoldoutGroup("움직임/구르기")] public float turnDuration_roll = 0.75f; 
    [TitleGroup("움직임")] [FoldoutGroup("움직임/LookAt")] public float lookDisplayDuration = 0.125f;
    [TitleGroup("움직임")] [FoldoutGroup("움직임/LookAt")] public float lookTargetDuration = 0.5f;
    [TitleGroup("움직임")] [FoldoutGroup("움직임/LookAt")]
    [MinMaxSlider(-180,180,true)] public Vector2 lookRange,lookRangeDeadZone;
    [TitleGroup("전투")] [FoldoutGroup("전투/피격")] public float hit_Strong_MoveDistance = 1.0f;
    [TitleGroup("전투")] [FoldoutGroup("전투/피격")] public float hit_Smash_MotionSpeed = 1.0f;
    [TitleGroup("전투")] [FoldoutGroup("전투/피격")] public float hit_Smash_RecoveryInputDelay = 0.2f;
    [TitleGroup("전투")] [FoldoutGroup("전투/차지")] public float chargeDuration = 1.0f;
    
    [TitleGroup("Input")] public float dash_roll_delay = 0.15f;
}

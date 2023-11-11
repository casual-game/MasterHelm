using System;
using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public partial class GameManager : MonoBehaviour
{
    //Static
    public static GameManager Instance;
    public static readonly Vector3 V3_Zero = new Vector3(0, 0, 0), V3_One = new Vector3(1, 1, 1);
    public static readonly Quaternion Q_Identity = Quaternion.identity;
    public static string s_speed = "Speed",s_rot = "Rot",s_crouch = "Crouch",s_state_change = "State_Change",s_state_type = "State_Type"
        ,s_footstep = "Footstep",s_charge_normal = "Charge_Normal",s_player = "Player",s_monster = "Monster"
        ,s_ladder = "Ladder",s_ladder_speed = "Ladder_Speed",s_transition = "Transition",s_death = "Death",s_isair = "IsAir"
        ,s_hit = "Hit",s_hit_additive = "Hit_Additive",s_hit_rot = "Hit_Rot",s_hit_type = "Hit_Type",s_turn = "Turn"
        ,s_publiccolor = "_PublicColor",s_leftstate = "LeftState",s_chargeenterindex = "ChargeEnterIndex",s_spawn = "Spawn",s_shadow = "Shadow"
        ,s_fadeamount = "_FadeAmount",s_shinelocation = "_ShineLocation",s_chromaberramount = "_ChromAberrAmount"
        ,s_radius = "_radius",s_size = "_size",s_wavesize = "_wavesize",s_position = "_position"
        ,s_action = "Action",s_combobegin = "콤보 시작",s_truecombo = "확정 콤보",s_smash = "강력한 공격",s_combofinish = "콤보 마무리",s_kill = "적 처치"
        ,s_normalattack = "첫번째 공격",s_continuousattack = "연속 공격",s_counter = "카운터 성공"
        ,s_advanceddissolvecutoutstandardmap1 = "_AdvancedDissolveCutoutStandardMap1",s_basemap = "_BaseMap",s_maincolor = "_MainColor"
        ,s_bluramount = "_BlurAmount",s_colour = "_Colour";
    
    //Public
    public AnimationCurve curve_inout,curve_in,curve_out;
    public DamageNumber dmp_normal, dmp_strong, dmp_weak;
    
    //기본 함수
    public void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        Setting_Resource();
        Setting_UI();
        Setting_Shockwave();
    }
    public void LateUpdate()
    {
        E_LateUpdate?.Invoke();
    }
}
public enum AttackMotionType {Center=0,LeftSlash=60,RightSlash=-60}
public enum AttackType {Normal = 0,Stun=1,Smash=2,Combo=3}
public enum HitType {Normal =-1,Bound=2,Screw=3,Flip=4,Smash=5,Stun=6}

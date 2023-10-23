using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public partial class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    
    public static string s_speed = "Speed",s_rot = "Rot",s_crouch = "Crouch",s_state_change = "State_Change",s_state_type = "State_Type"
        ,s_footstep = "Footstep",s_charge_normal = "Charge_Normal",s_player = "Player",s_monster = "Monster"
        ,s_ladder = "Ladder",s_ladder_speed = "Ladder_Speed",s_transition = "Transition",s_death = "Death"
        ,s_hit = "Hit",s_hit_additive = "Hit_Additive",s_hit_rot = "Hit_Rot",s_hit_type = "Hit_Type"
        ,s_publiccolor = "_PublicColor",s_leftstate = "LeftState",s_chargeenterindex = "ChargeEnterIndex",s_spawn = "Spawn",s_shadow = "Shadow";
    public AnimationCurve curve_inout,curve_in,curve_out;
    public void Awake()
    {
        Instance = this;
        Setting_Resource();
    }
    public void LateUpdate()
    {
        E_LateUpdate?.Invoke();
    }
}
public enum AttackMotionType {Center=0,LeftSlash=60,RightSlash=-60}
public enum PlayerSmashedType {None =-1,Bound=2,Screw=3,Flip=4,Center=5,Stun=6}

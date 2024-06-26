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
    //기본 함수
    public void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        FindObjectOfType<SoundManager>().Setting();
        FindObjectOfType<BgmManager>().Setting();
        FindObjectOfType<ParticleManager>().Setting();
        FindObjectOfType<CamArm>().Setting();
        FindObjectOfType<Hero>().Setting();
        Setting_Resource();
        Setting_UI();
        Setting_Shockwave();
        Setting_AI();
        Setting_Area();
    }
    public void LateUpdate()
    {
        E_LateUpdate?.Invoke();
    }
}
public enum AttackType {Normal = 0,Weak = 1,Stun=2,Smash=3}

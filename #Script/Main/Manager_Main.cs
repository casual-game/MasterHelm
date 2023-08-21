using System;
using System.Collections;
using System.Collections.Generic;
using AmazingAssets.AdvancedDissolve;
using Beautify.Universal;
using DamageNumbersPro;
using MoreMountains.NiceVibrations;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public partial class Manager_Main : MonoBehaviour
{
    
    public static Manager_Main instance;
    public Data_Main mainData;
    public Transform _folder_;
    
    [TitleGroup("TextEffect")][FoldoutGroup("TextEffect/text effect",false)][SerializeField]
    private DamageNumber damage_number,damage_text,damage_dangerText,damage_BIGText,
        damage_Highlight,damage_Combo_Main,damage_Combo_Specific,damage_Combo_Info;
    [TitleGroup("TextEffect")] [FoldoutGroup("TextEffect/text effect", false)] [SerializeField]
    private RectTransform T_DamageComboMain,T_DamageComboSpecific,T_DamagecomboInfo;

    [TitleGroup("TextEffect")] [FoldoutGroup("TextEffect/text effect", false)] [SerializeField]
    private Animator damageAnimator;
    
    [HideInInspector] public Manager_Blood manager_Blood = null;
    [HideInInspector] public Manager_Pooler manager_Pooler = null;
    [HideInInspector] public Manager_Enemy manager_Enemy = null;
    private UniversalAdditionalLightData lightData;
    //See Trough
    private Transform dissolve_start,dissolve_end;
    private AdvancedDissolveGeometricCutoutController dissolveController;
    
    void Awake()
    {
        EnemyStart.starts.Clear();
        DestructibleObject.destructibleObjects.Clear();
        Enemy.enemies.Clear();
        StopAllCoroutines();
        StartCoroutine("C_Setting");
    }
    private IEnumerator C_Setting()
    {
        Time.timeScale = 1;
        GraphicsSettings.useScriptableRenderPipelineBatching = true;
        Application.targetFrameRate = 60;
        instance = this;
        
        //라이트 쿠키 설정
        lightData = GetComponentInChildren<UniversalAdditionalLightData>();
        //각종 Manager 생성
        manager_Pooler = gameObject.AddComponent<Manager_Pooler>();
        manager_Pooler.Setting();
        SoundManager[] soundManagers = FindObjectsOfType<SoundManager>();
        foreach (var soundManager in soundManagers)
        {
            soundManager.Setting();
        }
        mainData.Setting_Audio();
        //Player_Canvas 생성
        Canvas_Player canvasPlayer = FindObjectOfType<Canvas_Player>(false);
        if ( canvasPlayer== null) Debug.LogError("Canvas_Player가 없습니다.");
        else canvasPlayer.Setting();
        canvasPlayer.anim.speed = 0;
        //Pooler,Blood,A*Pathfinder 등 각종 코루틴
        manager_Blood = gameObject.AddComponent<Manager_Blood>();
        StartCoroutine(manager_Blood.Setting());
        StartCoroutine(manager_Pooler.Add("Orb_Special",2));
        StartCoroutine(manager_Pooler.Add("Orb_Normal",2));
        StartCoroutine(manager_Pooler.Add("Explosion_Fire",3));
        StartCoroutine(manager_Pooler.Add("Weapon_Hit_Effect",4));
        StartCoroutine(manager_Pooler.Add("Weapon_RevengeHit_Effect",2));
        StartCoroutine(manager_Pooler.Add("Spark",20));
        StartCoroutine(manager_Pooler.Add("Shockwave",2));
        yield return null;
        while (true)
        {
            bool check = true;
            foreach (var value in manager_Pooler.pooler.Values)
            {
                if (!value.loaded)
                {
                    check = false;
                    break;
                }
            }
            yield return null;
            if (check) break;
        }
        //Player 생성
        PlayerStart playerStart = FindObjectOfType<PlayerStart>(false);
        if (playerStart == null) Debug.LogError("PlayerStart가 없습니다.");
        Player player = playerStart.Setting();
        FindObjectOfType<BeautifySettings>().depthOfFieldTarget = player.T_Head;
        //Canvas 추가 설정
        canvasPlayer.Setting_After();
        //Player_Canvas 생성
        Canvas_Player_World canvasPlayerWorld = FindObjectOfType<Canvas_Player_World>(false);
        canvasPlayerWorld.Setting();
        //CamArm 생성
        CamArm camArm = FindObjectOfType<CamArm>(false);
        if (camArm == null) Debug.LogError("CamArm이 없습니다.");
        else
        {
            camArm.target = player.T_CamRoot;
            camArm.Setting();
            camArm.MoveToTarget();
        }
        //Manager_Enemy생성
        manager_Enemy = gameObject.AddComponent<Manager_Enemy>();
        manager_Enemy.Setting();
        //See Trough
        dissolveController = FindObjectOfType<AdvancedDissolveGeometricCutoutController>();
        dissolve_start = player.transform.Find("dissolve_start");
        dissolve_end = player.transform.Find("dissolve_end");
        dissolve_start.transform.SetParent(_folder_);
        dissolve_end.transform.SetParent(_folder_);
        dissolveController.target1StartPointTransform = dissolve_start;
        dissolveController.target1EndPointTransform = dissolve_end;
        dissolve_start.transform.localPosition = Vector3.zero;
        dissolve_end.transform.localPosition = Vector3.zero;
        //Sparkable
        /*
        Sparkable.Sparkables = new List<Sparkable>();
        foreach (var sparkable in FindObjectsOfType<Sparkable>())
        {
            sparkable.Setting();
        }
        */
        //GetItem
        UI_GetItem getItem = FindObjectOfType<UI_GetItem>(true);
        getItem.Setting();
        //기타등등
        Setting_Spawner();
        StartCoroutine(C_Room_Update());
        Tutorial tutorial = FindObjectOfType<Tutorial>();
        
        if (tutorial != null)
        {
            PlayerPrefs.SetFloat("HP",100);
            tutorial.Setting();
        }
        LoadHP();
        //Breakable
        /*
        foreach (var breakable in FindObjectsOfType<DestructibleObject>())
        {
            breakable.Setting();
        }
        */
        //Barricade
        foreach (var barricede in FindObjectsOfType<Barricade>())
        {
            barricede.Setting();
        }
        //페이드 인
        canvasPlayer.anim.speed = 1;
        camArm.MoveToTarget();
        canvasPlayer.anim.CrossFade("Anim_Enter",0,0,0);
        canvasPlayer.audio_Enter.Play();
        SoundManager.instance.Ingame_Enter();
    }
    
    #region Effect_Text
    
    
    public void Text_Num(Vector3 pos,float num)
    {
        damage_number.Spawn(pos, num);
    }
    public void Text_Normal(Vector3 pos,string txt)
    {
        damage_text.Spawn(pos, txt);
    }
    public void Text_Danger(Vector3 pos,string txt)
    {
        damage_dangerText.Spawn(pos, txt);
    }
    public void Text_Big(Vector3 pos,string txt)
    {
        damage_BIGText.Spawn(pos, txt);
    }
    public void Text_Highlight(Vector3 pos,string txt)
    {
        damage_Highlight.Spawn(pos, txt);
    }
    
    
    private string s_damagespecific = "Damage_Specific", s_damagemain = "Damage_Main",
        s_damageinfo = "Damage_Info", s_damageinfo_fin = "Damage_Info_Fin";
    private int combo = 0;
    private float lastComboTime = -100, comboDelay = 2.5f;
    private DamageNumber damageinfo = null;
    public void Text_Damage_Main()
    {
        if (Time.unscaledTime - lastComboTime < comboDelay) combo += 1;
        else combo = 1;

        lastComboTime = Time.unscaledTime;
        
        DamageNumber damageNumber =damage_Combo_Main.Spawn(Vector3.zero, combo);
        damageNumber.SetAnchoredPosition(T_DamageComboMain, new Vector2(0, 0));
        damageAnimator.CrossFade(s_damagemain, 0,0,0);
    }
    public void Text_Damage_Specific(string specific)
    {
        DamageNumber damageNumber2 =damage_Combo_Specific.Spawn(Vector3.zero, specific);
        damageNumber2.SetAnchoredPosition(T_DamageComboSpecific, new Vector2(0, 0));
        damageAnimator.CrossFade(s_damagespecific, 0,1,0);
    }

    public void Text_Info(string info)
    {
        if (damageinfo != null)
        {
            damageinfo.FadeOut();
            damageinfo = null;
        }
        damageinfo =damage_Combo_Info.Spawn(Vector3.zero, info);
        damageinfo.SetAnchoredPosition(T_DamagecomboInfo, new Vector2(0, 0));
        damageAnimator.CrossFade(s_damageinfo, 0,0,0);
    }
    public void Text_Info_Fin()
    {
        damageinfo.FadeOut();
        damageAnimator.CrossFade(s_damageinfo_fin, 0,0,0);
        damageinfo = null;
    }
    #endregion
    
    #region Vibrate

    private bool isAndroid = false,isGamepad = false;

    public void Vibrate(float sec,float power01)
    {
        MMNVAndroid.AndroidVibrate((long)(sec*1000),Mathf.RoundToInt(power01*255));
        MMNVRumble.Rumble(power01,power01,sec,this);
    }
    

    #endregion

    #region Scene
    public void NextScene()
    {
        SaveHP();
        //StartCoroutine(C_LoadScene(stage_next));
    }

    public void ResetScene()
    {
        //StartCoroutine(C_LoadScene(stage_reset));
    }

    private IEnumerator C_LoadScene(string sceneName)
    {
        //Canvas_Player.instance.FadeOut();
        yield return new WaitForSecondsRealtime(1.5f);
        SceneManager.LoadScene(sceneName);
    }

    public void SaveHP()
    {
        //PlayerPrefs.SetFloat("HP",Canvas_Player.instance.health.current);
    }

    public void LoadHP()
    {
        //Canvas_Player.instance.health.SetValue(PlayerPrefs.GetFloat("HP"));
    }

    #endregion

    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        OnDrawGizmos_Spawner();
        //if(gizmoTab == 0) OnDrawGizmos_Spawner();
        //else if (gizmoTab == 1) OnDrawGizmos_Room();
    }
    #endif
    [HideInInspector] public int gizmoTab = 0;
}

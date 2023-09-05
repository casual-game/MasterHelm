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
    private DamageNumber damage_number_normal,damage_number_strong,damage_numbertext
        ,damage_Combo_Main,damage_Combo_Specific,damage_Combo_Info;
    [TitleGroup("TextEffect")] [FoldoutGroup("TextEffect/text effect", false)] [SerializeField]
    private RectTransform T_DamageComboMain,T_DamageComboSpecific,T_DamagecomboInfo;

    [TitleGroup("TextEffect")] [FoldoutGroup("TextEffect/text effect", false)] [SerializeField]
    private Animator damageAnimator;
    
    [HideInInspector] public Manager_Blood manager_Blood = null;
    [HideInInspector] public Manager_Pooler manager_Pooler = null;
    [HideInInspector] public Manager_Enemy manager_Enemy = null;
    //See Trough
    private Transform dissolve_start,dissolve_end;
    private AdvancedDissolveGeometricCutoutController dissolveController;
    
    void Awake()
    {
        Enemy.enemies.Clear();
        StopAllCoroutines();
        StartCoroutine("C_Setting");
    }
    private IEnumerator C_Setting()
    {
        int targetWidth = 1280;
        float ratio = (float)Screen.height/(float)Screen.width;
        print(Mathf.RoundToInt(targetWidth*ratio));
        Screen.SetResolution(targetWidth,Mathf.RoundToInt(targetWidth*ratio),true);
        Time.timeScale = 1;
        GraphicsSettings.useScriptableRenderPipelineBatching = true;
        Application.targetFrameRate = 48;
        instance = this;
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
        //맵 로딩
        for (int i = startLoadingRange.x-1; i < startLoadingRange.y; i++)
        {
            yield return SceneManager.LoadSceneAsync(scenenames[i], LoadSceneMode.Additive);
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

    private string s_superarmor = "무적!",s_guarded = "막힘!";
    [HideInInspector] public string specific_counter = "카운터",
        specific_stun = "적 스턴",
        specific_stunstreak = "연속 스턴",
        specific_evade = "완벽한 회피",
        specific_punish = "완벽한 반격",
        specific_kill = "몬스터 처치",
        specific_guardbreak = "가드 브레이크",
        specefic_cleararea = "구역 이동",
        specific_allclear = "올 클리어",
        specific_sniping = "스나이핑",
        specific_endure = "버티기",
        specific_guard = "방어";
    
    public void Damage_Normal(Vector3 pos,float num)
    {
        damage_number_normal.Spawn(pos, num);
    }
    public void Damage_Strong(Vector3 pos,float num)
    {
        damage_number_strong.Spawn(pos, num);
    }
    public void Damage_Guarded(Vector3 pos)
    {
        damage_numbertext.Spawn(pos, s_guarded);
    }
    public void Damage_Missed(Vector3 pos)
    {
        damage_numbertext.Spawn(pos, s_superarmor);
    }
    private string s_damagespecific = "Damage_Specific",s_damagespecific_fin = "Damage_Specific_Fin", s_damagemain = "Damage_Main",
        s_damageinfo = "Damage_Info", s_damageinfo_fin = "Damage_Info_Fin";
    private int combo = 0;
    private float lastComboTime = -100, comboDelay = 4.5f;
    private DamageNumber damageinfo = null,damagespecefic = null;
    public void Text_Damage_Main()
    {
        //Main
        if (Time.unscaledTime - lastComboTime < comboDelay) combo += 1;
        else combo = 1;
        CalculateAction(combo);
        lastComboTime = Time.unscaledTime;
        
        DamageNumber damageNumber =damage_Combo_Main.Spawn(Vector3.zero, combo);
        damageNumber.SetAnchoredPosition(T_DamageComboMain, new Vector2(0, 0));
        damageAnimator.CrossFade(s_damagemain, 0,0,0);
    }
    public void Text_Damage_Main(string specific)
    {
        Text_Damage_Main();
        //Specific
        Text_Damage_Specefic(specific);
    }

    

    public void Text_Info(string info,string specific)
    {
        if (damageinfo != null)
        {
            damageinfo.FadeOut();
            damageinfo = null;
        }
        damageinfo =damage_Combo_Info.Spawn(Vector3.zero, info);
        damageinfo.SetAnchoredPosition(T_DamagecomboInfo, new Vector2(0, 0));
        damageAnimator.CrossFade(s_damageinfo, 0,0,0);

        Text_Damage_Specefic(specific);
    }
    public void Text_Info_Fin()
    {
        if(damageinfo!=null) damageinfo.FadeOut();
        damageAnimator.CrossFade(s_damageinfo_fin, 0,0,0);
        damageinfo = null;
    }
    private void Text_Damage_Specefic(string specific)
    {
        if (damagespecefic != null)
        {
            damagespecefic.FadeOut();
            damagespecefic = null;
        }
        damagespecefic =damage_Combo_Specific.Spawn(Vector3.zero, specific);
        damagespecefic.SetAnchoredPosition(T_DamageComboSpecific, new Vector2(0, 0));
        damageAnimator.CrossFade(s_damagespecific, 0,1,0);
    }
    public void Text_Specefic_Fin()
    {
        if(damagespecefic!=null) damagespecefic.FadeOut();
        damageAnimator.CrossFade(s_damagespecific_fin, 0,1,0);
        damagespecefic = null;
    }
    #endregion
    
    #region Vibrate

    private bool isAndroid = false,isGamepad = false;

    public void Vibrate(float sec,float power01)
    {
        //MMNVAndroid.AndroidVibrate((long)(sec*1000),Mathf.RoundToInt(power01*255));
        //MMNVRumble.Rumble(power01,power01,sec,this);
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
    public enum SpawnerState
    {
        Enter=0,Fight=1,Exit=2
    }
    [ReadOnly][EnumToggleButtons][TitleGroup("에디터 툴")][HideLabel]
    public SpawnerState spawnerState = SpawnerState.Enter;
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        OnDrawGizmos_Spawner();
    }
    #endif
    [HideInInspector] public int gizmoTab = 0;
}

using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
public partial class GameManager : MonoBehaviour
{
    [TitleGroup("콤보 시스템 인스펙터")]
    [TabGroup("콤보 시스템 인스펙터/ComboUI","인게임",SdfIconType.Controller)]
    public DamageNumber dmp_norm, dmp_special;
    
    
    private float comboDelay = 2.5f;//-> 콤보 텍스트 초기화되는 딜레이 시간
    
    private Sequence s_combo;
    private DamageNumber dmp_created_main, dmp_created_sub;
    private float comboBeginTime = -100;
    private int comboAction = 0, damageAction = 0;
    private string subTest;
    private void Setting_UI()
    {
        
    }
    
    private void Combo()
    {
        if (Time.time - comboBeginTime > comboDelay)
        {
            comboAction = 1;
            comboBeginTime = Time.time;
        }
        else
        {
            comboAction++;
            comboBeginTime = Time.time;
        }
    }
    public void ComboText_Norm(Vector3 pos)
    {
        Combo();
        pos += Vector3.up * 1.75f + Random.insideUnitSphere;
        dmp_norm.Spawn(pos, comboAction + "콤보");
    }
    private void Damaged()
    {
        damageAction++;
    }
    public void DamagedText_Norm(Vector3 pos)
    {
        Damaged();
        pos += Vector3.up * 1.75f + Random.insideUnitSphere;
        dmp_norm.Spawn(pos, damageAction + "피격");
    }
    public void ComboText_Smash(Vector3 pos)
    {
        Combo();
        pos += Vector3.up * 2.25f + Random.insideUnitSphere;
        dmp_special.Spawn(pos, "피니쉬!");
    }

    public void ComboText_Kill(Vector3 pos)
    {
        Combo();
        pos += Vector3.up * 2.25f + Random.insideUnitSphere;
        dmp_special.Spawn(pos, "처치!");
    }
    
    public void ComboText_Counter(Vector3 pos)
    {
        Combo();
        pos += Vector3.up * 2.25f + Random.insideUnitSphere;
        dmp_special.Spawn(pos, "카운터!");
    }
}

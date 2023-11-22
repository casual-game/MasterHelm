using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using Random = UnityEngine.Random;

public class Frame_Main : MonoBehaviour
{
    private RectTransform frame_main;
    private Vector2 frameAnchoredPos;
    private Sequence s_hp,s_mp;
    [TitleGroup("HP 데이터")] public Image i_hp_main, i_hp_lerp;
    [TitleGroup("HP 데이터")] public TMP_Text tmp_hp;
    [TitleGroup("HP 데이터")] public Color c_lerp_main, c_lerp_hit, c_tmp_main, c_tmp_hit;
    private int maxhp,currenthp;
    private float i_hp_width = 413.01f,hp_CurrentRatio;
    
    
    [TitleGroup("MP 데이터")] public Image[] i_mp_slots = new Image[3];
    [TitleGroup("MP 데이터")] public Sprite sprite_mp_charged, sprite_mp_charging;
    [TitleGroup("MP 데이터")] public ParticleImage[] p_usemp = new ParticleImage[3];

    private int mp_Slot_Capacity,mp_Slot_CurrentCapacity;
    private float i_mp_width = 83.1372f;


    public void Setting(int hp,int mp_capacity)
    {
        frame_main = GetComponent<RectTransform>();
        frameAnchoredPos = frame_main.anchoredPosition;
        maxhp = hp;
        currenthp = maxhp;
        hp_CurrentRatio = 1;
        mp_Slot_Capacity = mp_capacity;
        mp_Slot_CurrentCapacity = 0;
        foreach (var slot in i_mp_slots)
        {
            slot.sprite = sprite_mp_charging;
            slot.rectTransform.sizeDelta = new Vector2(0, 20.3274f);
        }
        float ratio = (float)currenthp / (float)maxhp;
        float width = i_hp_width * ratio;
        i_hp_lerp.rectTransform.sizeDelta = new Vector2(width, 35.219f);
        i_hp_main.rectTransform.sizeDelta = new Vector2(width, 37.9649f);
        tmp_hp.text = currenthp + "/" + maxhp + " (" + 100 + "%)";
        Charge_MP(100);
    }
    [Button]
    public void HP_Damage(int damage=10)
    {
        s_hp.Stop();
        currenthp = Mathf.Clamp(currenthp - damage, 0, maxhp);
        float ratio = (float)currenthp / (float)maxhp;
        float width = i_hp_width * ratio;
        i_hp_lerp.color = c_lerp_hit;
        tmp_hp.color = c_tmp_hit;
        frame_main.anchoredPosition = frameAnchoredPos;
        
        
        s_hp = Sequence.Create().Group(Tween.UISizeDelta(i_hp_main.rectTransform, new Vector2(width, 37.9649f),
                0.6f, Ease.OutQuart, useUnscaledTime: true))
            .Group(Tween.UISizeDelta(i_hp_lerp.rectTransform, new Vector2(width, 35.219f),
                0.5f, Ease.InOutSine, useUnscaledTime: true, startDelay: 1.5f))
            .Group(Tween.Custom(hp_CurrentRatio, ratio, 0.6f, onValueChange: f =>
            {
                hp_CurrentRatio = f;
                tmp_hp.text = currenthp + "/" + maxhp + " (" + Mathf.RoundToInt(f * 100) + "%)";
            }, ease: Ease.OutQuart, useUnscaledTime: true))
            .Group(Tween.Color(i_hp_lerp, c_lerp_main, 0.75f, ease: Ease.InOutQuart, useUnscaledTime: true))
            .Group(Tween.Color(tmp_hp, c_tmp_main, 0.75f, ease: Ease.InOutQuart, useUnscaledTime: true))
            .Group(Tween.Custom(0,1,0.5f,useUnscaledTime:true,onValueChange: newVal =>
            {
                Vector2 RandomVec = Random.insideUnitCircle.normalized * Mathf.Clamp01(2-2*newVal) * 3;
                frame_main.anchoredPosition = frameAnchoredPos + RandomVec;
            } ));

    }

    [Button]
    public void Charge_MP(int charge =1)
    {
        s_mp.Stop();
        int startIndex = Mathf.Clamp(mp_Slot_CurrentCapacity / mp_Slot_Capacity,0,2);
        int startCapacity = mp_Slot_CurrentCapacity;
        mp_Slot_CurrentCapacity = Mathf.Clamp(mp_Slot_CurrentCapacity + charge,0,mp_Slot_Capacity*3);
        int targetIndex = Mathf.Clamp(mp_Slot_CurrentCapacity / mp_Slot_Capacity,0,2);
        float width = i_mp_width * (mp_Slot_CurrentCapacity - mp_Slot_Capacity*targetIndex) / (1.0f*mp_Slot_Capacity);
        //Change Image,Tween
        s_mp = Sequence.Create();
        for (int i = 0; i <= 2; i++)
        {
            if (i == targetIndex)
            {
                bool isfull = mp_Slot_CurrentCapacity == (targetIndex + 1) * mp_Slot_Capacity;
                if(isfull) i_mp_slots[i].sprite = sprite_mp_charged;
                else i_mp_slots[i].sprite = sprite_mp_charging;
                
                s_mp.Group(Tween.UISizeDelta(i_mp_slots[i].rectTransform, new Vector2(width, 20.3274f),
                    0.5f, Ease.OutQuart, useUnscaledTime: true));
            }
            else if(i<targetIndex)
            {
                i_mp_slots[i].sprite = sprite_mp_charged;
                s_mp.Group(Tween.UISizeDelta(i_mp_slots[i].rectTransform, new Vector2(i_mp_width, 20.3274f),
                    0.2f, Ease.OutQuart, useUnscaledTime: true));
            }
            else
            {
                i_mp_slots[i].sprite = sprite_mp_charged;
                s_mp.Group(Tween.UISizeDelta(i_mp_slots[i].rectTransform, new Vector2(0, 20.3274f),
                    0.2f, Ease.OutQuart, useUnscaledTime: true));
            }
        }
        
        if (startIndex < targetIndex)
        {
            p_usemp[startIndex].Play();
        }
        else if (mp_Slot_CurrentCapacity == mp_Slot_Capacity * i_mp_slots.Length && startCapacity!=mp_Slot_CurrentCapacity)
        {
            p_usemp[startIndex].Play();
        }
    }

    public bool MP_CanUse()
    {
        return mp_Slot_CurrentCapacity >= mp_Slot_Capacity;
    }
    public void MP_Use()
    {
        if (mp_Slot_CurrentCapacity < mp_Slot_Capacity)
        {
            #if UNITY_EDITOR
            print("마나 부족!");
            #endif
            return;
        }
        Charge_MP(-mp_Slot_Capacity);
    }
}

using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using PrimeTween;
public partial class Monster : MonoBehaviour
{
    
    protected virtual void Setting_Effect()
    {
        _customMaterialController = GetComponent<CustomMaterialController>();
        if (_customMaterialController != null)
        {
            List<Renderer> renderers = new List<Renderer>();
            renderers.Add(smr);
            if (_weaponL != null) AddPropRenderer(_weaponL);
            if (_weaponR != null) AddPropRenderer(_weaponR);
            if (_shield != null) AddPropRenderer(_shield);
            _customMaterialController.Setting(renderers);
            void AddPropRenderer(Prefab_Prop prop)
            {
                MeshRenderer renderer = prop.GetComponent<MeshRenderer>();
                renderers.Add(renderer);
            }
        }
    }
    //Public
    [FoldoutGroup("Effect")] public ParticleSystem p_spawn, p_smoke,p_blood_normal,p_blood_strong,p_blood_combo;
    //Private
    protected Tween t_blink, t_punch;
    protected CustomMaterialController _customMaterialController;
    //Punch
    public void Punch_Down(float speed)
    {
        t_punch.Complete();
        _meshRoot.localScale = GameManager.V3_One;
        t_punch = Tween.PunchScale(_meshRoot, strength: new Vector3(0.2f,-0.2f,0.2f),
            duration: 0.75f, frequency: 4, easeBetweenShakes: Ease.OutQuad,useUnscaledTime:true);
        t_punch.timeScale = speed;
    }
    public void Punch_Up(float speed)
    {
        t_punch.Complete();
        _meshRoot.localScale = GameManager.V3_One;
        t_punch = Tween.PunchScale(_meshRoot, strength: new Vector3(-0.2f,0.2f,-0.2f),
            duration: 0.75f, frequency: 4, easeBetweenShakes: Ease.OutQuad,useUnscaledTime:true);
        t_punch.timeScale = speed;
    }
    public void Punch_Down_Compact(float speed)
    {
        t_punch.Complete();
        _meshRoot.localScale = GameManager.V3_One;
        t_punch = Tween.PunchScale(_meshRoot, strength: new Vector3(0.125f,-0.125f,0.125f),
            duration: 0.45f, frequency: 3, easeBetweenShakes: Ease.OutSine,useUnscaledTime:true);
        t_punch.timeScale = speed;
    }
    public void Punch_Up_Compact(float speed)
    {
        t_punch.Complete();
        _meshRoot.localScale = GameManager.V3_One;
        t_punch = Tween.PunchScale(_meshRoot, strength: new Vector3(-0.125f,0.125f,-0.125f),
            duration: 0.45f, frequency: 3, easeBetweenShakes: Ease.OutSine,useUnscaledTime:true);
        t_punch.timeScale = speed;
    }
    //Effect
    public void Effect_Land()
    {
        p_smoke.Play();
        Punch_Up(0.75f);
    }
    public void Effect_Hit_Normal()
    {
        t_blink.Complete();
        t_blink = Tween.Custom(monsterInfo.c_hit_begin, monsterInfo.c_hit_fin, duration: 0.2f,
            onValueChange: newVal => _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, newVal)
            ,ease: Ease.Linear);
        
        
        Transform t = transform;
        Vector3 currentPos = t.position;
        p_blood_normal.Play();
    }
    public void Effect_Hit_Strong(bool isBloodBottom,bool isCombo)
    {
        t_blink.Complete();
        t_blink = Tween.Custom(monsterInfo.c_hit_begin, monsterInfo.c_hit_fin, duration: 0.3f,
            onValueChange: newVal => _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, newVal)
            ,ease: Ease.InQuad);
        p_blood_normal.Play();
        if(!isCombo) p_blood_strong.Play();
        else p_blood_combo.Play();
    }
    public void Effect_Hit_Counter()
    {
        t_blink.Complete();
        t_blink = Tween.Custom(Color.white, Color.clear, duration: 0.5f,
            onValueChange: newVal => _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, newVal)
            ,ease: Ease.InQuad);
        p_blood_normal.Play();
        p_blood_combo.Play();
    }
    //CustomMaterialController
    
    public void Deactivate_CustomMaterial()
    {
        _customMaterialController.Deactivate();
    }
    //Animation Event
    public void FallDown()
    {
        p_smoke.Play();
        Punch_Up_Compact(1.5f);
        Set_HitState(HitState.Recovery);
    }
}

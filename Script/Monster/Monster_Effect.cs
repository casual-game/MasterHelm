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
        if (subMat)
        {
            m_submat = new Material(m_submat);
            id_submat = Shader.PropertyToID(GameManager.s_maincolor);
            UpdateMaterial(smr.gameObject);
            if(_weaponL!=null) UpdateMaterial(_weaponL.gameObject);
            if(_weaponR!=null) UpdateMaterial(_weaponR.gameObject);
            if(_shield!=null) UpdateMaterial(_shield.gameObject);
            m_submat.SetColor(GameManager.s_maincolor,c_submatfin);
            void UpdateMaterial(GameObject prop)
            {
                Renderer meshRenderer = prop.GetComponent<Renderer>();
                Material[] materialArray = new Material[2];
                Material originalMat = meshRenderer.material;
                materialArray[0] = originalMat;
                materialArray[1] = m_submat;
                meshRenderer.materials = materialArray;
            }
        }
        
    }
    //Public
    [FoldoutGroup("Effect")] public ParticleSystem p_spawn, p_smoke,p_blood_normal,p_blood_strong,p_blood_combo;
    [FoldoutGroup("Effect")][ColorUsage(true,true)]  public Color c_hit_begin, c_hit_fin;
    [FoldoutGroup("Effect")][PropertySpace(16)] public bool subMat = false;
    [FoldoutGroup("Effect")][ShowIf("subMat")] public ParticleSystem p_submat;
    [FoldoutGroup("Effect")][ColorUsage(true,true)][ShowIf("subMat")] public Color c_submatbegin,c_submatfin;
    [FoldoutGroup("Effect")][ShowIf("subMat")]  public Material m_submat;
    //Private
    protected Tween t_blink, t_punch, t_submat;
    private int id_submat;
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
        t_blink = Tween.Custom(c_hit_begin, c_hit_fin, duration: 0.2f,
            onValueChange: newVal => _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, newVal)
            ,ease: Ease.Linear);
        
        
        Transform t = transform;
        Vector3 currentPos = t.position;
        p_blood_normal.Play();
        
        //Blood
        Vector3 bloodPos = currentPos + Vector3.up * 0.8f;
        Quaternion bloodRot = Quaternion.Euler(0,Random.Range(0,360),0);
        BloodManager.instance.Blood_Normal(ref bloodPos,ref bloodRot);
    }
    public void Effect_Hit_Strong(bool isBloodBottom,bool isCombo)
    {
        t_blink.Complete();
        t_blink = Tween.Custom(c_hit_begin, c_hit_fin, duration: 0.3f,
            onValueChange: newVal => _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, newVal)
            ,ease: Ease.InQuad);
        
        
        Transform t = transform;
        Vector3 currentPos = t.position;
        p_blood_normal.Play();
        if(!isCombo) p_blood_strong.Play();
        else p_blood_combo.Play();
        //Blood
        Vector3 bloodPos = currentPos + Vector3.up * 0.8f;
        Quaternion bloodRot;
        if (isBloodBottom)
        {
            bloodRot = Quaternion.Euler(0,Random.Range(0,360),0);
            BloodManager.instance.Blood_Strong_Bottom(ref bloodPos,ref bloodRot);
        }
        else
        {
            bloodRot = t.rotation;
            BloodManager.instance.Blood_Strong_Front(ref bloodPos,ref bloodRot);
        }
    }
    public void Effect_Submat(bool activate)
    {
        t_submat.Stop();
        if (activate)
        {
            t_submat = Tween.MaterialColor(m_submat, id_submat, c_submatbegin, 0.35f);
            p_submat.Play();
        }
        else
        {
            t_submat = Tween.MaterialColor(m_submat, id_submat, c_submatfin, 0.35f);
            p_submat.Stop(true,ParticleSystemStopBehavior.StopEmitting);
        }
    }
    //Animation Event
    public void FallDown()
    {
        p_smoke.Play();
        Punch_Up_Compact(1.5f);
        Core_HitState(HitState.Recovery);
    }
}

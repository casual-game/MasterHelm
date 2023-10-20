using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Monster : MonoBehaviour
{
    public Vector3 punch;
    public float punchD;
    public int punchV;
    private void Setting_Effect()
    {
        s_blink_hit_normal = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, c_hit_begin); })
            .Append(_outlinable.FrontParameters.FillPass
                .DOColor(GameManager.s_publiccolor, c_hit_fin, 0.2f).SetEase(Ease.Linear));
        s_blink_hit_strong = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, c_hit_begin); })
            .Append(_outlinable.FrontParameters.FillPass
                .DOColor(GameManager.s_publiccolor, c_hit_fin, 0.3f).SetEase(Ease.InQuad));
        s_punch_land = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _meshRoot.localScale = Vector3.one; })
            .Append(_meshRoot.DOPunchScale(new Vector3(-0.35f,0.35f,-0.35f), 0.4f,3)
            .SetEase(Ease.InOutElastic));
        s_punch_hit_normal = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _meshRoot.localScale = Vector3.one; })
            .Append(_meshRoot.DOPunchScale(new Vector3(0.12f,-0.12f,0.12f), 0.175f,3)
                .SetEase(Ease.InOutElastic));
        s_punch_hit_strong = DOTween.Sequence().SetAutoKill(false)
            .OnStart(() => { _meshRoot.localScale = Vector3.one; })
            .Append(_meshRoot.DOPunchScale(new Vector3(0.15f,-0.15f,0.15f), 0.75f,7)
                .SetEase(Ease.InOutBack));
    }
    
    //Public
    [FoldoutGroup("Effect")] public ParticleSystem p_spawn, p_smoke,p_blood_normal,p_blood_strong;
    [FoldoutGroup("Effect")][ColorUsage(true,true)]  public Color c_hit_begin, c_hit_fin;
    
    //Private
    protected Sequence s_blink_hit_normal, s_blink_hit_strong,s_punch_land,s_punch_hit_normal,s_punch_hit_strong;
    
    //Effect
    public void Effect_Land()
    {
        p_smoke.Play();
        if(!s_punch_land.IsInitialized()) s_punch_land.Play();
        else s_punch_land.Restart();
    }
    public void FallDown()
    {
        p_smoke.Play();
        if(!s_punch_hit_normal.IsInitialized()) s_punch_hit_normal.Play();
        else s_punch_hit_normal.Restart();
    }
    public void Effect_Hit_Normal()
    {
        if(!s_blink_hit_normal.IsInitialized()) s_blink_hit_normal.Play();
        else s_blink_hit_normal.Restart();
        
        if(!s_punch_hit_normal.IsInitialized()) s_punch_hit_normal.Play();
        else s_punch_hit_normal.Restart();
        
        Transform t = transform;
        Vector3 currentPos = t.position;
        p_blood_normal.Play();
        
        //Blood
        Vector3 bloodPos = currentPos + Vector3.up * 0.8f;
        Quaternion bloodRot = Quaternion.Euler(0,Random.Range(0,360),0);
        BloodManager.instance.Blood_Normal(ref bloodPos,ref bloodRot);
    }
    [Button]
    public void Effect_Hit_Strong(bool isBloodBottom)
    {
        if(!s_blink_hit_strong.IsInitialized()) s_blink_hit_strong.Play();
        else s_blink_hit_strong.Restart();
        
        if(!s_punch_hit_strong.IsInitialized()) s_punch_hit_strong.Play();
        else s_punch_hit_strong.Restart();
        
        Transform t = transform;
        Vector3 currentPos = t.position;
        p_blood_normal.Play();
        p_blood_strong.Play();
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
    
}

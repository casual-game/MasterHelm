using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Monster : MonoBehaviour
{
    private void Setting_Effect()
    {
        s_blink_hit_normal = DOTween.Sequence().SetAutoKill(false)
            .PrependCallback(() => { _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, c_hit_begin); })
            .Append(_outlinable.FrontParameters.FillPass
                .DOColor(GameManager.s_publiccolor, c_hit_fin, 0.2f).SetEase(Ease.Linear));
        s_blink_hit_strong = DOTween.Sequence().SetAutoKill(false)
            .PrependCallback(() => { _outlinable.FrontParameters.FillPass.SetColor(GameManager.s_publiccolor, c_hit_begin); })
            .Append(_outlinable.FrontParameters.FillPass
                .DOColor(GameManager.s_publiccolor, c_hit_fin, 0.3f).SetEase(Ease.InQuad));
        
        s_punch_up = DOTween.Sequence().SetAutoKill(false)
            .PrependCallback(() => { _meshRoot.localScale = GameManager.V3_One; })
            .Append(_meshRoot.DOPunchScale(new Vector3(-0.15f,0.15f,-0.15f), 0.75f,7)
                .SetEase(Ease.InOutBack));
        
        s_punch_down = DOTween.Sequence().SetAutoKill(false)
            .PrependCallback(() => { _meshRoot.localScale = GameManager.V3_One; })
            .Append(_meshRoot.DOPunchScale(new Vector3(0.15f,-0.15f,0.15f), 0.75f,7)
                .SetEase(Ease.InOutBack));
        s_punch_up_compact = DOTween.Sequence().SetAutoKill(false)
            .PrependCallback(() => { _meshRoot.localScale = GameManager.V3_One; })
            .Append(_meshRoot.DOPunchScale(new Vector3(-0.125f,0.125f,-0.125f), 0.45f,7)
                .SetEase(Ease.InOutBack));
        
        s_punch_down_compact = DOTween.Sequence().SetAutoKill(false)
            .PrependCallback(() => { _meshRoot.localScale = GameManager.V3_One; })
            .Append(_meshRoot.DOPunchScale(new Vector3(0.125f,-0.125f,0.125f), 0.45f,7)
                .SetEase(Ease.InOutBack));
    }
    
    //Public
    [FoldoutGroup("Effect")] public ParticleSystem p_spawn, p_smoke,p_blood_normal,p_blood_strong,p_blood_combo;
    [FoldoutGroup("Effect")][ColorUsage(true,true)]  public Color c_hit_begin, c_hit_fin;
    
    //Private
    protected Sequence s_blink_hit_normal, s_blink_hit_strong,
                       s_punch_up,s_punch_down,s_punch_up_compact,s_punch_down_compact;
    
    //Punch
    public void Punch_Down(float speed)
    {
        if (s_punch_up.IsPlaying()) s_punch_up.Pause();
        if (s_punch_up_compact.IsPlaying()) s_punch_up_compact.Pause();
        if (s_punch_down_compact.IsPlaying()) s_punch_down_compact.Pause();
        
        s_punch_down.timeScale = speed;
        if(!s_punch_down.IsInitialized()) s_punch_down.Play();
        else s_punch_down.Restart();
    }
    public void Punch_Up(float speed)
    {
        if (s_punch_down.IsPlaying()) s_punch_down.Pause();
        if (s_punch_up_compact.IsPlaying()) s_punch_up_compact.Pause();
        if (s_punch_down_compact.IsPlaying()) s_punch_down_compact.Pause();
        
        s_punch_up.timeScale = speed;
        if(!s_punch_up.IsInitialized()) s_punch_up.Play();
        else s_punch_up.Restart();
    }
    public void Punch_Down_Compact(float speed)
    {
        if (s_punch_up.IsPlaying()) s_punch_up.Pause();
        if (s_punch_down.IsPlaying()) s_punch_down.Pause();
        if (s_punch_up_compact.IsPlaying()) s_punch_up_compact.Pause();
        
        
        s_punch_down_compact.timeScale = speed;
        if(!s_punch_down_compact.IsInitialized()) s_punch_down_compact.Play();
        else s_punch_down_compact.Restart();
    }
    public void Punch_Up_Compact(float speed)
    {
        if (s_punch_up.IsPlaying()) s_punch_up.Pause();
        if (s_punch_down.IsPlaying()) s_punch_down.Pause();
        if (s_punch_down_compact.IsPlaying()) s_punch_down_compact.Pause();
        
        s_punch_up_compact.timeScale = speed;
        if(!s_punch_up_compact.IsInitialized()) s_punch_up_compact.Play();
        else s_punch_up_compact.Restart();
    }
    //Effect
    public void Effect_Land()
    {
        p_smoke.Play();
        Punch_Up(0.75f);
    }
    public void Effect_Hit_Normal()
    {
        if(!s_blink_hit_normal.IsInitialized()) s_blink_hit_normal.Play();
        else s_blink_hit_normal.Restart();
        
        
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
        if(!s_blink_hit_strong.IsInitialized()) s_blink_hit_strong.Play();
        else s_blink_hit_strong.Restart();
        
        
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
    
    //Animation Event
    public void FallDown()
    {
        p_smoke.Play();
        Punch_Up_Compact(1.5f);
        Core_HitState(HitState.Recovery);
    }
}

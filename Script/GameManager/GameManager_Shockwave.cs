using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    [FoldoutGroup("Shockwave")]
    public Material mat_shockwave;
    private Sequence s_shockwave_strong, s_shockwave_normal;

    private void Setting_Shockwave()
    {
        s_shockwave_strong = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
            
            .PrependCallback(() =>
            {
                mat_shockwave.SetFloat(s_size, 0.85f);
                mat_shockwave.SetFloat(s_radius, 0.225f);
                mat_shockwave.SetFloat(s_wavesize, 0.3f);
            })
            .Append(mat_shockwave.DOFloat(1, s_radius, 9.5f).SetEase(Ease.OutExpo));
        
        s_shockwave_normal = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
            
            .PrependCallback(() =>
            {
                mat_shockwave.SetFloat(s_size, 0.6f);
                mat_shockwave.SetFloat(s_radius, 0.225f);
                mat_shockwave.SetFloat(s_wavesize, 0.25f);
            })
            .Append(mat_shockwave.DOFloat(1, s_radius, 4.0f).SetEase(Ease.OutExpo));
    }
    [Button]
    public void Shockwave_Strong(Vector3 pos)
    {
        mat_shockwave.SetVector(s_position, CamArm.instance.mainCam.WorldToScreenPoint(pos));
        
        if (s_shockwave_normal.IsPlaying()) s_shockwave_normal.Pause();

        if (!s_shockwave_strong.IsInitialized()) s_shockwave_strong.Play();
        else s_shockwave_strong.Restart();
    }
    [Button]
    public void Shockwave_Normal(Vector3 pos)
    {
        mat_shockwave.SetVector(s_position, CamArm.instance.mainCam.WorldToScreenPoint(pos));
        
        if (s_shockwave_strong.IsPlaying()) s_shockwave_strong.Pause();

        if (!s_shockwave_normal.IsInitialized()) s_shockwave_normal.Play();
        else s_shockwave_normal.Restart();
    }
}

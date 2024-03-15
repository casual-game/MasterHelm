using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    [TitleGroup("기타 데이터들")][BoxGroup("기타 데이터들/data",false)]
    public Material mat_shockwave;
    private Tween t_shockwave;
    private int properyID;

    private void Setting_Shockwave()
    {
        properyID = Shader.PropertyToID(s_radius);
        /*
        s_shockwave_normal = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)

            .PrependCallback(() =>
            {
                mat_shockwave.SetFloat(s_size, 0.75f);
                mat_shockwave.SetFloat(s_radius, 0.225f);
                mat_shockwave.SetFloat(s_wavesize, 0.25f);
            })
            .Append(mat_shockwave.DOFloat(1, s_radius, 4.5f).SetEase(Ease.OutExpo).SetUpdate(true));
        */
    }
    public void Shockwave(Vector3 pos,float duration = 1.25f,float waveSize = 0.125f,float saturation =0,Ease ease = Ease.Linear,float delay=0)
    {
        t_shockwave.Stop();
        mat_shockwave.SetFloat(s_radius, 0.225f);
        mat_shockwave.SetFloat(s_wavesize, waveSize);
        mat_shockwave.SetFloat(s_saturation, saturation);
        mat_shockwave.SetVector(s_position, CamArm.instance.mainCam.WorldToScreenPoint(pos));
        t_shockwave = Tween.MaterialProperty(mat_shockwave,properyID ,1.0f, duration,useUnscaledTime:true,ease:ease,startDelay:delay);
    }
}

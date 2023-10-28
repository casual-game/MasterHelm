using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public partial class Monster : MonoBehaviour
{
    [FoldoutGroup("UI")] public Image img_health_lerp;
    [FoldoutGroup("UI")] public Image img_health_main;
    private bool ui_activated = false;
    protected Sequence ui_Sequence_Activate, ui_Sequence_Deactivated;

    protected virtual void Setting_UI()
    {
        
    }
    protected virtual void ActivateUI()
    {
        currenthp = hp;
        img_health_main.fillAmount = 1;
        img_health_lerp.fillAmount = 1;
    }

    protected virtual void DeactivateUI()
    {
        
    }
    [Button]
    protected virtual void Core_Damage(float damage)
    {
        currenthp -= damage;
        float ratio = currenthp / hp;
        if (DOTween.IsTweening(img_health_main))
        {
            print("tweening");
            img_health_main.DOKill();
        }

        img_health_main.DOFillAmount(ratio, 0.5f).SetEase(Ease.OutQuart);
    }
}

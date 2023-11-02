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
    private bool _uiActivated = false;
    protected Sequence ui_Sequence_Activate, ui_Sequence_Deactivated;

    protected virtual void Setting_UI()
    {
        
    }
    protected virtual void ActivateUI()
    {
        _isAlive = true;
        currenthp = hp;
        img_health_main.fillAmount = 1;
        img_health_lerp.fillAmount = 1;
    }

    protected virtual void DeactivateUI()
    {
        
    }

    protected void Core_Damage_Normal(int damage)
    {
        GameManager.Instance.dmp_normal.Spawn(transform.position + Vector3.up * 1.2f, damage);
        Core_Damage(damage);
    }
    protected void Core_Damage_Weak(int damage)
    {
        GameManager.Instance.dmp_weak.Spawn(transform.position + Vector3.up * 1.2f, damage);
        Core_Damage(damage);
    }
    protected void Core_Damage_Strong(int damage)
    {
        GameManager.Instance.dmp_strong.Spawn(transform.position + Vector3.up * 1.2f, damage);
        Core_Damage(damage);
    }
    private void Core_Damage(int damage)
    {
        currenthp -= damage;
        if (currenthp > 0)
        {
            float ratio = (float)currenthp / (float)hp;
            if (DOTween.IsTweening(img_health_main)) img_health_main.DOKill();
            if (DOTween.IsTweening(img_health_lerp)) img_health_lerp.DOKill();
        
            img_health_main.DOFillAmount(ratio, 0.5f).SetEase(Ease.OutQuart).SetUpdate(true);
            img_health_lerp.DOFillAmount(ratio, 0.5f).SetEase(Ease.OutQuart).SetDelay(2.0f).SetUpdate(true);
        }
        else if(_isAlive)
        {
            _isAlive = false;
            if (DOTween.IsTweening(img_health_main)) img_health_main.DOKill();
            if (DOTween.IsTweening(img_health_lerp)) img_health_lerp.DOKill();
            img_health_main.DOFillAmount(0, 0.5f).SetEase(Ease.OutQuart).SetUpdate(true);
            img_health_lerp.DOFillAmount(0, 1.5f).SetEase(Ease.InOutSine).SetUpdate(true);
            Despawn().Forget();
        }
        
    }
}

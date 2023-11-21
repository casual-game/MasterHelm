using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using PrimeTween;

public partial class Monster : MonoBehaviour
{
    [FoldoutGroup("UI")] public Image img_health_lerp;
    [FoldoutGroup("UI")] public Image img_health_main;
    private bool _uiActivated = false;
    protected Tween t_dmg_main,t_dmg_lerp;
    protected Sequence seq_ui;

    protected virtual void Setting_UI()
    {
        
    }
    protected virtual void ActivateUI()
    {
        _isAlive = true;
        currenthp = monsterInfo.hp;
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
        damage = Mathf.CeilToInt(damage*GameManager.recoveryDamage);
        GameManager.Instance.dmp_weak.Spawn(transform.position + Vector3.up * 1.2f, damage);
        Core_Damage(damage);
    }
    protected void Core_Damage_Strong(int damage,bool spawn = true)
    {
        if(spawn) GameManager.Instance.dmp_strong.Spawn(transform.position + Vector3.up * 1.2f, damage);
        Core_Damage(damage);
    }
    protected virtual void Core_Damage(int damage)
    {
        currenthp -= damage;
        if (currenthp > 0)
        {
            float ratio = (float)currenthp / (float)monsterInfo.hp;
            t_dmg_main.Stop();
            t_dmg_lerp.Stop();
            t_dmg_main = Tween.UIFillAmount(img_health_main, ratio, 0.5f, Ease.OutQuart, useUnscaledTime: true);
            t_dmg_lerp = Tween.UIFillAmount(img_health_lerp, 
                ratio, 0.5f, Ease.OutQuart, useUnscaledTime: true,startDelay:2.5f);
        }
        else if(_isAlive)
        {
            _isAlive = false;
            t_dmg_main.Stop();
            t_dmg_lerp.Stop();
            t_dmg_main = Tween.UIFillAmount(img_health_main, 0, 0.5f, Ease.OutQuart, useUnscaledTime: true);
            t_dmg_lerp = Tween.UIFillAmount(img_health_lerp, 
                0, 1.5f, Ease.InOutSine, useUnscaledTime: true,startDelay:2.0f);
            Despawn().Forget();
        }
        
    }
}

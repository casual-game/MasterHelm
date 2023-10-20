using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public partial class Monster_Normal : Monster
{
    //Public
    [PropertySpace(8)]
    [FoldoutGroup("UI")] public Transform t_UIRoot;
    [FoldoutGroup("UI")] public Transform t_UI_XZ,t_UI_Y;
    [FoldoutGroup("UI")] public float ui_Height;
    [FoldoutGroup("UI")] public Image img_health_root;

    
    protected override void Setting_UI()
    {
        base.Setting_UI();
        
        //Activate Sequence
        ui_Sequence_Activate = DOTween.Sequence();
        ui_Sequence_Activate.SetAutoKill(false);
        ui_Sequence_Activate.Append(img_health_root.rectTransform
            .DOScale(0.0087626f, 0.5f).SetEase(Ease.OutBack));
        ui_Sequence_Activate.Append(img_health_root.rectTransform
            .DOSizeDelta(new Vector2(147.5f, 36), 0.5f).SetEase(Ease.InOutBack));
        ui_Sequence_Activate.OnPlay(() =>
        {
            GameManager.Instance.E_LateUpdate.RemoveListener(UI_Move);
            GameManager.Instance.E_LateUpdate.AddListener(UI_Move);
        });
        //Deactivate Sequence
        ui_Sequence_Deactivated = DOTween.Sequence();
        ui_Sequence_Deactivated.SetAutoKill(false);
        ui_Sequence_Deactivated.Append(img_health_root.rectTransform
            .DOSizeDelta(new Vector2(50, 36), 0.35f).SetEase(Ease.InOutBack));
        ui_Sequence_Deactivated.Append(img_health_root.rectTransform
            .DOScale(0, 0.35f).SetEase(Ease.InBack));
        ui_Sequence_Deactivated.OnComplete(() => GameManager.Instance.E_LateUpdate.RemoveListener(UI_Move));
    }
    protected override void ActivateUI()
    {
        base.ActivateUI();
        if (ui_Sequence_Deactivated.IsPlaying()) ui_Sequence_Deactivated.Pause();
        img_health_root.rectTransform.localScale = Vector3.zero;
        img_health_root.rectTransform.sizeDelta = new Vector2(50, 36);
        
        if (!ui_Sequence_Activate.IsInitialized()) ui_Sequence_Activate.Play();
        else ui_Sequence_Activate.Restart();
    }
    protected override void DeactivateUI()
    {
        base.DeactivateUI();
        if (ui_Sequence_Activate.IsPlaying()) ui_Sequence_Activate.Pause();
        img_health_root.rectTransform.localScale = Vector3.one*0.0087626f;
        img_health_root.rectTransform.sizeDelta = new Vector2(147.5f, 36);
        
        if (!ui_Sequence_Deactivated.IsInitialized()) ui_Sequence_Deactivated.Play();
        else ui_Sequence_Deactivated.Restart();
    }
    private void UI_Move()
    {
        Vector3 pos = t_UI_XZ.position;
        pos.y = t_UI_Y.position.y + ui_Height;
        t_UIRoot.SetPositionAndRotation(pos,CamArm.instance.mainCam.transform.rotation);
    }
}

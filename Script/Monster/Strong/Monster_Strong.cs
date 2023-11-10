using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using PrimeTween;

public partial class Monster_Strong : Monster
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
    }

    protected override void ActivateUI()
    {
        base.ActivateUI();
        seq_ui.Complete();
        img_health_root.rectTransform.localScale = GameManager.V3_Zero;
        img_health_root.rectTransform.sizeDelta = new Vector2(50, 36);
        GameManager.Instance.E_LateUpdate.RemoveListener(UI_Move);
        GameManager.Instance.E_LateUpdate.AddListener(UI_Move);

        seq_ui = Sequence.Create()
            .Chain(Tween.Scale(img_health_root.transform, 0.0087626f, 0.5f, Ease.OutBack))
            .Chain(Tween.UISizeDelta(img_health_root.rectTransform, new Vector2(147.5f, 36), 0.5f, Ease.InOutBack));
    }

    protected override void DeactivateUI()
    {
        base.DeactivateUI();
        seq_ui.Complete();
        img_health_root.rectTransform.localScale = GameManager.V3_One*0.0087626f;
        img_health_root.rectTransform.sizeDelta = new Vector2(147.5f, 36);

        seq_ui = Sequence.Create()
            .Chain(Tween.UISizeDelta(img_health_root.rectTransform, new Vector2(50, 36), 0.35f, Ease.InOutBack))
            .Chain(Tween.Scale(img_health_root.transform, 0, 0.35f, Ease.InBack))
            .ChainCallback(target: GameManager.Instance, target => target.E_LateUpdate.RemoveListener(UI_Move));
    }
    private void UI_Move()
    {
        Vector3 pos = t_UI_XZ.position;
        pos.y = t_UI_Y.position.y + ui_Height;
        t_UIRoot.SetPositionAndRotation(pos,CamArm.instance.mainCam.transform.rotation);
    }
}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public partial class Monster : MonoBehaviour
{
    [FoldoutGroup("UI")] public Image img_health_lerp, imge_health_main;
    private bool ui_activated = false;
    protected Sequence ui_Sequence_Activate, ui_Sequence_Deactivated;

    protected virtual void Setting_UI()
    {
        
    }
    protected virtual void ActivateUI()
    {
        
    }

    protected virtual void DeactivateUI()
    {
        
    }
}

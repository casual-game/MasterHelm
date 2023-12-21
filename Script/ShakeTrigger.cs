using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeTrigger : MonoBehaviour
{
    public enum ShakeType {Normal,Strong}
    public ShakeType shakeType = ShakeType.Strong;
    public void Shake()
    {
        switch (shakeType)
        {
            case ShakeType.Normal:
            default:
                CamArm.instance.Tween_ShakeNormal();
                break;
            case ShakeType.Strong:
                CamArm.instance.Tween_ShakeStrong();
                break;
        }
    }
}

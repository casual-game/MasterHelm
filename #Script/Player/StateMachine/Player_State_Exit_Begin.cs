using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State_Exit_Begin : Player_State_Base
{
    private Quaternion startRot, endRot;
    private AnimationCurve rotateCurve = AnimationCurve.EaseInOut(0,0,1,1);
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Transform t = player.transform;
        startRot = t.rotation;

        Vector3 vec = CamArm.instance.mainCam.transform.position - t.position;
        vec.y = 0;
        endRot = Quaternion.LookRotation(vec);
        player.audio_StageClear.Play();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        float ratio = rotateCurve.Evaluate(Mathf.Clamp01(stateInfo.normalizedTime / 0.25f));
        player.Move(player.transform.position,Quaternion.Lerp(startRot,endRot,ratio));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player_Guarded_Normal : Player_State_Base
{
    private Quaternion rot;
    public float endRatio=0.6f;
    public float guardMove = 0.6f;
    private float movedDist = 0;
    private float currentMoveScale = 1.0f;
    
    public AnimationCurve moveCurve= AnimationCurve.EaseInOut(0,0,1,1);
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Vector3 vec = Player.instance.guardPoint - player.transform.position;
        vec.y = 0;
        rot = Quaternion.LookRotation(vec);
        currentMoveScale = guardMove;
        
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);

        float ratio = stateInfo.normalizedTime / endRatio;
        float dist = currentMoveScale * moveCurve.Evaluate(Mathf.Clamp01(ratio)) - movedDist;
        movedDist += dist;
        Vector3 vec = rot * Vector3.back * dist;
        player.Move(player.transform.position +vec,player.transform.rotation);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Ladder_Off_Bottom : HeroAnim_Base
{
    public AnimationCurve moveCurve;
    private Vector3 _startPos, _endPos;
    private Quaternion _startRot, _endRot;
    private float _startRatio;
    private Ladder _ladder;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _ladder = movement.CurrentLadder;
        Transform mt = movement.transform;
        _startPos = mt.position;
        _startRot = mt.rotation;

        _endPos = _ladder.downPoint;
        _endRot = Quaternion.Euler(0,_ladder.transform.rotation.eulerAngles.y + 180,0);
        _startRatio = stateInfo.normalizedTime;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator, stateInfo)) return;
        if (stateInfo.normalizedTime < 0.7f)
        {
            float ratio = moveCurve.Evaluate((stateInfo.normalizedTime-_startRatio) / (0.7f-_startRatio));
            Vector3 nextPos = Vector3.Lerp(_startPos,_endPos,ratio);
            Quaternion nextRot = Quaternion.Lerp(_startRot,_endRot,ratio);
            movement.Move_Normal(nextPos, nextRot);
        }
        else
        {
            animator.SetBool(GameManager.s_ladder,false);
            movement.Get_NavMeshAgent().updatePosition = true;
            movement.Get_NavMeshAgent().Warp(_endPos);
            isFinished = true;
            return;
        }
    }
}
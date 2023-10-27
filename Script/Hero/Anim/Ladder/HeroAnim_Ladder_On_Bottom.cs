using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Ladder_On_Bottom : HeroAnim_Base
{
    private Vector3 _startPos, _endPos;
    private Quaternion _startRot, _endRot;
    private float _startRatio;
    private Ladder _ladder;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _ladder = _hero.CurrentLadder;
        animator.SetFloat(GameManager.s_ladder_speed,0);
        Transform mt = _hero.transform;
        Transform lt = _ladder.transform;
        _startRot = mt.rotation;
        _endRot = Quaternion.Euler(0,180 + lt.rotation.eulerAngles.y,0);
        _startPos = mt.position;
        _endPos = lt.position + lt.forward * 1.4f;
        _endPos.y = _ladder.range.x + 0.6f;
        _startRatio = stateInfo.normalizedTime;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
        if (stateInfo.normalizedTime< 0.5f)
        {
            float ratio = GameManager.Instance.curve_inout.Evaluate((stateInfo.normalizedTime-_startRatio)/(0.5f-_startRatio));
            Vector3 nextPos = Vector3.Lerp(_startPos, _endPos, ratio);
            Quaternion nextRot = Quaternion.Lerp(_startRot, _endRot, ratio);
            _hero.Move_Normal(nextPos, nextRot);
        }
        else
        {
            _hero.Move_Normal(_endPos,_endRot);
            isFinished = true;
            return;
        }
    }
}

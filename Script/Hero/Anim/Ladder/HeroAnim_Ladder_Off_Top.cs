using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Ladder_Off_Top : HeroAnim_Base
{
    public AnimationCurve moveCurve;
    private Vector3 _startPos, _endPos;
    private Quaternion _startRot, _endRot;
    private float _startRatio;
    private Ladder _ladder;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        _ladder = _hero.CurrentLadder;
        Transform mt = _hero.transform;
        _startPos = mt.position;
        _startRot = mt.rotation;
        _endPos = _ladder.upPoint;

        float endPos_x = Mathf.Lerp(_startPos.x, _endPos.x, 0.1f);
        float endPos_y = Mathf.Lerp(_startPos.y, _endPos.y, 0.75f);
        float endPos_z = Mathf.Lerp(_startPos.z, _endPos.z, 0.1f);
        _endPos = new Vector3(endPos_x, endPos_y, endPos_z);
        _endRot = Quaternion.Euler(0,_ladder.transform.rotation.eulerAngles.y + 180,0);
        _startRatio = stateInfo.normalizedTime;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator, stateInfo)) return;
        if (stateInfo.normalizedTime < 0.45f)
        {
            float ratio = moveCurve.Evaluate((stateInfo.normalizedTime-_startRatio) / (0.45f-_startRatio));
            Vector3 nextPos = Vector3.Lerp(_startPos,_endPos,ratio);
            Quaternion nextRot = Quaternion.Lerp(_startRot,_endRot,ratio);
            _hero.Move_Normal(nextPos, nextRot);
        }
    }
}

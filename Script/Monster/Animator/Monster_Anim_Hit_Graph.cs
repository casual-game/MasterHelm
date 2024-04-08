using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Anim_Hit_Graph : MonsterAnim_Base
{
    public AnimationCurve moveFlowCurve;
    public float moveDistanceRatio = 1.0f;
    public float endRatio = 0.6f;
    
    private Vector3 _startPos;
    private Vector3 _moveVec;
    private Quaternion _startRot;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Transform t = _monster.transform;
        _startPos = t.position;
        _startRot = t.rotation;
        _moveVec = t.forward;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator, stateInfo)) return;
        
        float normalizedTime = Mathf.Clamp01(stateInfo.normalizedTime);
        Vector3 finalPos = _startPos + _moveVec * moveFlowCurve.Evaluate(normalizedTime) * moveDistanceRatio;

        Transform t = _monster.transform;
        t.rotation = _startRot;
        _monster.Move_Nav(finalPos-t.position);

        if (normalizedTime > endRatio)
        {
            isFinished = true;
            animator.SetBool(GameManager.s_hit,false);
            animator.SetInteger(GameManager.s_state_type,0);
            return;
        }
    }
}

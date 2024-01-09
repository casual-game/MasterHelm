using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAnim_Flight : DragonAnim_Base
{
    private Vector3 startPos;
    private Vector3 destination;
    private float startTime;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        startPos = _dragon.transform.position;
        startTime = Time.time;
        if (_dragon.destination != null) destination = _dragon.destination.position;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
        Transform t = animator.transform;
        if (_dragon.destination == null)
        {
            Vector3 pos = t.position + animator.deltaPosition * (1.35f * Mathf.Min(stateInfo.normalizedTime * 6, 1));
            Quaternion rot = t.rotation * animator.deltaRotation;
            _dragon.transform.SetPositionAndRotation(pos, rot);
        }
        else
        {
            //변수 설정
            Vector3 _startPos = startPos;
            Vector3 _destination = destination;
            _startPos.y = 0;
            _destination.y = 0;
            float distance = Vector3.Distance(startPos, destination);
            float ratio = Mathf.Clamp01(((Time.time-startTime)*6.5f)/distance);
            //회전
            Vector3 dir = _destination - _startPos;
            var rot = Quaternion.Lerp(t.rotation, Quaternion.LookRotation(dir), 3 * Time.deltaTime);
            
            //2차함수로 높이 이쁘게 설정
            float height = Mathf.Clamp01(-4*ratio*ratio+ 4*ratio);
            animator.SetFloat(GameManager.s_flight_y,0.65f-ratio);
            //이동
            Vector3 pos = Vector3.Lerp(startPos,destination,ratio);
            pos.y = height*distance*0.2f;
            t.SetPositionAndRotation(pos,rot);
            if (ratio > 0.99f)
            {
                isFinished = true;
                animator.SetTrigger(GameManager.s_transition);
                return;
            }
        }
    }
}

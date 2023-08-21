using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Hit : StateMachineBehaviour
{
    private Enemy enemy=null;
    private Quaternion rot;
    
    private float movedDist = 0.125f;
    private float currentMoveScale = 1.0f;
    public AnimationCurve moveCurve= AnimationCurve.EaseInOut(0,0,1,1);
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (enemy == null) enemy = animator.GetComponent<Enemy>();
        Vector3 vec = Player.instance.transform.position - enemy.transform.position;
        vec.y = 0;
        rot = Quaternion.LookRotation(vec);
        movedDist = 0;
        Vector3 dist = enemy.transform.position - Player.instance.transform.position;
        dist.y = 0;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        float ratio = Mathf.Clamp01(stateInfo.normalizedTime / 0.6f);
        float dist = currentMoveScale * moveCurve.Evaluate(ratio) - movedDist;
        movedDist += dist;
        Vector3 vec = rot * Vector3.back * dist;
        enemy.Move(enemy.transform.position +vec,enemy.transform.rotation);
    }
}

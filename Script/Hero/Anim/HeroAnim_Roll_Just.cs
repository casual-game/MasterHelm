using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAnim_Roll_Just : HeroAnim_Base
{
    public float endRatio = 0.6f,moveSpeed = 1.0f,addEndDeg = 0;
    private int pattern = 0;
    private float startDeg,endDeg;
    private AnimationCurve rotateCurve = AnimationCurve.EaseInOut(0,0,1,1);
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        startDeg = _hero.transform.rotation.eulerAngles.y;
        if (GameManager.Bool_Move)
        {
            endDeg = Mathf.Atan2(GameManager.JS_Move.y, GameManager.JS_Move.x) * Mathf.Rad2Deg +
                     CamArm.instance.transform.rotation.eulerAngles.y;
            endDeg = -endDeg + 180 + addEndDeg;
        }
        else endDeg = startDeg;
        pattern = 0;
        animator.SetBool(GameManager.s_leftstate,false);
        _hero.Effect_Smoke();
        _hero.Tween_Blink_Evade(1.75f);
        _hero.Tween_Punch_Up_Compact(0.4f);
        _hero.Activate_Feather();
        _hero.frameMain.Charge_MP(_heroData.MP_Recovery_JustRoll);
        Quaternion targetRot = Quaternion.Euler(0,endDeg,0);
        _hero.Move_Nav(Vector3.zero, targetRot);
        pattern = 1;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateMove(animator, stateInfo, layerIndex);
        if (IsNotAvailable(animator,stateInfo)) return;
        Transform heroT = _hero.transform;
        Transform monsterT = _hero.Get_RollLookT();
        Quaternion targetRot;
        if (monsterT!=null && false)
        {
            Vector3 lookVec = monsterT.position - heroT.position;
            lookVec.y = 0;
            float deg = Quaternion.LookRotation(lookVec).eulerAngles.y;
            targetRot = Quaternion.Euler(0,deg,0);
        }
        else
        {
            targetRot = animator.rootRotation;
        }
        //최종 이동 설정
        Vector3 targetPos = animator.deltaPosition * moveSpeed;
        _hero.Move_Nav(targetPos,targetRot);

        if (stateInfo.normalizedTime > endRatio)
        {
            isFinished = true;
            _hero.Set_RolledTime();
            _hero.Deactivate_CustomMaterial();
            animator.SetBool(GameManager.s_roll,false);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public partial class Enemy : MonoBehaviour
{
    protected virtual void Pattern_Setting()
    {
        Pattern_Cancel();
    }

    private void Pattern_Disable()
    {
        Pattern_Cancel();
    }

    private Coroutine pattern_current = null;
    //메인 함수
    public IEnumerator Pattern_Set(Coroutine cpattern)
    {
        if(pattern_current!=null) StopCoroutine(pattern_current);
        pattern_current = cpattern;
        yield return cpattern;
        
    }

    public void Pattern_Cancel()
    {
        if(state_current!=null) StopCoroutine(state_current);
        if (stateMachineBehavior != null) stateMachineBehavior.finished = true;
        if(pattern_current!=null) StopCoroutine(pattern_current);
    }
    protected bool PERCENT(int _0_100)
    {
        int rd = Random.Range(0,100);
        return _0_100 >= rd;
    }

    protected bool ATTACKED(float duration = 1.0f)
    {
        return Time.time - attack_attackedTime < duration && attack_attacked;
    }
    //CPattern
    private IEnumerator CPattern_Tutorial()
    {
        while (showingup) yield return null;
        while (true)
        {
            yield return State_Set(StartCoroutine(CState_Chase_Fast(1.0f)));
            yield return State_Set(StartCoroutine(CState_Backstep()));
            while (Vector3.Distance(transform.position, Player.instance.transform.position) < 4.0f) yield return null;
        }
    }
   
    
}

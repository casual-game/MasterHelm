using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undead_2Hand : Enemy
{
    protected override void Pattern_Setting()
    {
        base.Pattern_Setting();
        Pattern_Set(StartCoroutine(CPattern_Undead_2Hand()));
    }
    private IEnumerator CPattern_Undead_2Hand()
    {
        while (showingup) yield return null;
        while (true)
        {
            yield return State_Set(StartCoroutine(CState_Chase_Fast(2.25f,1)));
            yield return State_Set(StartCoroutine(CState_Attack(1)));
            if (ATTACKED())
            {
                if (PERCENT(50))yield return State_Set(StartCoroutine(CState_Backstep()));
                if (PERCENT(50))
                {
                    
                    if (PERCENT(50))
                    {
                        yield return State_Set(StartCoroutine(CState_Wait(1.0f)));
                        yield return State_Set(StartCoroutine(CState_Chase_Fast(2.25f,1)));
                        yield return State_Set(StartCoroutine(CState_Attack(1)));
                    }
                    else
                    {
                        yield return State_Set(StartCoroutine(CState_Wait(2.0f)));
                        yield return State_Set(StartCoroutine(CState_Chase_Fast(2.0f)));
                        yield return State_Set(StartCoroutine(CState_Attack(0)));
                    }
                }
            }
            yield return State_Set(StartCoroutine(CState_Wait(2.0f)));

            if (PERCENT(50))
            {
                yield return State_Set(StartCoroutine(CState_Backstep()));
                yield return State_Set(StartCoroutine(CState_Attack(0)));
                yield return State_Set(StartCoroutine(CState_Chase_Fast(2.25f,1)));
                yield return State_Set(StartCoroutine(CState_Attack(1)));
            }
            yield return State_Set(StartCoroutine(CState_Wait(2.0f)));
        }
    }
}

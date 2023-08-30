using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undead_1Hand : Enemy
{
    protected override void Pattern_Setting()
    {
        base.Pattern_Setting();
        if(guard_use) Pattern_Set(StartCoroutine(CPattern_Undead_Guard()));
        else Pattern_Set(StartCoroutine(CPattern_Undead_1Hand()));
    }
    
    protected override void UI_BreakGuard()
    {
        base.UI_BreakGuard();
        Pattern_Set(StartCoroutine(CPattern_Undead_1Hand()));
    }
    private IEnumerator CPattern_Undead_1Hand()
    {
        while (showingup) yield return null;
        while (true)
        {
            yield return State_Set(StartCoroutine(CState_Chase_Fast(2.0f)));
            yield return State_Set(StartCoroutine(CState_Attack(0)));
            if (ATTACKED())
            {
                if (PERCENT(50))
                {
                    yield return State_Set(StartCoroutine(CState_Backstep()));
                    if (PERCENT(50))
                    {
                        yield return State_Set(StartCoroutine(CState_Wait(1.5f)));
                        yield return State_Set(StartCoroutine(CState_Attack(0)));
                    }
                    else
                    {
                        audio_create.Play();
                        yield return State_Set(StartCoroutine(CState_Attack(1)));
                        audio_roar.Play();
                        yield return State_Set(StartCoroutine(CState_Chase_Fast(2.75f,1)));
                        yield return State_Set(StartCoroutine(CState_Attack(2)));
                    }
                }
            }
            
            yield return State_Set(StartCoroutine(CState_Wait(2.0f)));

            yield return State_Set(StartCoroutine(CState_Backstep()));
            audio_create.Play();
            yield return State_Set(StartCoroutine(CState_Attack(1)));
            audio_roar.Play();
            yield return State_Set(StartCoroutine(CState_Chase_Fast(2.75f,1)));
            yield return State_Set(StartCoroutine(CState_Attack(2)));
        }
    }
    private IEnumerator CPattern_Undead_Guard()
    {
        while (showingup) yield return null;
        while (true)
        {
            yield return State_Set(StartCoroutine(CState_Chase_Fast(2.25f)));
            yield return State_Set(StartCoroutine(CState_Attack(0)));
            yield return State_Set(StartCoroutine(CState_Attack(1)));
            if (ATTACKED() && PERCENT(70))
            {
                yield return State_Set(StartCoroutine(CState_Attack(1)));
                yield return State_Set(StartCoroutine(CState_Chase_Fast(1.5f)));
                if (PERCENT(50)) yield return State_Set(StartCoroutine(CState_Attack(1)));
            }
            
            if (PERCENT(50)) yield return State_Set(StartCoroutine(CState_Backstep()));
            else yield return State_Set(StartCoroutine(CState_Attack(1)));
            
            if (PERCENT(50))
            {
                
                yield return State_Set(StartCoroutine(CState_Attack(0)));
                yield return State_Set(StartCoroutine(CState_Chase_Fast(2.25f)));
                yield return State_Set(StartCoroutine(CState_Attack(1)));
            }
            else
            {
                yield return State_Set(StartCoroutine(CState_Attack(1)));
                yield return State_Set(StartCoroutine(CState_Wait(2.0f)));
            }
        }
    }

    public void OneHand_StrongSwing()
    {
        Player.instance.DoHit(transform.position,currentSingleAttackData);
    }
    public void Guard_Stamp()
    {
        particle_smoke.Play();
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_SpecialSmooth);
    }
}

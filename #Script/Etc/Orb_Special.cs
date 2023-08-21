using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb_Special : Orb
{
    public override void EnabledEvent()
    {
        Player.instance.audio_action_firering.Play();
    }

    public override void Reward()
    {
        Player.instance.audio_action_firering.Play();
        int count_Coin = Random.Range(25,30);
        int count_Item = Random.Range(5, 11);
        Canvas_Player.instance.Coin(count_Coin);
        Canvas_Player.instance.Crystal(2);
        foreach (var item in Canvas_Player.instance.GetOrbTable(this).GetItem(count_Item))
        {
            Canvas_Player.instance.AddItem(item);
        }
    }
}

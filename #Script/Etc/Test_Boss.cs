using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test_Boss : MonoBehaviour
{
    private Animator anim;
    private CustomEffect customEffect;
    private void Start()
    {
        anim = GetComponent<Animator>();
        customEffect = GetComponentInChildren<CustomEffect>();
        customEffect.Setting(null);
    }

    public void Play(string tag)
    {
        customEffect.PlayParticle(tag);
    }
    

    [Button]
    public void Attack_Back()
    {
        anim.Play("Attack_Back");
    }
    [Button]
    public void Attack_Clap()
    {
        anim.Play("Attack_Clap");
    }
    [Button]
    public void Attack_Dash()
    {
        anim.Play("Attack_Dash");
    }
    [Button]
    public void Attack_Foot()
    {
        anim.Play("Attack_Foot");
    }
    [Button]
    public void Attack_Front_Combo()
    {
        anim.Play("Attack_Front_Combo");
    }
    [Button]
    public void Attack_Front_Normal()
    {
        anim.Play("Attack_Front_Normal");
    }
    [Button]
    public void Attack_Ground()
    {
        anim.Play("Attack_Ground");
    }
}

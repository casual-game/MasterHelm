using System.Collections;
using System.Collections.Generic;
using AmazingAssets.AdvancedDissolve;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;

public class Prop_Breakable : Prop
{
    protected Material _mat;
    private static float sound_wood_normal_time = -100, sound_wood_strong_time = -100;
    public override void Awake()
    {
        base.Awake();
        _mat = GetComponent<Renderer>().material;
    }

    public override void Interact_Normal(Vector3 forceVec)
    {
        base.Interact_Normal(forceVec);
        if (Time.unscaledTime - sound_wood_normal_time > 0.3f)
        {
            SoundManager.Play(SoundContainer_Ingame.instance.sound_interact_wood_normal);
            sound_wood_normal_time = Time.unscaledTime;
        }
    }

    public override void Interact_Strong(Vector3 forceVec)
    {
        base.Interact_Strong(forceVec);
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
            t.SetParent(null);
        }
        if (Time.unscaledTime - sound_wood_strong_time > 0.3f)
        {
            SoundManager.Play(SoundContainer_Ingame.instance.sound_interact_wood_strong);
            sound_wood_strong_time = Time.unscaledTime;
        }
        ParticleManager.Play(ParticleManager.instance.pd_break, transform.position,GameManager.Q_Identity);
        _rigidbody.AddForce(forceVec.normalized * 100);
        _rigidbody.AddTorque(Random.insideUnitSphere.normalized*125);
        _sequence.Complete();
        _sequence = Sequence.Create()
            .Chain(Tween.PunchScale(transform, new Vector3(0.65f, -0.6f, 0.65f) 
                                               * tweenStrength, 0.3f, 5));
        _sequence.Group(Tween.Custom(0, 1, 1.0f, onValueChange: val =>
        {
            AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(_mat,
                AdvancedDissolveProperties.Cutout.Standard.Property.Clip, val);
        },startDelay:0.25f));
        _sequence.ChainCallback(() => gameObject.SetActive(false));
    }
    
}

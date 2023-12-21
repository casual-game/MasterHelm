using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Prop : MonoBehaviour
{
    private Sequence sequence;
    private Rigidbody _rigidbody;
    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Interact(Vector3 forceVec)
    {
        _rigidbody.AddForce(forceVec.normalized*20);
        sequence.Complete();
        sequence = Sequence.Create();
        sequence.Chain(Tween.PunchScale(transform, new Vector3(0.65f, -0.6f, 0.65f), 0.3f, 5));
    }
}

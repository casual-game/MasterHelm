using System;
using System.Collections;
using System.Collections.Generic;
using AmazingAssets.AdvancedDissolve;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Prop : MonoBehaviour
{
    public float tweenStrength = 1.0f;
    protected Sequence _sequence;
    protected Rigidbody _rigidbody;
    private bool finished = false;
    private Collider _coll;
    private NavMeshObstacle _obstacle;
    
    public virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _obstacle = GetComponent<NavMeshObstacle>();
        _coll = GetComponent<Collider>();
    }

    public virtual void Interact(Vector3 forceVec,bool isStrong)
    {
        if (finished) return;
        if (isStrong) Interact_Strong(forceVec);
        else Interact_Normal(forceVec);
    }

    public virtual void Interact_Normal(Vector3 forceVec)
    {
        forceVec = new Vector3(forceVec.x * 0.5f, forceVec.y, forceVec.z * 0.5f);
        _rigidbody.AddForce(forceVec.normalized * 25);
        _sequence.Complete();
        _sequence = Sequence.Create()
            .Chain(Tween.PunchScale(transform, new Vector3(0.65f, -0.6f, 0.65f) 
                                               * tweenStrength, 0.3f, 5));
    }

    public virtual void Interact_Strong(Vector3 forceVec)
    {
        //_coll.enabled = false;
        _obstacle.enabled = false;
        finished = true;
    }
}

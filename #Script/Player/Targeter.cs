using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    public float enforcedTime = -100, enforcedDuration = 0.5f;

    private ParticleSystem particle_Renew,particle_Main;
    private Coroutine c_enforce = null;
    private Enemy targetedEnemy = null;
    
    
    public void Setting()
    {
        particle_Renew = transform.Find("Renew").GetComponent<ParticleSystem>();
        particle_Main = transform.Find("Main").GetComponent<ParticleSystem>();
    }
    public void Activate(bool _activate)
    {
        if (Player.instance.death) return;
        if (!gameObject.activeSelf) return;
        if (_activate)
        {
            particle_Main.Play();
            //particle_Renew.Play();
        }
        else
        {
            //particle_Renew.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            particle_Main.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        
    }

    public void Enforce(bool enforce)
    {
        if (enforce)
        {
            if (particle_Main.isPlaying && !particle_Renew.isPlaying) particle_Renew.Play();
            enforcedTime = Time.unscaledTime + enforcedDuration;
        }
        else
        {
            if(c_enforce!=null) StopCoroutine(C_Enforce());
            c_enforce = StartCoroutine(C_Enforce());
        }
    }

    public void Enforce_Renew()
    {
        if(particle_Renew.isPlaying)enforcedTime = Time.unscaledTime + enforcedDuration;
    }
    public void Enforce_StopEmmediately()
    {
        enforcedTime = -100;
    }

    public IEnumerator C_Enforce()
    {
        while (Time.unscaledTime < enforcedTime) yield return null;
        particle_Renew.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
    public bool IsEnforced()
    {
        return Time.unscaledTime < enforcedTime;
    }
    public void SetParent(Enemy target,float height)
    {
        targetedEnemy = target;
        if (target == null)
        {
            transform.SetParent(Manager_Main.instance._folder_);
            Activate(false);
        }
        else if(transform.parent != target)
        {
            transform.SetParent(target.transform);
            transform.localPosition = Vector3.up*height;
            Activate(true);
        }
    }
    
    
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Barricade : MonoBehaviour
{
    private static string s_open = "Open", s_close = "Close";
    public Data_Audio audio_Activate, audio_Impact;
    
    
    private bool opened = false;
    private Animator anim;
    private ParticleSystem p_flame, p_heat;
    private BoxCollider boxC;
    private bool passed = false;
    private int passedCheck = 0;
    private Vector3 _from, _to;
    private LayerMask playerLayer;
    [HideInInspector] public Transform pointT;
    public void Setting()
    {
        SoundManager.instance.Add(audio_Activate);
        SoundManager.instance.Add(audio_Impact);
        anim = GetComponent<Animator>();
        Transform pt = transform.Find("Particle");
        pointT = transform.Find("PointT");
        p_flame = pt.Find("Flame").GetComponent<ParticleSystem>();
        p_heat = pt.Find("Heat").GetComponent<ParticleSystem>();
        playerLayer = LayerMask.NameToLayer("Player");
        boxC = GetComponent<BoxCollider>();
        boxC.isTrigger = false;
        opened = false;
        passedCheck = 0;
        
        Transform t = transform;
        Vector3 center = boxC.center , scale = t.lossyScale;
        center.x *= scale.x;
        center.y *= scale.y;
        center.z *= scale.z;
        _from = t.position + t.rotation*center;
        _to = _from + t.forward*2;
    }

    public bool IsOpened()
    {
        return opened;
    }
    public void Open()
    {
        if (opened) return;
        opened = true;
        if(c_openclose!=null) StopCoroutine(c_openclose);
        anim.CrossFade(s_open,0.1f,0);
        audio_Activate.Play();
    }
    private void Close()
    {
        if (!opened) return;
        opened = false;
        boxC.isTrigger = false;
        if(c_openclose!=null) StopCoroutine(c_openclose);
        c_openclose = StartCoroutine(C_Close());
    }
    private float closeDelay = 0.5f;
    private Coroutine c_openclose = null;
    private IEnumerator C_Close()
    {
        Manager_Main.instance.Text_Info_Fin();
        Manager_Main.instance.Text_Specefic_Fin();
        yield return new WaitForSecondsRealtime(closeDelay);
        p_heat.Play();
        audio_Impact.Play();
        anim.CrossFade(s_close,0.1f,0);
    }
    
    public void Impact_Med()
    {
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_SpecialSmooth,true,false,true);
        p_flame.Play();
        audio_Impact.Play();
        if (opened)
        {
            p_heat.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            boxC.isTrigger = true;
            Manager_Main.instance.Text_Info("Move!",Manager_Main.instance.specefic_cleararea);
        }
    }
    public void Impact_Norm()
    {
        CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Smooth,true,false,true);
        //if(!opened) Manager_Main.instance.Spawner_AreaStart();
    }

    public bool Passed()
    {
        return passed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (opened && passedCheck==0 && other.gameObject.layer == playerLayer)
        {
            passedCheck = 1;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (opened && passedCheck==1 && other.gameObject.layer == playerLayer)
        {
            Vector3 correctVec = _to-_from;
            Vector3 currentVec = Player.instance.transform.position - _from;
            correctVec.y = 0;
            currentVec.y = 0;
            if (Vector3.Dot(correctVec,currentVec)>0)
            {
                passedCheck = 2;
                passed = true;
                Close(); 
            }
            
        }

       
    }
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        boxC = GetComponent<BoxCollider>();
        Transform t = transform;
        Vector3 center = boxC.center , scale = t.lossyScale;
        center.x *= scale.x;
        center.y *= scale.y;
        center.z *= scale.z;
        _from = t.position + t.rotation*center;
        _to = _from + t.forward*2;
        Debug.DrawLine(_from,_to);
    }
    #endif
}

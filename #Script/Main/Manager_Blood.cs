using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Blood : MonoBehaviour
{
    public static Manager_Blood instance;
    
    private int blood_Hit_Length = 3, blood_Smash_Length = 2, hit_index = 0, smash_index = 0;

    public IEnumerator Setting()
    {
        instance = this;
        yield return StartCoroutine(Manager_Pooler.instance.Add("Blood_Hit_0",6));    
        yield return StartCoroutine(Manager_Pooler.instance.Add("Blood_Hit_1",6));    
        yield return StartCoroutine(Manager_Pooler.instance.Add("Blood_Hit_2",6));    
        
        yield return StartCoroutine(Manager_Pooler.instance.Add("Blood_Smash_0",3));    
        yield return StartCoroutine(Manager_Pooler.instance.Add("Blood_Smash_1",3));
    }

    public GameObject Blood_Hit()
    {
        string tag = "Blood_Hit_" + hit_index;
        hit_index = (hit_index + 1) % blood_Hit_Length;
        return Manager_Pooler.instance.Get(tag);
    }
    public GameObject Blood_Smash()
    {
        string tag = "Blood_Smash_" + smash_index;
        smash_index = (smash_index + 1) % blood_Smash_Length;
        return Manager_Pooler.instance.Get(tag);
    }
}

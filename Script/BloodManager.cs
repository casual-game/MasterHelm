using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BloodManager : MonoBehaviour
{
    public static BloodManager instance;
    private Transform blood_strong_front;
    private Transform blood_strong_bottom;
    private Transform blood_norm;
    private int bsf_maxindex, bsb_maxindex, bn_maxindex;
    private int bsf_index, bsb_index, bn_index;
    private void Awake()
    {
        blood_strong_front = transform.GetChild(0);
        blood_strong_bottom = transform.GetChild(1);
        blood_norm = transform.GetChild(2);
        bsf_maxindex = blood_strong_front.childCount;
        bsb_maxindex = blood_strong_bottom.childCount;
        bn_maxindex = blood_norm.childCount;
        bsf_index = 0;
        bsb_index = 0;
        bn_index = 0;
        instance = this;
    }

    public void Blood_Normal(ref Vector3 pos,ref Quaternion rot)
    {
        return;
        bn_index = (bn_index + 1) % bn_maxindex;
        Transform t = blood_norm.GetChild(bn_index);
        t.gameObject.SetActive(false);
        t.gameObject.SetActive(true);
        t.SetPositionAndRotation(pos,rot);
    }

    public void Blood_Strong_Front(ref Vector3 pos,ref Quaternion rot)
    {
        return;
        bsf_index = (bsf_index + 1) % bsf_maxindex;
        Transform t = blood_strong_front.GetChild(bsf_index);
        t.gameObject.SetActive(false);
        t.gameObject.SetActive(true);
        t.SetPositionAndRotation(pos,rot);
    }
    public void Blood_Strong_Bottom(ref Vector3 pos,ref Quaternion rot)
    {
        return;
        bsb_index = (bsb_index + 1) % bsb_maxindex;
        Transform t = blood_strong_bottom.GetChild(bsb_index);
        t.gameObject.SetActive(false);
        t.gameObject.SetActive(true);
        t.SetPositionAndRotation(pos,rot);
    }
}

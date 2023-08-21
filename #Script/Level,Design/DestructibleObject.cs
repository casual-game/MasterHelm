using System.Collections;
using System.Collections.Generic;
using Dest.Math;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class DestructibleObject : MonoBehaviour
{
    public static List<DestructibleObject> destructibleObjects = new List<DestructibleObject>();
    public float radius;
    public float force = 0.3f,torque = 360;
    public Data_Audio breakSound;

    private bool exploded = false;
    private Transform before,after,core,particle;

    private Rigidbody[] rigids;
    private ParticleSystem[] particles;
    [HideInInspector]public Box3 box;
    public void Setting2 () 
    {
        SoundManager.instance.Add(breakSound);
        destructibleObjects.Add(this);
        before = transform.Find("Before");
        after = transform.Find("After");
        core = transform.Find("Core");
        particle = transform.Find("Particle");

        rigids = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rigids) rb.isKinematic = true;
        particles = particle.GetComponentsInChildren<ParticleSystem>();
        box = CreateBox3(core.transform);
        
        //활성화
        before.gameObject.SetActive(true);
        after.gameObject.SetActive(false);
        core.gameObject.SetActive(true);
        particle.gameObject.SetActive(true);

        
    }
    [Button]
    public void Explode(Vector3 explodePos,bool effect = true)
    {
        if (exploded) return;
        destructibleObjects.Remove(this);
        before.gameObject.SetActive(false);
        after.gameObject.SetActive(true);
        exploded = true;
        breakSound.Play();
        foreach (var par in particles) par.Play();
        //물리 폭발
        foreach (Rigidbody rb in rigids)
        {
            rb.isKinematic = false;
            Vector3 vec =((rb.transform.position - transform.position) 
                          +(rb.transform.position - Player.instance.transform.position)).normalized;
            rb.AddForce(vec*force,ForceMode.Impulse);
            rb.AddTorque(Quaternion.Euler(90,0,0)*vec*torque,ForceMode.Impulse);
        }
        //카메라 효과
        if (effect)
        {
            //CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Smooth);
        }
        StartCoroutine("C_Explode");
    }
    private Box3 CreateBox3(Transform box)
    {
        return new Box3(box.position, box.right, box.up, box.forward, box.lossyScale);
    }
    private IEnumerator C_Explode()
    {
        yield return new WaitForSeconds(5.0f);
        foreach (Rigidbody rb in rigids) rb.isKinematic = true;
    }
    
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Box3 _box = CreateBox3(transform.Find("Core"));
        DrawBox(_box);
    }
    private void DrawBox(Box3 box)
    {
        Vector3 v0, v1, v2, v3, v4, v5, v6, v7;
        box.CalcVertices(out v0, out v1, out v2, out v3, out v4, out v5, out v6, out v7);
        Gizmos.DrawLine(v0, v1);
        Gizmos.DrawLine(v1, v2);
        Gizmos.DrawLine(v2, v3);
        Gizmos.DrawLine(v3, v0);
        Gizmos.DrawLine(v4, v5);
        Gizmos.DrawLine(v5, v6);
        Gizmos.DrawLine(v6, v7);
        Gizmos.DrawLine(v7, v4);
        Gizmos.DrawLine(v0, v4);
        Gizmos.DrawLine(v1, v5);
        Gizmos.DrawLine(v2, v6);
        Gizmos.DrawLine(v3, v7);
    }
    #endif
}

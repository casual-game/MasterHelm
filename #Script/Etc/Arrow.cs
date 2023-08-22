using System;
using System.Collections;
using System.Collections.Generic;
using Dest.Math;
using Dest.Math.Tests;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    [ReadOnly] public bool activated = false;
    private ParticleSystem arrow,trail;
    private Transform start, fin,boxT;
    private Vector3 targetPos,hitLocalPos;
    private float speed;
    private Enemy target;
    private string c_explision_fire = "Explosion_Fire", c_spark = "Spark";
    void Start()
    {
       Setting(); 
    }

    public void Setting()
    {
	    arrow = transform.Find("Arrow").GetComponent<ParticleSystem>();
	    trail = transform.Find("Trail").GetComponent<ParticleSystem>();
	    start = transform.Find("Start");
	    fin = transform.Find("Fin");
	    boxT = transform.Find("Box");
	    
	    activated = false;
    }

    
    private IEnumerator C_Arrow()
    {
	    yield return new WaitForSeconds(1.0f);
	    arrow.Pause();
    }

    public void Cancel()
    {
	    StopCoroutine("C_Arrow");
	    activated = false;
	    arrow.Stop();
	    arrow.gameObject.SetActive(false);
	    arrow.gameObject.SetActive(true);
	    trail.Stop();
	    trail.transform.localPosition = start.transform.localPosition;
    }

    [Button]
    public void Charge()
    {
	    activated = true;
	    arrow.Play();
	    trail.transform.localPosition = start.transform.localPosition;
	    trail.Play();
	    StartCoroutine("C_Arrow");
    }
    [Button]
    public void Shoot(Vector3 _targetPos,float _speed,Enemy _target)
    {
	    activated = true;
	    targetPos = _targetPos;
	    speed = _speed;
	    target = _target;
	    trail.transform.localPosition = fin.transform.localPosition;
	    StartCoroutine("C_Shoot");
    }

    public void DestroyArrow(Vector3 effectPos)
    {
	    Manager_Pooler.instance.GetParticle(c_explision_fire,effectPos,Player.instance.transform.rotation);
	    Manager_Pooler.instance.GetParticle(c_spark,effectPos,Player.instance.transform.rotation);
	    effectPos.y = Player.instance.transform.position.y + 1;
	    Cancel();
    }
    private IEnumerator C_Shoot()
    {
	    bool isExecute = target != null;
	    Vector3 dist = start.position-transform.position;
	    Vector3 startPos = transform.position;
	    targetPos -= dist;
	    transform.rotation = Quaternion.LookRotation(targetPos - startPos);
	    float startTime = Time.time;
	    float duration = Vector3.Distance(startPos, targetPos) / speed;
	    float ratio = 0;
	    yield return null;
	    Vector3 currentPos = transform.position;
	    /*
	    foreach (var sparkable in Sparkable.Sparkables)
	    {
		    if (Vector3.SqrMagnitude(currentPos - sparkable.transform.position) < 400)
		    {
			    boxes.Add(sparkable.box);
		    }
	    }
	    */
	    //목표까지 날아가기
	    while (ratio<1)
	    {
		    yield return null;
		    ratio = (Time.time - startTime)/duration;
		    transform.position = Vector3.Lerp(startPos,targetPos,ratio);
		    /*
		    //destructible
		    int jlength = DestructibleObject.destructibleObjects.Count;
		    for (int j = jlength-1; j >= 0; j--)
		    {
			    DestructibleObject destructible = DestructibleObject.destructibleObjects[j];
			    Vector3 distVec = transform.position-destructible.transform.GetChild(0).position;
			    distVec.y = 0;
			    if (Vector3.Magnitude(distVec) < 0.1f+destructible.radius)
			    {
				    destructible.Explode(transform.position);
				    DestroyArrow(destructible.transform.GetChild(0).position);
				    yield break;
			    }
		    }
		    //collision
		    foreach (var _box in boxes)
		    {
			    Box3 targetBox = _box,thisBox = CreateBox3(boxT);
			    if (Intersection.TestBox3Box3(ref targetBox, ref thisBox))
			    {
				    DestroyArrow(start.position);
				    yield break;
			    }
		    }
		    */
	    }
		
	    transform.position = targetPos;
	    yield return null;
	    if (target != null && !target.death)
	    {
		    float skillPoint = 20;
		    Canvas_Player.instance.skillGauge_L.SetValue(Canvas_Player.instance.skillGauge_L.current + skillPoint);
		    Canvas_Player.instance.skillGauge_R.SetValue(Canvas_Player.instance.skillGauge_R.current + skillPoint);
		    Vector3 hitPos = target.transform.position+(Player.instance.transform.position-target.transform.position).normalized*0.5f;
		    hitPos.y = transform.position.y;
		    
		    //CamArm.instance.Impact(Manager_Main.instance.mainData.impact_Hit);
		    Player.instance.isStrong = false;
		    target.Hit(false,Player.instance.transform.rotation.eulerAngles,null,true);
		    
		    DestroyArrow(hitPos);
	    }
	    else
	    {
		    Cancel();
	    }
    }

    #region Collision
    private Box3 box;
    private List<Box3> boxes = new List<Box3>();
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
    private Box3 CreateBox3(Transform box)
    {
        return new Box3(box.position, box.right, box.up, box.forward, box.lossyScale);
    }
    #endregion

    private void OnDrawGizmos()
    {
	    DrawBox(CreateBox3(transform.Find("Box")));
	    void DrawBox(Box3 box)
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
    }
}

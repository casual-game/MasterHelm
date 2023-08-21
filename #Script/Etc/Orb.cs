using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    private ParticleSystem glow, explode;
    private Vector3 startPos;
    private Vector3 startMoveVec;
    private float startTime;
    private float moveDuration;
    private bool death = false;
    private RFX4_TrailRenderer[] trails;

    [ColorUsage(true, true)] public Color playerColor;
    public float startMoveSpeed = 3.0f;
    public float moveSpeed = 1.0f;
    public float maxDuration = 3.5f;
    public float height = 1.0f;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0,0,1,1);
    public AnimationCurve heightCurve;
    public void OnEnable()
    {
        EnabledEvent();
        Manager_Main.instance.mainData.Orb_Create.Play();
        transform.rotation = Quaternion.Euler(-90,0,0);
        death = false;
        glow = transform.Find("Glow").GetComponent<ParticleSystem>();
        explode = transform.Find("Explode").GetComponent<ParticleSystem>();
        trails = GetComponentsInChildren<RFX4_TrailRenderer>();

        startPos = transform.position;
        startMoveVec = transform.position - Player.instance.transform.position;
        startMoveVec.y = 0;
        startMoveVec.Normalize();
        startTime = Time.time;
        
        moveDuration = Vector3.Distance(transform.position, Player.instance.transform.position) * (1.0f/moveSpeed);
        moveDuration = Mathf.Clamp(moveDuration, 0, maxDuration);
        explode.Play();
        foreach (var trail in trails)
        {
            trail.currentLifeTime = 0;
            trail.TrailLifeTime = moveDuration;
        }
    }

    public virtual void EnabledEvent()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (death) return;

        
        if (startMoveSpeed > 0.01f)
        {
            startMoveSpeed -= 3 * Time.deltaTime;
            startPos += startMoveVec * startMoveSpeed*Time.deltaTime;
        }
        
        float ratio = Mathf.Clamp01((Time.time - startTime) / moveDuration);
        float moveRatio = moveCurve.Evaluate(ratio);
        float height = heightCurve.Evaluate(ratio)*this.height;
        Vector3 movePos = Vector3.Lerp(startPos,Player.instance.transform.position+Vector3.up*0.8f,moveRatio);
        movePos.y += height;
        transform.position = movePos;

        if (moveRatio > 0.99f) StartCoroutine("Death");
    }

    IEnumerator Death()
    {
        death = true;
        explode.Play();
        glow.Stop();
        Manager_Main.instance.mainData.Orb_Remove.Play();
        Player.instance.highlight.HitFX(playerColor,1.25f);
        Reward();
        yield return new WaitForSeconds(2.5f);
        gameObject.SetActive(false);
    }

    public virtual void Reward()
    {
        
    }
}

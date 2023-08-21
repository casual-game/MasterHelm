using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Prefab_Bow : MonoBehaviour
{
    public Transform bowString;
    public Arrow arrow;
    public Color color;
    [Range(1,5)]public int multiShot = 1;
    public float speed = 5.0f;
    private Queue<Arrow> arrows;
    private Arrow currentArrow=null;
    private Vector3 bowStringPos;
    private Transform holdTransform;
    private Transform parentTransform;
    private Transform pullHandTransform;
    private Animator animator;
    private bool pulling = false;
    public void Setting(Transform holdHand,Transform pullHand)
    {
        pullHandTransform = pullHand;
        holdTransform = transform.Find("Hold");
        parentTransform = holdHand;
        animator = GetComponent<Animator>();
        animator.SetBool("Hold",false);
        bowStringPos = bowString.transform.localPosition;
        On(false);
        //화살 pool에 추가
        arrows = new Queue<Arrow>();
        for (int i = 0; i < multiShot*2; i++)
        {
            Arrow a = Instantiate(arrow);
            a.transform.SetParent(Manager_Main.instance._folder_);
            a.Setting();
            arrows.Enqueue(a);
        }
    }
    public void SetHold(bool _hold)
    {
        animator.SetBool("Hold",_hold);
    }
    public void SetPulling(bool _pulling)
    {
        pulling = _pulling;
        if (_pulling)
        {
            currentArrow = GetArrow();
            UpdateArrow();
            currentArrow.Charge();
        }
        else
        {
            Vector3 targetPos;
            if (Player.instance.target != null)
            {
                targetPos =Player.instance.target.transform.position;
                targetPos.y = currentArrow.transform.position.y;
                currentArrow.Shoot(targetPos,speed,Player.instance.target);
                
            }
            else
            {
                if (Canvas_Player.RS_Scale > 0.1f)
                {
                    targetPos = Player.instance.image_pointer.transform.position-Player.instance.transform.position;
                    targetPos = Player.instance.transform.position+targetPos.normalized * 20;
                }
                else targetPos = Player.instance.transform.position+Player.instance.transform.forward * 20; 
                targetPos.y = currentArrow.transform.position.y;
                currentArrow.Shoot(targetPos,speed,null);
            }
        }
    }

    private void UpdateArrow()
    {
        if (currentArrow != null)
        {
            currentArrow.transform.position = bowString.position;
            Vector3 lookVec = Player.instance.transform.forward;
            currentArrow.transform.rotation = Quaternion.LookRotation(lookVec);
        }
    }
    public void Cancel()
    {
        if (currentArrow != null)
        {
            currentArrow.Cancel();
            currentArrow = null;
        }
    }
    public void Update()
    {
        if (pulling)
        {
            bowString.position = Vector3.Lerp(bowString.position,pullHandTransform.position,10*Time.deltaTime);
        }
        else
        {
            bowString.localPosition = Vector3.Lerp(bowString.localPosition,bowStringPos,10*Time.deltaTime);
        }

        UpdateArrow();
    }
    public void On(bool activate)
    {
        if (activate)
        {
            transform.SetParent(parentTransform);
            transform.localPosition = holdTransform.localPosition;
            transform.localRotation = holdTransform.localRotation;
            transform.localScale = holdTransform.localScale;
            animator.SetBool("On",activate);
        }
        else
        {
            transform.SetParent(Manager_Main.instance._folder_);
            animator.SetBool("On",activate);
        }
    }

    public Arrow GetArrow()
    {
        Arrow a = null;
        for (int i = 0; i < arrows.Count; i++)
        {
            a = arrows.Dequeue();
            if (a.activated)
            {
                arrows.Enqueue(a);
                a = null;
            }
        }

        if (a == null)
        {
            a = Instantiate(arrow);
            a.transform.SetParent(Manager_Main.instance._folder_);
            a.Setting();
            arrows.Enqueue(a);
            return a;
        }
        else return a;
    }
}

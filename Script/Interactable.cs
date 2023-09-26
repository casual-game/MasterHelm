using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public static Interactable currentInteractable = null;
    public bool isInteracting = false;
    public virtual void Interact()
    {
        if (currentInteractable == this && !isInteracting)
        {
            #if UNITY_EDITOR
            print("Interact: " + gameObject.name);
            #endif
            
            //currentInteractable = null;
            isInteracting = true;
        }
    }
}

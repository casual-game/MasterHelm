using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_DragData : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler,IPointerClickHandler
{
    public SelectUI target;
    public void OnPointerDown(PointerEventData data)
    {
        target.E_PointerDown(data);
    }
    public void OnDrag(PointerEventData data)
    {
        target.E_Drag(data);
    }
    public void OnPointerUp(PointerEventData data)
    {
        target.E_PointerUp(data);
    }
    public void OnPointerClick(PointerEventData data)
    {
        target.E_PointerClick(data);
    }
}

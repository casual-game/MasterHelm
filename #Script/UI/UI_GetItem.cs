using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class UI_GetItem : MonoBehaviour
{
    private int index = 0;
    private Dictionary<int, UI_GetItem_Slot> slotsDic = new Dictionary<int, UI_GetItem_Slot>();
    private Dictionary<int, Vector2> slotsPos = new Dictionary<int, Vector2>();
    private Coroutine c_moveitems = null;
    public float moveDuration = 0.5f;
    public float delay = 0.25f;
    public List<Data_Item> items = new List<Data_Item>();
    public void Setting()
    {
        slotsDic.Add(0,transform.GetChild(0).GetComponent<UI_GetItem_Slot>());
        slotsDic.Add(1,transform.GetChild(1).GetComponent<UI_GetItem_Slot>());
        slotsDic.Add(2,transform.GetChild(2).GetComponent<UI_GetItem_Slot>());
        Vector2 tempFirstPos = transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition;
        Vector2 tempCreatePos = tempFirstPos + Vector2.right*50;
        Vector2 tempDestroyPos = transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition + Vector2.right*50;
        foreach (var slot in slotsDic.Values)
        {
           slot.Setting(tempFirstPos,tempCreatePos,tempDestroyPos,moveDuration); 
        }
        slotsPos.Add(0,slotsDic[0].rectT.anchoredPosition);
        slotsPos.Add(1,slotsDic[1].rectT.anchoredPosition);
        slotsPos.Add(2,slotsDic[2].rectT.anchoredPosition);
    }
    public void AddItem(params Data_Item[] items)
    {
        this.items.AddRange(items);
        if(c_moveitems != null) StopCoroutine(c_moveitems);
        c_moveitems = StartCoroutine(C_MoveItems());
    }
    private void GetItem(Data_Item item)
    {
        int nextIndex = (index + 1) % 3;
        for (int i = 0; i < 3; i++)
        {
            int currentSlotIndex = (index+i) % 3;
            int nextSlotIndex = (nextIndex + i) % 3;
            slotsDic[i].Move(slotsPos[currentSlotIndex],slotsPos[nextSlotIndex],nextSlotIndex,item);
        }

        index = nextIndex;
    }

    private IEnumerator C_MoveItems()
    {
        while (items.Count > 0)
        {
            while (true)
            {
                bool check = true;
                foreach (var slot in slotsDic.Values)
                {
                    if (!slot.finished)
                    {
                        check = false;
                        break;
                    }
                }
                if(check) break;
                yield return null;
            }
            GetItem(items[0]);
            items.RemoveAt(0);
            yield return new WaitForSecondsRealtime(delay);
        }
    }
}

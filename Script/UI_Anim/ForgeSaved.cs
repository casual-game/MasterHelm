using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class ForgeSaved : MonoBehaviour
{
    
    public List<RectTransform> saveRects = new List<RectTransform>();
    public RectTransform saveFrame,title;
    public List<CanvasGroup> cgList = new List<CanvasGroup>();
    public Sequence seqFS;
    public List<ForgeSavedSlot> slots = new List<ForgeSavedSlot>();
    
    [HideInInspector] public bool forgeOpened = false;
    public void Setting()
    {
        //FitUI
        int blocksCount = 4;
        float spaceRatio = 3*Mathf.Clamp01(0.6f-(saveFrame.rect.x/saveFrame.rect.y));
        float widthSpace = 32;
        float height = saveFrame.rect.height*1.0f / (blocksCount + (blocksCount - 1) * spaceRatio);
        float width = saveFrame.rect.width - widthSpace * 2;
        var rect = saveFrame.rect;
        
        for(int i=0; i<saveRects.Count; i++)
        {
            saveRects[i].anchoredPosition = rect.position + new Vector2(widthSpace + width*0.5f, 
                (height + height * spaceRatio)*i + height*0.5f);
            saveRects[i].sizeDelta = new Vector2(width, height);

            float slotRadius = Mathf.Min(width, height) - 64;
            if (i < 3)
            {
                int index = -1;
                foreach (Transform t in saveRects[i].transform)
                {
                    var rt = t.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2(slotRadius, slotRadius);
                    rt.anchoredPosition = new Vector2(width*0.175f*index,0);
                    index+=2;
                }
            }
        }

        title.position = saveRects[3].position;
        title.anchoredPosition = new Vector2(0, title.anchoredPosition.y);
        title.sizeDelta = saveRects[3].sizeDelta;
        
        UpdateData();
        gameObject.SetActive(false);
    }

    public void UpdateData()
    {
        for (int i = 0; i < SaveManager.instance.forgeWeaponDatas.Count; i++)
        {
            Item_Weapon weapon = SaveManager.instance.GetWeapon(SaveManager.instance.forgeWeaponDatas[i]);
            slots[i].SetItem(weapon);
            slots[i].Deselected();
        }

        for (int i = SaveManager.instance.forgeWeaponDatas.Count; i < 6; i++)
        {
            slots[i].SetItem(null);
            slots[i].Deselected();
        }
    }

    public void Deselect()
    {
        foreach (var slot in slots) slot.Deselected();
    }
    public void Show(float delay)
    {
        if (forgeOpened) return;
        gameObject.SetActive(true);
        forgeOpened = true;
        seqFS.Stop();
        seqFS = Sequence.Create();
        delay += 0.05f;
        for(int i=0; i<cgList.Count; i++)
        {
            CanvasGroup cg = cgList[i];
            cg.alpha = 0;
            Transform t = cg.transform;
            t.localScale = Vector3.one*0.8f;
            delay += i * 0.05f;
            seqFS.Group(Tween.Scale(t, 1, 0.375f, startDelay: delay,ease:Ease.OutCubic));
            seqFS.Group(Tween.Alpha(cg, 1, 0.2f, startDelay: delay));
        }
    }
    public void Hide()
    {
        if (!forgeOpened) return;
        forgeOpened = false;
        seqFS.Stop();
        seqFS = Sequence.Create();
        for(int i=0; i<cgList.Count; i++)
        {
            CanvasGroup cg = cgList[i];
            cg.alpha = 1;
            Transform t = cg.transform;
            t.localScale = Vector3.one*1.0f;
            float delay = i * 0.05f;
            seqFS.Group(Tween.Scale(t, 0.9f, 0.375f, startDelay: delay,ease:Ease.OutCubic));
            seqFS.Group(Tween.Alpha(cg, 0, 0.2f, startDelay: delay));
        }

        seqFS.OnComplete(() => gameObject.SetActive(false));
    }
}

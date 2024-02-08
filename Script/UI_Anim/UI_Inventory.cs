using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    public RectTransform rectT;

    public List<Image> _slotImages = new List<Image>();
    public List<CanvasGroup> _slotCanvasGroups = new List<CanvasGroup>();
    private Sequence _seqInventory;
    public void Setting()
    {
        
        gameObject.SetActive(false);
    }
    
    [Button]
    public void FitUI()
    {
        var rect = rectT.rect;
        float a, b, x, width = rect.width, height = rect.height;
        x = Mathf.Min(width / 3.0f, height / 4.0f) * 0.75f;
        a = (width - 3 * x) * 0.25f;
        b = (height - 4 * x) * 0.2f;
        
        for (int w = 0; w < 3; w++)
        {
            for (int h = 0; h < 4; h++)
            {
                RectTransform rt = transform.GetChild(3 * h + w).GetComponent<RectTransform>();
                rt.anchoredPosition= new Vector2((w+1)*a + w*x + 0.5f*x,(h+1)*b + h*x - height + 0.5f*x);
                rt.sizeDelta = Vector2.one*x;
            }
        }
    }
    public void Open(float duration = 0.15f,float delay = 0.05f)
    {
        _seqInventory.Stop();
        foreach (var slot in _slotImages) slot.transform.localScale = Vector3.one*0.75f;
        foreach (var slot in _slotImages) slot.color = Color.clear;
        foreach (var slot in _slotCanvasGroups) slot.alpha = 0;
        gameObject.SetActive(true);
        
        _seqInventory = Sequence.Create();
        float startDelay = 0;
        for (int h = 3; h >= 0; h--)
        {
            for (int w = 0; w < 3; w++)
            {
                _seqInventory.Group(Tween.Scale(_slotImages[h * 3 + w].transform, 1, 
                    duration, Ease.OutCubic,startDelay: startDelay));
                _seqInventory.Group(Tween.Color(_slotImages[h * 3 + w],Color.white,
                    duration, Ease.OutSine,startDelay: startDelay));
                _seqInventory.Group(Tween.Alpha(_slotCanvasGroups[h * 3 + w],1,
                    duration, Ease.OutCubic,startDelay: startDelay));
            }

            startDelay += delay;
        }
    }
    [Button]
    public void Reroll()
    {
        Close(0.15f,0.05f,true);
        float startDelay = 0.3f;
        for (int h = 3; h >= 0; h--)
        {
            for (int w = 0; w < 3; w++)
            {
                _seqInventory.Group(Tween.Scale(_slotImages[h * 3 + w].transform, 1, 
                    0.15f, Ease.OutCubic,startDelay: startDelay));
                _seqInventory.Group(Tween.Color(_slotImages[h * 3 + w],Color.white,
                    0.15f, Ease.OutSine,startDelay: startDelay));
                _seqInventory.Group(Tween.Alpha(_slotCanvasGroups[h * 3 + w],1,
                    0.15f, Ease.OutCubic,startDelay: startDelay));
            }
            startDelay += 0.05f;
        }
    }
    [Button]
    public void Page()
    {
        _seqInventory.Stop();
        transform.localScale = Vector3.one;

        _seqInventory = Sequence.Create();
        Tween.PunchScale(transform, Vector3.down*0.15f, 0.15f, 1);
    }
    public void Close(float duration = 0.15f,float delay = 0.05f,bool activate = false)
    {
        _seqInventory.Stop();
        foreach (var slot in _slotImages) slot.transform.localScale = Vector3.one;
        foreach (var slot in _slotImages) slot.color = Color.white;
        foreach (var slot in _slotCanvasGroups) slot.alpha = 1;
        
        
        _seqInventory = Sequence.Create();
        float startDelay = 0;
        for (int h = 3; h >= 0; h--)
        {
            for (int w = 0; w < 3; w++)
            {
                _seqInventory.Group(Tween.Scale(_slotImages[h * 3 + w].transform, 0.75f, 
                    duration, Ease.InCubic,startDelay: startDelay));
                _seqInventory.Group(Tween.Color(_slotImages[h * 3 + w],Color.clear,
                    duration, Ease.InSine,startDelay: startDelay));
                _seqInventory.Group(Tween.Alpha(_slotCanvasGroups[h * 3 + w],0,
                    duration, Ease.InCubic,startDelay: startDelay));
            }

            startDelay += delay;
        }

        if(!activate) _seqInventory.OnComplete(() => gameObject.SetActive(false));
    }
}

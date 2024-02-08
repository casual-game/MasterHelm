using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public SoundData sound_selected;
    public float soundDelay;
    public List<ShopButton> anotherButtons = new List<ShopButton>();
    public Image img;
    public Color cSelected, cDeselected;
    public UnityEvent onSelected;
    
    private Tween _tMain;
    private bool _selected = false;
    
    public void Selected()
    {
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_click);
        if (_selected) return;
        SoundManager.Play(sound_selected,soundDelay);
        _selected = true;
        onSelected.Invoke();
        foreach (var btn in anotherButtons) btn.Deselected();
        img.color = cSelected;
        transform.localScale = Vector3.one * 1.5f;
        
        _tMain.Stop();
        _tMain = Tween.PunchScale(transform, Vector3.one * -0.25f, 0.25f, 2);
    }

    public void Deselected()
    {
        _selected = false;
        img.color = cDeselected;
        transform.localScale = Vector3.one * 1.5f;
    }
}

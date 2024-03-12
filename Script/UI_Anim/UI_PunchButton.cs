using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_PunchButton : MonoBehaviour,IPointerClickHandler
{
    public UnityEvent onSelected;
    public bool usePunch = true;
    public List<Graphic> graphics;
    public float scale;
    private Tween _tMain;
    private bool _selected,_usable;
    private List<Color> _colors;

    public void Setting()
    {
        _selected = false;
        _usable = true;
        _colors = new List<Color>();
        foreach (var graphic in graphics) _colors.Add(graphic.color);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!_usable) return;
        //SoundManager.Play(SoundContainer_StageSelect.instance.sound_click);
        if (usePunch)
        {
            _tMain.Stop();
            transform.localScale = Vector3.one * scale;
            _tMain = Tween.PunchScale(transform, Vector3.one *scale* -0.2f, 0.2f, 2,useUnscaledTime:true);   
        }
        onSelected?.Invoke();
    }
    [Button]
    public void Usable(bool usable)
    {
        _usable = usable;
        if (usable) for (int i = 0; i < graphics.Count; i++) graphics[i].color = _colors[i];
        else for (int i = 0; i < graphics.Count; i++) graphics[i].color = _colors[i]*0.25f;
    }
}

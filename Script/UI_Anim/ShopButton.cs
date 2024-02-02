using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public List<ShopButton> anotherButtons = new List<ShopButton>();
    public Image img;
    public Color cSelected, cDeselected;

    private Tween _tMain;
    public void Selected()
    {
        foreach (var btn in anotherButtons) btn.Deselected();
        img.color = cSelected;
        transform.localScale = Vector3.one * 1.5f;
        
        _tMain.Stop();
        _tMain = Tween.PunchScale(transform, Vector3.one * -0.25f, 0.25f, 2);
    }

    public void Deselected()
    {
        img.color = cDeselected;
        transform.localScale = Vector3.one * 1.5f;
    }
}

using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_IngameEarnableItem : MonoBehaviour
{
    public List<UI_EarnableSlot> slots = new List<UI_EarnableSlot>();
    public CanvasGroup cgPopup;
    public TMP_Text tmpPopup;
    public RectTransform rtPopup;
    public List<ContentSizeFitter> fitters = new List<ContentSizeFitter>();
    
    private StageData _stageData;
    private Sequence _seqPopup;
    public void Setting(StageData stageData)
    {
        _stageData = stageData;
        for (int i = 0; i < 9; i++)
        {
            var item = _stageData.GetItem(i);
            if(item.weapon!=null) slots[i].UpdateData(item.weapon);
            else slots[i].UpdateData(item.resource);
            
            slots[i].Setting(this,item);
        }
    }
    public void Activate()
    {
        gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
    public void Popup(string itemName,Vector3 pos)
    {
        cgPopup.gameObject.SetActive(true);
        _seqPopup.Stop();
        rtPopup.position = pos;
        tmpPopup.text = itemName;
        cgPopup.alpha = 0;
        cgPopup.transform.localScale = Vector3.one*0.8f;
        foreach (var fitter in fitters) fitter.SetLayoutHorizontal();
        
        
        
        _seqPopup = Sequence.Create(useUnscaledTime: true,cycleMode: CycleMode.Yoyo, cycles:2);
        _seqPopup.Group(Tween.Alpha(cgPopup, 1, 0.1f));
        _seqPopup.Group(Tween.Scale(cgPopup.transform, 1, 0.15f, Ease.OutCubic));
        _seqPopup.ChainDelay(0.4f);
        _seqPopup.OnComplete(() => cgPopup.gameObject.SetActive(false));
    }
    public void DeselectAll()
    {
        foreach (var slot in slots) slot.Deselected();
    }
}

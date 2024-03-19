using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_EarnableSlot : MonoBehaviour,IPointerClickHandler
{
    public Image icon;
    public GameObject locked;
    public CanvasGroup cgSelectedFrame;
    public RectTransform rectTransform;
    
    private Item_Weapon _itemWeapon;
    private Item_Resource _itemResource;
    private UI_IngameEarnableItem _ingameEarnableItem;
    private EarnableItem _earnableItem;
    private Tween tPunch, tSelected;
    public void UpdateData(Item_Weapon weapon)
    {
        if (weapon == null)
        {
            icon.gameObject.SetActive(false);
            locked.gameObject.SetActive(true);
            return;
        }
        
        icon.gameObject.SetActive(true);
        locked.gameObject.SetActive(false);
        _itemResource = null;
        _itemWeapon = weapon;
        icon.enabled = true;
        icon.sprite = weapon.icon;
        icon.rectTransform.offsetMin = new Vector2(weapon.left, weapon.bottom);
        icon.rectTransform.offsetMax = new Vector2(-weapon.right, -weapon.top);
        icon.rectTransform.localScale = weapon.scale;
    }
    public void UpdateData(Item_Resource resource)
    {
        if (resource == null)
        {
            icon.gameObject.SetActive(false);
            locked.gameObject.SetActive(true);
            return;
        }
        
        icon.gameObject.SetActive(true);
        locked.gameObject.SetActive(false);
        _itemWeapon = null;
        _itemResource = resource;
        icon.enabled = true;
        icon.sprite = resource.icon;
        icon.rectTransform.offsetMin = new Vector2(resource.left, resource.bottom);
        icon.rectTransform.offsetMax = new Vector2(-resource.right, -resource.top);
        icon.rectTransform.localScale = resource.scale;
    }
    public void Selected()
    {
        if(_itemWeapon != null) _ingameEarnableItem.Popup(_itemWeapon.title,transform.position);
        else if (_itemResource != null) _ingameEarnableItem.Popup(_itemResource.title,transform.position);
        else return;
        
        _ingameEarnableItem.DeselectAll();
        cgSelectedFrame.gameObject.SetActive(true);
        tSelected = Tween.Alpha(cgSelectedFrame,1,0.375f,useUnscaledTime:true);
    }
    public void Deselected()
    {
        tSelected.Stop();
        cgSelectedFrame.alpha = 0;
        cgSelectedFrame.gameObject.SetActive(false);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Play(SoundContainer_Ingame.instance.sound_stage_click);
        tPunch.Stop();
        transform.localScale = Vector3.one * 0.75f;
        tPunch = Tween.PunchScale(transform, Vector3.one *-0.25f, 0.15f, 2,useUnscaledTime:true);
        Selected();
    }
    public void Setting(UI_IngameEarnableItem ingameEarnableItem,EarnableItem earnableItem)
    {
        _ingameEarnableItem = ingameEarnableItem;
        _earnableItem = earnableItem;
    }
}

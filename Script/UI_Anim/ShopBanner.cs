using System;
using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using Febucci.UI.Core;
using PrimeTween;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ShopBanner : MonoBehaviour
{
    public RectTransform rt;
    public TMP_Text tmpTitle,tmpPrice;
    public ContentSizeFitter fitterPrice;
    public TypewriterCore twTitle;
    public CanvasGroup cgPrice;
    public CanvasGroup cgMain;
    public bool isShowed = false;
    public List<ShopBannerSingle> banners = new List<ShopBannerSingle>();
    public ShopBannerSingle bannerPackage;
    public Sprite starResourceNorm, starResourceSpecial;
    public GameObject gIconCoin, gIconGem,gKrw;
    [VerticalGroup("Debug")] public Item_Weapon debugWeapon;
    [VerticalGroup("Debug")] public Item_Resource debugResource;
    [VerticalGroup("Debug")] public Item_ShopPackage debugPackage;
    
    private float _bottom;
    private Sequence _seq,_seqShow,_seqIcon;
    private Item_Weapon _currentWeapon;
    private Item_Resource _currentResource;
    private Item_ShopPackage _currentPackage;
    private enum PriceType
    {
        coin=0,gem=1,money=2
    }
    private PriceType _priceType;
    private int itemPrice;
    private string itemTitle;
    
    public void Setting()
    {
        
        tmpTitle.text = string.Empty;
        tmpPrice.text = string.Empty;
        _bottom = rt.rect.size.y*0.375f;
        
        if(debugWeapon!=null) PresetWeapon(debugWeapon);
        else if(debugResource!=null) PresetResource(debugResource);
        PresetPackage(debugPackage);
    }
    public void Open(float delay)
    {
        _seq.Stop();
        gameObject.SetActive(true);
        if(twTitle.isHidingText)twTitle.StopDisappearingText();
        if(twTitle.isShowingText)twTitle.StopShowingText();
        tmpTitle.text = string.Empty;
        tmpPrice.text = itemPrice.ToString();
        switch (_priceType)
        {
            case PriceType.coin:
                gKrw.SetActive(false);
                gIconCoin.SetActive(true);
                gIconGem.SetActive(false);
                break;
            case PriceType.gem:
                gKrw.SetActive(false);
                gIconCoin.SetActive(false);
                gIconGem.SetActive(true);
                break;
            case PriceType.money:
                gKrw.SetActive(true);
                gIconCoin.SetActive(false);
                gIconGem.SetActive(false);
                break;
        }
        transform.localScale = Vector3.one;
        cgPrice.transform.localScale = Vector3.one*0.75f;
        cgPrice.alpha = 0;
        tmpPrice.text = itemPrice.ToString();
        fitterPrice.SetLayoutHorizontal();
        rt.offsetMin= new Vector2(rt.offsetMin.x,_bottom);
        
        _seq = Sequence.Create();
        if (gameObject.activeSelf) _seq.Group(Tween.Delay(delay, () => twTitle.ShowText(itemTitle)));
        else tmpTitle.text = itemTitle;
        _seq.Group(Tween.UIOffsetMinY(rt, 0, 0.75f, Ease.InOutBack,startDelay:delay));
        _seq.Group(Tween.Scale(cgPrice.transform, 1.0f, 0.5f, Ease.OutBack, startDelay: delay + 0.25f));
        _seq.Group(Tween.Alpha(cgPrice, 1.0f, 0.25f, startDelay: delay + 0.25f));
        Show();
    }
    public void Change(float delay)
    {
        if (!gameObject.activeSelf)
        {
            Open(0);
            return;
        }
        _seq.Stop();
        tmpTitle.text = string.Empty;
        transform.localScale = Vector3.one;
        cgPrice.transform.localScale = Vector3.one;
        tmpPrice.text = itemPrice.ToString();
        switch (_priceType)
        {
            case PriceType.coin:
                gKrw.SetActive(false);
                gIconCoin.SetActive(true);
                gIconGem.SetActive(false);
                break;
            case PriceType.gem:
                gKrw.SetActive(false);
                gIconCoin.SetActive(false);
                gIconGem.SetActive(true);
                break;
            case PriceType.money:
                gKrw.SetActive(true);
                gIconCoin.SetActive(false);
                gIconGem.SetActive(false);
                break;
        }
        fitterPrice.SetLayoutHorizontal();
        
        _seq = Sequence.Create();
        _seq.Group(Tween.PunchScale(transform, Vector3.up * -0.05f, 0.375f, 2, startDelay: delay));
        if (gameObject.activeSelf) _seq.Group(Tween.Delay(delay, () => twTitle.ShowText(itemTitle)));
        else tmpTitle.text = itemTitle;
        _seq.Group(Tween.PunchScale(cgPrice.transform, Vector3.one*-0.375f,0.375f, 2, startDelay: delay));
    }

    public void Show()
    {
        if (isShowed) return;
        isShowed = true;
        
        _seqShow.Stop();
        gameObject.SetActive(true);
        _seqShow = Sequence.Create();
        _seqShow.Chain(Tween.Alpha(cgMain, 1, 0.5f));
    }
    public void Hide()
    {
        if (!isShowed) return;
        isShowed = false;
        
        _seqShow.Stop();
        _seqShow = Sequence.Create();
        _seqShow.Chain(Tween.Alpha(cgMain, 0, 0.5f));
        _seqShow.OnComplete(() => gameObject.SetActive(false));
    }
    private void PresetWeapon(Item_Weapon item)
    {
        _currentWeapon = item;
        bannerPackage.main.SetActive(false);
        int index = 2;
        Image icon = banners[index].icon;
        icon.sprite = item.icon;
        icon.rectTransform.offsetMin = new Vector2(item.sleft, item.sbottom);
        icon.rectTransform.offsetMax = new Vector2(-item.sright, -item.stop);
        icon.rectTransform.localScale = item.sscale;
        banners[index].imgColorDeco.color = item.decoColor;
        banners[index].particleImage.sprite = item.star;
        for (int i = 0; i < 3; i++) banners[i].main.SetActive(i==index);
    }
    private void PresetResource(Item_Resource item)
    {
        _currentResource = item;
        bannerPackage.main.SetActive(false);
        if (!item.isSpecial)
        {
            int index = 0;
            Image icon = banners[index].icon;
            icon.sprite = item.icon;
            icon.rectTransform.offsetMin = new Vector2(item.sleft, item.sbottom);
            icon.rectTransform.offsetMax = new Vector2(-item.sright, -item.stop);
            icon.rectTransform.localScale = item.sscale;
            banners[index].particleImage.sprite = starResourceNorm;
            for (int i = 0; i < 3; i++) banners[i].main.SetActive(i==index);
        }
        else
        {
            int index = 1;
            Image icon = banners[index].icon;
            icon.sprite = item.icon;
            icon.rectTransform.offsetMin = new Vector2(item.sleft, item.sbottom);
            icon.rectTransform.offsetMax = new Vector2(-item.sright, -item.stop);
            icon.rectTransform.localScale = item.sscale;
            banners[index].particleImage.sprite = starResourceSpecial;
            for (int i = 0; i < 3; i++) banners[i].main.SetActive(i==index);
        }
    }
    private void PresetPackage(Item_ShopPackage package)
    {
        _currentPackage = package;
        foreach (var banner in banners) banner.main.SetActive(false);
        bannerPackage.main.SetActive(true);
        bannerPackage.icon.sprite = package.sprite;
        bannerPackage.icon.rectTransform.offsetMin = new Vector2(package.left, package.bottom);
        bannerPackage.icon.rectTransform.offsetMax = new Vector2(-package.right, -package.top);
        bannerPackage.icon.rectTransform.localScale = package.scale;
    }
    [Button]
    public void SetItem()
    {
        if (_currentWeapon != null)
        {
            bannerPackage.main.SetActive(false);
            for (int i = 0; i < 3; i++) banners[i].main.SetActive(i==2);
            if (_currentWeapon.isGem) _priceType = PriceType.gem;
            else _priceType = PriceType.coin;
            itemPrice = _currentWeapon.price;
            itemTitle = _currentWeapon.title;
            
            _seqIcon.Stop();
            _seqIcon = Sequence.Create();
            banners[2].icon.color = Color.clear;
            _seqIcon.Chain(Tween.Color(banners[2].icon, Color.white, 0.25f));
        }
        else if (_currentResource != null)
        {
            bannerPackage.main.SetActive(false);
            int index = 0;
            if(_currentResource.isSpecial) index=1;
            else for (int i = 0; i < 3; i++) index=0;
            for (int i = 0; i < 3; i++) banners[i].main.SetActive(i == index);
            if (_currentResource.isGem) _priceType = PriceType.gem;
            else _priceType = PriceType.coin;
            itemPrice = _currentResource.price;
            itemTitle = _currentResource.title;
            
            _seqIcon.Stop();
            _seqIcon = Sequence.Create();
            banners[index].icon.color = Color.clear;
            _seqIcon.Chain(Tween.Color(banners[index].icon, Color.white, 0.25f));
        }
    }
    [Button]
    public void SetPackage()
    {
        bannerPackage.main.SetActive(true);
        for (int i = 0; i < 3; i++) banners[i].main.SetActive(false);
        _priceType = PriceType.money;
        itemPrice = _currentPackage.price;
        itemTitle = _currentPackage.title;
        
        _seqIcon.Stop();
        _seqIcon = Sequence.Create();
        bannerPackage.icon.color = Color.clear;
        _seqIcon.Chain(Tween.Color(bannerPackage.icon, Color.white, 0.25f));
    }

    public void Clicked()
    {
        if (!_seq.isAlive)
        {
            transform.localScale = Vector3.one;
            _seq = Sequence.Create();
            _seq.Group(Tween.PunchScale(transform, Vector3.one * -0.1f, 0.25f, 2));   
        }
        SoundManager.Play(SoundContainer_StageSelect.instance.sound_click);
        PopupManager.instance.Negative("아이템 구매는 아직 개발중입니다.",1.5f);
    }
}
[System.Serializable]
public class ShopBannerSingle
{
    public Image icon;
    public GameObject main;
    public Image imgColorDeco;
    public ParticleImage particleImage;
    public CanvasGroup cg;
}

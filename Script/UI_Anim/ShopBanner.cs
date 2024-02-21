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
    public TypewriterCore twTitle,twPrice;
    public CanvasGroup cgPrice;
    public CanvasGroup cgMain;
    private float _bottom;
    private Sequence _seq,_seqShow;
    public bool isShowed = false;
    public List<ShopBannerSingle> banners = new List<ShopBannerSingle>();
    public Sprite starResourceNorm, starResourceSpecial;
    
    public void Setting()
    {
        tmpTitle.text = string.Empty;
        tmpPrice.text = string.Empty;
        _bottom = rt.rect.size.y*0.375f;
    }
    public void Open(float delay)
    {
        _seq.Stop();
        gameObject.SetActive(true);
        if(twTitle.isHidingText)twTitle.StopDisappearingText();
        if(twTitle.isShowingText)twTitle.StopShowingText();
        if(twPrice.isHidingText)twPrice.StopDisappearingText();
        if(twPrice.isShowingText)twPrice.StopShowingText();
        tmpTitle.text = string.Empty;
        tmpPrice.text = string.Empty;
        transform.localScale = Vector3.one;
        cgPrice.transform.localScale = Vector3.one*0.75f;
        cgPrice.alpha = 0;
        
        rt.offsetMin= new Vector2(rt.offsetMin.x,_bottom);
        
        _seq = Sequence.Create();
        if (gameObject.activeSelf) _seq.Group(Tween.Delay(delay, () => twTitle.ShowText("상점 아이템\n예시")));
        else tmpTitle.text = "상점 아이템\n예시";
        _seq.Group(Tween.UIOffsetMinY(rt, 0, 0.75f, Ease.InOutBack,startDelay:delay));
        if (gameObject.activeSelf) _seq.Group(Tween.Delay(delay + 0.375f, () => twPrice.ShowText("2000 krw")));
        else tmpPrice.text = "2000 krw";
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
        tmpPrice.text = string.Empty;
        transform.localScale = Vector3.one;
        cgPrice.transform.localScale = Vector3.one;
        
        _seq = Sequence.Create();
        _seq.Group(Tween.PunchScale(transform, Vector3.up * -0.05f, 0.375f, 2, startDelay: delay));
        if (gameObject.activeSelf) _seq.Group(Tween.Delay(delay, () => twTitle.ShowText("상점 아이템\n예시")));
        else tmpTitle.text = "상점 아이템\n예시";
        _seq.Group(Tween.PunchScale(cgPrice.transform, Vector3.one*-0.375f,0.375f, 2, startDelay: delay));
        if (gameObject.activeSelf) _seq.Group(Tween.Delay(delay+0.125f,() => twPrice.ShowText("2000 krw")));
        else tmpPrice.text = "2000 krw";
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
    [Button]
    public void SetWeapon(Item_Weapon item)
    {
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
    [Button]
    public void SetResource(Item_Resource item)
    {
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
}
[System.Serializable]
public class ShopBannerSingle
{
    public Image icon;
    public GameObject main;
    public Image imgColorDeco;
    public ParticleImage particleImage;
}

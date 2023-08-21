using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_Stick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private RectTransform parentRect;
    private RectTransform thisRect;
    public RectTransform bgRect;
    public bool follow = true;
    public float minMoveRange = 10;
    public float maxMoveRange = 200;
    public Vector2 addVec;
    private float w, h;
    public void Setting()
    {
        thisRect = ((RectTransform) transform);
        parentRect = transform.parent.GetComponentInParent<RectTransform>();
        m_StartPos = thisRect.anchoredPosition;
        addVec = new Vector2(parentRect.sizeDelta.x * (0.5f - (thisRect.anchorMin.x + thisRect.anchorMax.x) * 0.5f),
            parentRect.sizeDelta.y * (0.5f - (thisRect.anchorMin.y + thisRect.anchorMax.y) * 0.5f));
        bgRect.anchorMin = thisRect.anchorMin;
        bgRect.anchorMax = thisRect.anchorMax;
        bgRect.anchoredPosition = thisRect.anchoredPosition;
        btn_CanvasGroup = thisRect.GetComponent<CanvasGroup>();
        bg_CanvasGroup = bgRect.GetComponent<CanvasGroup>();
        btn_beginSize = thisRect.localScale;
        bg_beginSize = bgRect.localScale;
        
        CanvasScaler c = Canvas_Player.instance.GetComponent<CanvasScaler>();
        float wRatio = c.referenceResolution.x / Screen.width;
        float hRatio = c.referenceResolution.y / Screen.height;
        w = wRatio > hRatio ? c.referenceResolution.x : Screen.width * hRatio;
        h = wRatio < hRatio ? c.referenceResolution.y : Screen.height * wRatio;
    }

    public Vector2 ConvertPos(Vector2 ap)
    {
        Vector2 anchorMin = thisRect.anchorMin;
        ap.x = (w * 0.5f + ap.x) - (w * anchorMin.x);// + Screen.width*0.25f;
        ap.y = (h * 0.5f + ap.y) - (h * anchorMin.y);// - Screen.height*0.25f;
        return ap;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position,
            eventData.pressEventCamera, out m_PointerDownPos);
        bgRect.anchoredPosition = ConvertPos(m_PointerDownPos);
        thisRect.anchoredPosition = bgRect.anchoredPosition;
        m_DownPos = thisRect.anchoredPosition;
        m_BGPos = m_PointerDownPos;
        //BG_ClickedAnim();
        BTN_ClickedAnim();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position,
            eventData.pressEventCamera, out var position);
        //var delta = position - m_PointerDownPos;
        //thisRect.anchoredPosition = m_StartPos + (Vector3)delta;
        var delta = position - m_PointerDownPos;
        var dist = ((Vector2) (m_DownPos + (Vector3) delta) - bgRect.anchoredPosition).magnitude;
        if (true)//dist > minMoveRange)
        {
            if (follow)
            {
                Vector3 vec = (m_DownPos + (Vector3) delta) - (Vector3) bgRect.anchoredPosition;
                float dst = vec.magnitude;
                thisRect.anchoredPosition = (bgRect.anchoredPosition + (Vector2) vec.normalized * dst);
                m_DownPos = bgRect.anchoredPosition + (Vector2) vec.normalized * dst;
            }
            else
            {
                Vector3 vec = (m_DownPos + (Vector3) delta) - (Vector3) bgRect.anchoredPosition;
                float dst = Mathf.Clamp(vec.magnitude, 0, maxMoveRange);
                thisRect.anchoredPosition = (bgRect.anchoredPosition + (Vector2) vec.normalized * dst);
                m_DownPos = bgRect.anchoredPosition + (Vector2) vec.normalized * dst;
            }
        }
        else
        {
            thisRect.anchoredPosition = (bgRect.anchoredPosition);
            m_DownPos += (Vector3) delta;
        }


        if (follow && (bgRect.anchoredPosition - thisRect.anchoredPosition).sqrMagnitude >
            movementRange * movementRange)
        {
            Vector2 vec = bgRect.anchoredPosition - thisRect.anchoredPosition;
            bgRect.anchoredPosition = (thisRect.anchoredPosition + vec.normalized * movementRange);
        }

        var newPos = (thisRect.anchoredPosition - bgRect.anchoredPosition) / movementRange;
        SendValueToControl(newPos);


        m_PointerDownPos = position;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        thisRect.anchoredPosition = m_StartPos;
        bgRect.anchoredPosition = thisRect.anchoredPosition;
        SendValueToControl(Vector2.zero);
        BG_ClickedAnim();
        BTN_ClickedAnim();
    }

    #region Anim

    private CanvasGroup bg_CanvasGroup, btn_CanvasGroup;
    private Coroutine bg_clickedanim=null,btn_clickedanim=null;
    private Vector3 bg_beginSize, btn_beginSize;
    [TitleGroup("Anim")] public float bgDuration = 0.35f, btnDuration = 0.25f;

    [TitleGroup("Anim")]
    public AnimationCurve bgCurve = AnimationCurve.EaseInOut(0, 0.75f, 1, 1),
        btnCurve = AnimationCurve.EaseInOut(0, 0.75f, 1, 1);
    private void BG_ClickedAnim()
    {
        if(bg_clickedanim!=null) StopCoroutine(bg_clickedanim);
        bg_clickedanim = StartCoroutine(C_BG_ClickedAnim());
    }

    private void BTN_ClickedAnim()
    {
        if(btn_clickedanim!=null) StopCoroutine(btn_clickedanim);
        btn_clickedanim = StartCoroutine(C_BTN_ClickedAnim());
    }
    private IEnumerator C_BG_ClickedAnim()
    {
        float startTime = Time.unscaledTime;
        float endTime = startTime + bgDuration;
        while (Time.unscaledTime<endTime)
        {
            float ratio = (Time.unscaledTime - startTime) / bgDuration;
            bgRect.localScale = bg_beginSize * bgCurve.Evaluate(ratio);
            bg_CanvasGroup.alpha = ratio;
            yield return null;
        }
    }

    private IEnumerator C_BTN_ClickedAnim()
    {
        float startTime = Time.unscaledTime;
        float endTime = startTime + btnDuration;
        while (Time.unscaledTime<endTime)
        {
            float ratio = (Time.unscaledTime - startTime) / btnDuration;
            thisRect.localScale = btn_beginSize * btnCurve.Evaluate(ratio);
            btn_CanvasGroup.alpha = ratio;
            yield return null;
        }
    }
    #endregion
    
    #region 세부 구현


    public float movementRange
    {
        get => m_MovementRange;
        set => m_MovementRange = value;
    }


    [FormerlySerializedAs("movementRange")] [SerializeField]
    private float m_MovementRange = 50;

    [InputControl(layout = "Vector2")] [SerializeField]
    private string m_ControlPath;

    private Vector3 m_StartPos;
    private Vector3 m_DownPos;
    private Vector2 m_BGPos;
    private Vector2 m_PointerDownPos;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    #endregion
}
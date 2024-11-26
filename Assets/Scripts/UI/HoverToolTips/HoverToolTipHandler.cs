using System;
using TMPro;
using UnityEngine;

public class HoverToolTipHandler : MonoBehaviour
{
    [SerializeField] float _maxTipWidth = 200.0f;

    public TextMeshProUGUI tipText;
    public RectTransform tipWindow;

    public Action<string, Vector2> OnMouseHover;
    public Action OnMouseLoseFocus;

    void OnEnable()
    {
        OnMouseHover += ShowTip;
        OnMouseLoseFocus += HideTip;
    }

    void OnDisable()
    {
        OnMouseHover -= ShowTip;
        OnMouseLoseFocus -= HideTip;
    }

    void Start()
    {
        HideTip();
    }

    void ShowTip(string tip, Vector2 mousePosition)
    {
        float windowX = mousePosition.x + tipWindow.sizeDelta.x / 2;
        if (mousePosition.x + tipWindow.sizeDelta.x >= Screen.width)
        {
            windowX = mousePosition.x - tipWindow.sizeDelta.x / 2;
        }

        float windowY = mousePosition.y + tipWindow.sizeDelta.y / 2;
        if (mousePosition.y + tipWindow.sizeDelta.y >= Screen.height)
        {
            windowY = mousePosition.y - tipWindow.sizeDelta.y / 2;
        }

        tipText.text = tip;
        float x = Math.Min(_maxTipWidth, tipText.preferredWidth);
        tipWindow.sizeDelta = new Vector2(x, tipText.preferredHeight);

        tipWindow.gameObject.SetActive(true);

        tipWindow.transform.position = new Vector2(windowX, windowY);
        tipText.transform.position = new Vector2(tipWindow.transform.position.x, tipWindow.transform.position.y);
    }


    void HideTip()
    {
        tipText.text = string.Empty;
        tipWindow.gameObject.SetActive(false);
    }
}
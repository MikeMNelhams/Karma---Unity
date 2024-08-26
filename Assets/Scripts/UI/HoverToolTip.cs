using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] float _timeToWait = 0.5f;

    protected HoverToolTipHandler HoverTipHandler { get; set; }
    public string toolTipText;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        if (HoverTipHandler != null)
        {
            HoverTipHandler.OnMouseLoseFocus();
        }   
    }

    void ShowMessage()
    {
        UnityEngine.Debug.Log("Hover over " + gameObject.name);
        if (HoverTipHandler != null)
        {
            HoverTipHandler.OnMouseHover(toolTipText, Input.mousePosition);
        }
    }

    protected IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(_timeToWait);

        ShowMessage();
    }
}

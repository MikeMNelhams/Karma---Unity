using UnityEngine;
using UnityEngine.EventSystems;
using CardToolTips;

public class CardObjectHoverToolTip : HoverToolTip
{
    [SerializeField] CardObject _cardObject;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();

        if (_cardObject.transform.parent == null)
        {
            return;
        }

        // We need the index of the player with the camera. 
        // Maybe swap this to a getComponent on the camera itself, rather than going -> parent -> parent -> get component

        Camera playerCamera = eventData.enterEventCamera;
        if (playerCamera.gameObject.transform.parent == null) { Debug.Break(); return; }

        PlayerHandler observerPlayerProperties = playerCamera.gameObject.transform.parent.parent.GetComponent<PlayerHandler>();
        if (!observerPlayerProperties.IsToolTipsEnabled) { return; }

        if (_cardObject.IsVisible(observerPlayerProperties.Index))
        {
            HoverTipHandler = observerPlayerProperties.HoverTipHandler;

            if (_cardObject.CurrentCard is null) { return; }

            CardToolTipText tipText = CardTipTextManager.Instance.GetCardToolTipText(_cardObject.CurrentCard.Value);

            ToolTipText = tipText.CardEffectText;
            StartCoroutine(StartTimer());
        }
    }
}

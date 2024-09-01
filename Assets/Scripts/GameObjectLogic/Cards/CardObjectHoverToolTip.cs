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
        // Maybe swap this to a getComponent on the camera, rather than going -> parent -> parent

        Camera playerCamera = eventData.enterEventCamera;
        PlayerProperties observerPlayerProperties = playerCamera.gameObject.transform.parent.parent.GetComponent<PlayerProperties>();
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

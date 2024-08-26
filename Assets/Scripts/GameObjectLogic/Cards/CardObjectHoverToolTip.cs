using UnityEngine;
using UnityEngine.EventSystems;

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
        // Ideas: 
        // 1. Get the camera from eventData,
        // get the player (parent or by reference on mono on the camera?)
        // then the playerProperties monobehaviour associated w/ player,
        // then get the index
        // Maybe swap this to a getComponent on the camera, rather than going -> parent -> parent

        // 2. Don't use onPointerHandler? Code this directly as raycasts in the already existing PlayerPhysics controller thing
        // You then have access to the observerIndex (duh) and you can process the gameobject on hit. This also uses the same # of getcomponent
        // Doesn't use the onPointerEvent interface tho...
        Camera playerCamera = eventData.enterEventCamera;
        PlayerProperties observerPlayerProperties = playerCamera.gameObject.transform.parent.parent.GetComponent<PlayerProperties>();
        if (_cardObject.IsVisible(observerPlayerProperties.Index))
        {
            //HoverTipHandler = observerPlayerProperties.HoverTipHandler;
            StartCoroutine(StartTimer());
        }
    }
}

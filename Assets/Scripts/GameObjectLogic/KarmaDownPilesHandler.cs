using System.Collections.Generic;
using UnityEngine;
using CardVisibility;
using KarmaLogic.Cards;

public class KarmaDownPilesHandler : MonoBehaviour, ICardVisibilityHandler
{
    [SerializeField] Transform _debugCube;
    [SerializeField] float _leftOffset = 0.1f;
    [SerializeField] float _rightOffset = 0.1f;

    public int OwnerIndex { get; protected set; }

    public bool IsVisible(int observerPlayerIndex)
    {
        return KarmaGameManager.Instance.PlayersProperties[observerPlayerIndex].IsKarmaDownFlippedUp;
    }

    public bool IsOwnedBy(int observerPlayerIndex)
    {
        return KarmaGameManager.Instance.PlayersProperties[observerPlayerIndex].Index == OwnerIndex;
    }

    public List<SelectableCardObject> CreateKarmaDownCards(CardsList karmaDown, int ownerIndex)
    {
        float width = _debugCube.localScale.x;
        float left = -width / 2 + _leftOffset * width;
        float right = width / 2 - _rightOffset * width;

        if (left > right)
        {
            throw new System.Exception("Cards are overlapping, change leftOffset to be < rightOffset");
        }

        OwnerIndex = ownerIndex;

        float karmaDownOffset = 0.01f;
        float xStepSize = (right - left) / 2;
        float halfHeight = _debugCube.lossyScale.y / 2;

        KarmaGameManager gameManager = KarmaGameManager.Instance;

        int j = 0;
        List<SelectableCardObject> cardObjects = new();
        foreach (Card card in karmaDown)
        {
            float x = left + karmaDownOffset + j * xStepSize;
            Vector3 cardPosition = transform.TransformPoint(new Vector3(x, -halfHeight, 0));
            Quaternion cardRotation = Quaternion.Euler(90, -transform.rotation.eulerAngles.y, 0);
            GameObject cardGameObject = gameManager.InstantiateCard(card, cardPosition, cardRotation, gameObject);
            SelectableCardObject cardObject = cardGameObject.GetComponent<SelectableCardObject>();
            cardObject.SetParent(this);
            cardObjects.Add(cardObject);
            j++;
        }

        return cardObjects;
    }
}

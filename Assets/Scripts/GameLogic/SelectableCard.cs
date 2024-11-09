using KarmaLogic.Cards;
using CardVisibility;
using UnityEngine;
using System;

public abstract class SelectableCard : MonoBehaviour, ICardVisibilityHandler
{
    public Card CurrentCard { get; set; }
    protected ICardVisibilityHandler _cardVisibilityHandler;

    public void SetParent(ICardVisibilityHandler cardVisibilityHandler, Transform parentTransform)
    {
        transform.parent = parentTransform;
        SetParent(cardVisibilityHandler);
    }

    public void SetParent(ICardVisibilityHandler cardVisibilityHandler)
    {
        _cardVisibilityHandler = cardVisibilityHandler;
    }

    public abstract void ResetCardBorder();

    public abstract void DisableSelectShader();

    public abstract void ToggleSelectShader();

    public abstract void ColorCardBorder(Color color);

    public bool IsVisible(int observerPlayerIndex)
    {
        if (_cardVisibilityHandler == null)
        {
            throw new SystemException("Card has no parent set!");
        }

        return _cardVisibilityHandler.IsVisible(observerPlayerIndex);
    }

    public bool IsOwnedBy(int observerPlayerIndex)
    {
        if (_cardVisibilityHandler == null)
        {
            throw new SystemException("Card has no parent set!");
        }

        return _cardVisibilityHandler.IsOwnedBy(observerPlayerIndex);
    }
}

using System.Collections;
using UnityEngine;
using StateMachine;
using StateMachine.CharacterStateMachines;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using KarmaLogic.CardCombos;
using DataStructures;

public class CardLegalityHinter
{
    bool _areLegalHintsEnabled;
    PlayerHandler _playerHandler;

    public bool AreLegalHintsEnabled { get => _areLegalHintsEnabled; set => _areLegalHintsEnabled = value; }

    public CardLegalityHinter(PlayerHandler playerHandler, bool areLegalHintsEnabled)
    {
        _areLegalHintsEnabled = areLegalHintsEnabled;
        _playerHandler = playerHandler;
    }

    public void LegalHint()
    {
        if (!_areLegalHintsEnabled || _playerHandler.StateMachine.CurrentState is State.Mulligan) 
        { 
            LegalHintDefaults(_playerHandler.SelectableCardObjects); 
            return; 
        }

        if (_playerHandler.StateMachine.CurrentState is not State.SelectingCardGiveAwayIndex)
        {
            foreach (SelectableCardObject cardObject in _playerHandler.SelectableCardObjects)
            {
                LegalHintPlayableCard(cardObject, _playerHandler.CardSelector);
            }
        }
        else
        {
            foreach (SelectableCardObject cardObject in _playerHandler.SelectableCardObjects)
            {
                LegalHintGiveableCard(cardObject);
            }
        }
    }

    public void LegalHintGiveableCard(SelectableCardObject cardObject)
    {
        if (cardObject.CurrentCard.Value == CardValue.JOKER)
        {
            cardObject.ColorCardBorder(Color.red);
            return;
        }

        cardObject.ColorCardBorder(Color.green);
    }

    public void LegalHintPlayableCard(SelectableCardObject cardObject, CardSelector cardSelector)
    {
        LegalCombos legalCombos = KarmaGameManager.Instance.Board.CurrentLegalCombos;

        FrozenMultiSet<CardValue> combinedSelection = new();
        FrozenMultiSet<CardValue> selectionCardValues = cardSelector.SelectionCardValues;

        foreach (CardValue cardValue in selectionCardValues)
        {
            combinedSelection.Add(cardValue, selectionCardValues[cardValue]);
        }

        if (!cardSelector.CardObjects.Contains(cardObject))
        {
            combinedSelection.Add(cardObject.CurrentCard.Value);
        }

        if (legalCombos.IsLegal(combinedSelection))
        {
            cardObject.ColorCardBorder(Color.green);
            return;
        }

        if (legalCombos.IsSubsetLegal(combinedSelection))
        {
            cardObject.ColorCardBorder(Color.blue);
            return;
        }

        cardObject.ColorCardBorder(Color.red);
        return;
    }

    public void ResetAllLegalHints()
    {
        LegalHintDefaults(_playerHandler.CardsInHand);
        LegalHintDefaults(_playerHandler.CardsInKarmaUp);
        LegalHintDefaults(_playerHandler.CardsInKarmaDown);
    }

    public void LegalHintDefaults(IEnumerable cardObjects)
    {
        foreach (SelectableCardObject cardObject in cardObjects)
        {
            cardObject.ResetCardBorder();
        }
    }
}

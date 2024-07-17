using Karma.Players;
using Karma.Cards;
using Karma.CardCombos;
using Karma.Controller;
using Karma.Board;
using System.Collections.Generic;
using DataStructures;
using System.Linq;
using System;
using Karma.Board.BoardEvents;
using System.Diagnostics;
using UnityEngine;
using Karma.Board.BoardPrinters;


namespace Karma
{
    namespace BasicBoard
    {
        public class BasicBoard : IBoard
        {
            public static BoardPrinterDebug boardPrinterDefault = new();

            public List<Player> Players { get; set; }
            public CardPile DrawPile { get; protected set; }
            public CardPile BurnPile { get; protected set; }
            public PlayCardPile PlayPile { get; protected set; }
            public BoardEventSystem BoardEventSystem { get; protected set; }
            public IBoardPrinter BoardPrinter { get; protected set; }
            public BoardPlayOrder PlayOrder { get; protected set; }
            public BoardTurnOrder TurnOrder { get; protected set; }
            public bool HandsAreFlipped { get; set; } 
            public int EffectMultiplier { get; set; } 
            public int CurrentPlayerIndex { get; set; }
            public Player CurrentPlayer { get => Players[CurrentPlayerIndex]; }
            public DictionaryDefaultInt<CardValue> CardValuesInPlayCounts { get; protected set; }
            public DictionaryDefaultInt<CardValue> CardValuesTotalCounts { get; protected set; }
            public bool HasBurnedThisTurn { get; protected set; }
            public int TurnsPlayed { get; protected set; }
            public int NumberOfCardsDrawnThisTurn { get; protected set; } 
            public PlayingFrom StartingPlayerStartedPlayingFrom { get; protected set; }

            public int TotalJokers { get; protected set; }
            public int NumberOfCombosPlayedThisTurn { get; protected set; }

            public List<CardCombo> ComboHistory { get; protected set; }
            public CardComboFactory ComboFactory { get; protected set; }
            public int PlayerIndexWhoStartedTurn { get; protected set; }

            HashSet<BoardPlayerAction> _allActions;

            public BasicBoard(List<Player> players)
            {
                CardPile drawPile = CardPile.Empty;
                CardPile burnPile = CardPile.Empty;
                PlayCardPile playCardPile = PlayCardPile.Empty;

                SetInitParams(players, drawPile, burnPile, playCardPile);
            }

            public BasicBoard(List<Player> players, CardPile drawPile)
            {
                CardPile burnPile = CardPile.Empty;
                PlayCardPile playCardPile = PlayCardPile.Empty;

                SetInitParams(players, drawPile, burnPile, playCardPile);
            }

            public BasicBoard(List<Player> players, CardPile drawPile, CardPile burnPile, PlayCardPile playPile,
                BoardTurnOrder turnOrder = BoardTurnOrder.RIGHT, BoardPlayOrder playOrder = BoardPlayOrder.UP,
                bool handsAreFlipped = false, int effectMultiplier = 1, int whoStarts = 0,
                bool hasBurnedThisTurn = false, int turnsPlayed = 0, IBoardPrinter boardPrinter = null)
            {
                SetInitParams(players, drawPile, burnPile, playPile, turnOrder, playOrder, handsAreFlipped, 
                    effectMultiplier, whoStarts, hasBurnedThisTurn, turnsPlayed, null);
            }

            private void SetInitParams(List<Player> players, CardPile drawPile, CardPile burnPile, PlayCardPile playPile,
                BoardTurnOrder turnOrder = BoardTurnOrder.RIGHT, BoardPlayOrder playOrder = BoardPlayOrder.UP,
                bool handsAreFlipped = false, int effectMultiplier = 1, int whoStarts = 0,
                bool hasBurnedThisTurn = false, int turnsPlayed = 0, IBoardPrinter boardPrinter = null)
            {
                Players = players;
                DrawPile = drawPile;
                BurnPile = burnPile;
                PlayPile = playPile;
                TurnOrder = turnOrder;
                PlayOrder = playOrder;
                HandsAreFlipped = handsAreFlipped;
                EffectMultiplier = effectMultiplier;
                CurrentPlayerIndex = whoStarts;
                HasBurnedThisTurn = hasBurnedThisTurn;
                TurnsPlayed = turnsPlayed;
                NumberOfCardsDrawnThisTurn = 0;

                CardValuesInPlayCounts = CountCardValuesInPlay();
                CardValuesTotalCounts = CountCardValuesTotal(CardValuesInPlayCounts);
                StartingPlayerStartedPlayingFrom = Players[whoStarts].PlayingFrom;

                BoardPrinter = boardPrinter is null ? boardPrinterDefault : boardPrinter;
                ComboHistory = new List<CardCombo>();
                ComboFactory = new CardComboFactory();
                BoardEventSystem = new BoardEventSystem();
                CurrentLegalActions = new HashSet<BoardPlayerAction>();
                CurrentLegalCombos = new HashSet<FrozenMultiSet<CardValue>>();
                _allActions = new () {new PickupPlayPile(), new PlayCardsCombo()};

                CalculateLegalCombos(CurrentPlayer.PlayableCards);
                CalculateLegalActions();
            }

            DictionaryDefaultInt<CardValue> CountCardValuesInPlay()
            {
                DictionaryDefaultInt<CardValue> counts = new();
                foreach (Player player in Players)
                {
                    counts.UnionInPlace(player.CountAllCardValues());
                }

                counts.UnionInPlace(DrawPile.CountAllCardValues());
                counts.UnionInPlace(PlayPile.CountAllCardValues());

                return counts;
            }

            DictionaryDefaultInt<CardValue> CountCardValuesTotal(DictionaryDefaultInt<CardValue> cardValuesInPlayCounts)
            {
                DictionaryDefaultInt<CardValue> counts = new();
                counts.UnionInPlace(cardValuesInPlayCounts);
                counts.UnionInPlace(BurnPile.CountAllCardValues());
                return counts;
            }

            public void FlipTurnOrder()
            {
                if (TurnOrder == BoardTurnOrder.LEFT) { TurnOrder = BoardTurnOrder.RIGHT; }
                else if (TurnOrder == BoardTurnOrder.RIGHT) { TurnOrder = BoardTurnOrder.LEFT; }
            }

            public void SetTurnOrder(BoardTurnOrder turnOrder)
            {
                TurnOrder = turnOrder;
            }

            public void FlipPlayOrder()
            {
                if (PlayOrder == BoardPlayOrder.UP) { PlayOrder = BoardPlayOrder.DOWN; }
                else if (PlayOrder == BoardPlayOrder.DOWN) { PlayOrder = BoardPlayOrder.UP; }
            }

            public void ResetPlayOrder()
            {
                PlayOrder = BoardPlayOrder.UP;
            }

            public void FlipHands()
            {
                HandsAreFlipped = !HandsAreFlipped;
                BoardEventSystem.TriggerHandsFlippedEvent(this);
            }

            public void RotateHands(int numberOfRotations, Deque<Hand> hands)
            {
                if (numberOfRotations == 0) { return; }
                for (int i = 0; i < numberOfRotations; i++)
                {
                    hands.Rotate((int)TurnOrder);
                    BoardEventSystem.TriggerHandsRotatedEventListener((int)TurnOrder, this);
                    for (int j = 0; j < Players.Count; j++)
                    {
                        Players[j].Hand = hands[j];
                    }
                }     
            }

            public void StartGivingAwayPlayPile(int giverIndex)
            {
                BoardEventSystem.TriggerStartedPlayPileGiveAway(giverIndex);
            }

            public void StartGivingAwayCards(int numberOfCards, CardGiveAwayHandler.InvalidFilter invalidFilter = null)
            {
                CurrentPlayer.CardGiveAwayHandler = new CardGiveAwayHandler(numberOfCards, this, CurrentPlayerIndex, invalidFilter);
                CurrentPlayer.CardGiveAwayHandler.RegisterOnCardGiveAwayListener(ReceiveCard);
                BoardEventSystem.TriggerStartedCardGiveAway(numberOfCards, CurrentPlayerIndex);
            }

            void ReceiveCard(Card card, int giverIndex, int receiverIndex)
            {
                Player giver = Players[giverIndex];
                Player receiver = Players[receiverIndex];
                receiver.ReceiveCard(card, giver);
                //DrawUntilFull(giverIndex); // For some reason THIS doesn't work??
            }

            public void StartTurn()
            {
                PlayerIndexWhoStartedTurn = CurrentPlayerIndex;
                StartingPlayerStartedPlayingFrom = Players[PlayerIndexWhoStartedTurn].PlayingFrom;
                NumberOfCombosPlayedThisTurn = 0;
                NumberOfCardsDrawnThisTurn = 0;
                HasBurnedThisTurn = false;
                CalculateLegalCombos(CurrentPlayer.PlayableCards);
                CalculateLegalActions();

                BoardEventSystem.TriggerOnTurnStartEvents(this);
            }

            public void EndTurn()
            {
                TurnsPlayed++;
                BoardEventSystem.TriggerOnTurnEndEvents(this);
            }

            public bool PlayCards(CardsList cards, IController controller)
            {
                return PlayCards(cards, controller, true);
            }

            public bool PlayCards(CardsList cards, IController controller, bool addToPile=true)
            {
                ComboFactory.SetCounts(cards);
                CardCombo cardCombo = ComboFactory.CreateCombo(controller);
                List<bool> comboVisibility = ComboFactory.ComboVisibility(this);
                if (addToPile) { PlayPile.Add(cards, comboVisibility); }
                bool willBurnDueToMinimumRunFour = PlayPile.ContainsMinLengthRun(4);

                DrawUntilFull(CurrentPlayerIndex);

                if (NumberOfCombosPlayedThisTurn > 52) { return false; }
                cardCombo.Apply(this);

                ResetEffectMultiplierIfNecessary(ComboFactory.ComboCardValue());
                NumberOfCombosPlayedThisTurn++;
                ComboHistory.Add(cardCombo);

                int jokerCount = PlayPile.CountValue(CardValue.JOKER);

                if (willBurnDueToMinimumRunFour || jokerCount > 0) 
                {
                    Burn(jokerCount);
                }

                return false;
            }

            public void Burn(int jokerCount)
            {
                EffectMultiplier = 1;
                if (PlayPile.Count == 0) { return; }

                HasBurnedThisTurn = true;
                if (jokerCount > 0)
                {
                    List<int> indicesToBurn = new ();
                    if (jokerCount == 1)
                    {
                        indicesToBurn.Add(PlayPile.Count - 1);
                    }
                    else
                    {
                        int start = PlayPile.Count - jokerCount;
                        indicesToBurn = Enumerable.Range(start, jokerCount).ToList<int>();
                    }

                    CardsList cardsToBurn = PlayPile.PopMultiple(indicesToBurn.ToArray());
                    CardValuesInPlayCounts[CardValue.JOKER] -= cardsToBurn.CountValue(CardValue.JOKER);
                    BurnPile.Add(cardsToBurn);
                    BoardEventSystem.TriggerBurnEvents(jokerCount);
                    return;
                }
                BurnPile.Add(PlayPile);
                UnityEngine.Debug.Log(PlayPile.CountAllCardValues());
                CardValuesInPlayCounts.SubtractInPlace(PlayPile.CountAllCardValues());

                PlayPile.Clear();
                BoardEventSystem.TriggerBurnEvents(jokerCount);
                return;
            }

            public void StepPlayerIndex(int numberOfRepeats)
            {
                CurrentPlayerIndex += ((int)TurnOrder) * numberOfRepeats;
                CurrentPlayerIndex %= Players.Count;
                CurrentPlayerIndex = CurrentPlayerIndex < 0 ? CurrentPlayerIndex + Players.Count : CurrentPlayerIndex;
            }

            public void Print()
            {
                BoardPrinter.PrintBoard(this);
            }

            public void PrintChooseableCards()
            {
                BoardPrinter.PrintChoosableCards(this);
            }

            public CardsList DrawUntilFull(int playerIndex)
            {
                if (DrawPile.Count == 0) { return new CardsList(); }

                Player player = Players[playerIndex];

                if (player.Hand.Count >= 3) { return new CardsList(); }
                int handStartSize = player.Hand.Count;
                CardsList cardsDrawn = new ();
                for (int i = 0; i < 3 - handStartSize; i++) 
                {
                    if (DrawPile.Count == 0) { break; }
                    cardsDrawn.Add(player.DrawCard(DrawPile));
                }

                NumberOfCardsDrawnThisTurn += cardsDrawn.Count;
                BoardEventSystem.TriggerPlayerDrawEvents(cardsDrawn.Count, playerIndex);
                return cardsDrawn;
            }

            void ResetEffectMultiplierIfNecessary(CardValue cardValue)
            {
                if (cardValue == CardValue.THREE) { return; }
                if (cardValue != CardValue.JACK) { EffectMultiplier = 1; return; }
                if (ComboHistory.Count == 0) { EffectMultiplier = 1; }
                else if (ComboHistory[^1].GetType() != typeof(CardCombo_THREE))
                {
                    EffectMultiplier = 1;
                }
            }

            public HashSet<int> PotentialWinnerIndices 
            {
                get 
                {
                    HashSet<int> indices = new ();
                    for (int i = 0; i < Players.Count; i++)
                    {
                        {
                            Player player = Players[i];
                            if (!player.HasCards)
                            {
                                indices.Add(i);
                            }
                        }
                    }
                    return indices;
                }
            }

            public HashSet<FrozenMultiSet<CardValue>> CurrentLegalCombos { get; protected set; }
            public HashSet<BoardPlayerAction> CurrentLegalActions { get; protected set; }

            void CalculateLegalActions()
            {
                HashSet<BoardPlayerAction> actions = new ();
                foreach (BoardPlayerAction action in _allActions)
                {
                    if (action.IsValid(this))
                    {
                        actions.Add(action);
                    }
                }
                CurrentLegalActions = actions;
            }

            void CalculateLegalCombos(CardsList cards)
            {
                CurrentLegalCombos = LegalCombos(cards);
            }

            HashSet<FrozenMultiSet<CardValue>> LegalCombos(CardsList cards)
            {
                if (cards.Count == 0) { return new HashSet<FrozenMultiSet<CardValue>>(); }
                if (HandsAreFlipped) 
                {
                    return CardComboCalculator.HandFlippedCombos(cards);
                }

                if (PlayPile.Count == 0 || PlayPile.VisibleTopCard is null)
                {
                    return CardComboCalculator.FillerAndFilterCombos(cards, CardValue.SIX, CardComboCalculator.IsJoker, 3);
                }

                Card topCard = PlayPile.VisibleTopCard as Card;
                CardsList validCards = CardComboCalculator.PlayableCards(PlayOrder, cards, topCard.value);

                if (topCard.value == CardValue.ACE)
                {
                    if (CardComboCalculator.ContainsUnplayableFiller(PlayOrder, cards, topCard.value))
                    {
                        return CardComboCalculator.FillerNotExclusiveCombos(validCards, CardValue.SIX, 3);
                    }
                    return CardComboCalculator.FillerCombos(validCards, CardValue.SIX, 3);
                }

                if (CardComboCalculator.ContainsUnplayableFiller(PlayOrder, cards, topCard.value))
                {
                    return CardComboCalculator.FillerFilterNotExclusiveCombos(validCards, CardValue.SIX, CardComboCalculator.IsJoker, 3);
                }

                return CardComboCalculator.FillerAndFilterCombos(validCards, CardValue.SIX, CardComboCalculator.IsJoker, 3);
            }
        }
    }
}
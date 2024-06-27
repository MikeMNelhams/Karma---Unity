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


namespace Karma
{
    namespace BasicBoard
    {
        public class BasicBoard : IBoard
        {
            public List<Player> Players { get; set; }
            public CardPile DrawPile { get; protected set; }
            public CardPile BurnPile { get; protected set; }
            public PlayCardPile PlayPile { get; protected set; }
            public BoardEventSystem BoardEventSystem { get; protected set; }
            public BoardPlayOrder PlayOrder { get; protected set; }
            public BoardTurnOrder TurnOrder { get; protected set; }
            public bool HandsAreFlipped { get; set; } 
            public int EffectMultiplier { get; set; } 
            public int CurrentPlayerIndex { get; set; }
            public Player CurrentPlayer { get => Players[CurrentPlayerIndex]; }
            public int NumberOfJokersInPlay { get; set; }
            public bool HasBurnedThisTurn { get; protected set; }
            public int TurnsPlayed { get; protected set; }
            public int NumberOfCardsDrawnThisTurn { get; protected set; } 
            
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
                bool cardsAreFlipped = false, int effectMultiplier = 1, int whoStarts = 0,
                bool hasBurnedThisTurn = false, int turnsPlayed = 0)
            {
                SetInitParams(players, drawPile, burnPile, playPile, turnOrder, playOrder, cardsAreFlipped, 
                    effectMultiplier, whoStarts, hasBurnedThisTurn, turnsPlayed);
            }

            private void SetInitParams(List<Player> players, CardPile drawPile, CardPile burnPile, PlayCardPile playPile,
                BoardTurnOrder turnOrder = BoardTurnOrder.RIGHT, BoardPlayOrder playOrder = BoardPlayOrder.UP,
                bool cardsAreFlipped = false, int effectMultiplier = 1, int whoStarts = 0,
                bool hasBurnedThisTurn = false, int turnsPlayed = 0)
            {
                Players = players;
                DrawPile = drawPile;
                BurnPile = burnPile;
                PlayPile = playPile;
                TurnOrder = turnOrder;
                PlayOrder = playOrder;
                HandsAreFlipped = cardsAreFlipped;
                EffectMultiplier = effectMultiplier;
                CurrentPlayerIndex = whoStarts;
                HasBurnedThisTurn = hasBurnedThisTurn;
                TurnsPlayed = turnsPlayed;
                NumberOfCardsDrawnThisTurn = 0;

                ComboHistory = new List<CardCombo>();
                ComboFactory = new CardComboFactory();
                CardComboFactory cardComboFactory = new ();
                BoardEventSystem = new BoardEventSystem();
                CurrentLegalActions = new HashSet<BoardPlayerAction>();
                CurrentLegalCombos = new HashSet<FrozenMultiSet<CardValue>>();
                _allActions = new () {new PickupPlayPile(), new PlayCardsCombo()};
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
            }

            public void StartTurn()
            {
                PlayerIndexWhoStartedTurn = CurrentPlayerIndex;
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
                if (addToPile) { PlayPile.Add(cards); }
                bool willBurnDueToMinimumRunFour = PlayPile.ContainsMinLengthRun(4);

                NumberOfCardsDrawnThisTurn += DrawUntilFull().Count;

                if (NumberOfCombosPlayedThisTurn > 52) { return false; }
                cardCombo.Apply(this);

                NumberOfCombosPlayedThisTurn++;
                ComboHistory.Add(cardCombo);

                if (willBurnDueToMinimumRunFour) 
                {
                    Burn(PlayPile.CountValue(CardValue.JOKER));
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
                        CardsList cardsToBurn = PlayPile.PopMultiple(indicesToBurn.ToArray());
                        NumberOfJokersInPlay -= cardsToBurn.CountValue(CardValue.JOKER);
                        BurnPile.Add(cardsToBurn);
                        BoardEventSystem.TriggerBurnEvents(jokerCount);
                        return;
                    }
                }
                BurnPile.Add(PlayPile);
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

            CardsList DrawUntilFull()
            {
                if (DrawPile.Count == 0) { return new CardsList(); }
                if (CurrentPlayer.Hand.Count >= 3) { return new CardsList(); }
                int handStartSize = CurrentPlayer.Hand.Count;
                CardsList cardsDrawn = new ();
                for (int i = 0; i < 3 - handStartSize; i++) 
                {
                    if (DrawPile.Count == 0) { return cardsDrawn; }
                    cardsDrawn.Add(CurrentPlayer.DrawCard(DrawPile));
                }

                BoardEventSystem.TriggerPlayerDrawEvents(cardsDrawn.Count, CurrentPlayerIndex);

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
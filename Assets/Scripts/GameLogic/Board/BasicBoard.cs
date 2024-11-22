using KarmaLogic.Players;
using KarmaLogic.Cards;
using KarmaLogic.CardCombos;
using StateMachine;
using KarmaLogic.Board;
using KarmaLogic.Board.BoardEvents;
using KarmaLogic.Board.BoardPrinters;
using DataStructures;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.Playables;
using System;

namespace KarmaLogic
{
    namespace BasicBoard
    {
        public class BasicBoard : IBoard
        {
            public static BoardPrinterDebug BoardPrinterDefault = new();

            public List<Player> Players { get; set; }
            public CardPile DrawPile { get; protected set; }
            public CardPile BurnPile { get; protected set; }
            public PlayCardPile PlayPile { get; protected set; }
            public BoardEventSystem EventSystem { get; protected set; }
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
            public int WhichPlayerStartedGame {  get; protected set; }
            public int PlayerIndexWhoStartedTurn { get; protected set; }

            HashSet<BoardPlayerAction> _allActions;
            HashSet<int> _potentialWinnerIndices;

            public BasicBoard(BasicBoardParams basicBoardParams, IBoardPrinter boardPrinter = null)
            {
                List<Player> players = new();

                foreach (BasicBoardPlayerParams playerParams in basicBoardParams.PlayersParams)
                {
                    players.Add(playerParams.ToPlayer());
                }

                CardPile drawPile = new (basicBoardParams.DrawPileCards);
                PlayCardPile playPile = new (basicBoardParams.PlayPileCards);
                CardPile burnPile = new(basicBoardParams.BurnPileCards);

                SetInitParams(players, drawPile, burnPile, playPile, basicBoardParams.BoardTurnOrder, 
                    basicBoardParams.BoardPlayOrder, basicBoardParams.HandsAreFlipped, basicBoardParams.EffectMultiplier, 
                    basicBoardParams.WhoStarts, basicBoardParams.HasBurnedThisTurn, basicBoardParams.TurnsPlayed,
                    boardPrinter);
            }

            private void SetInitParams(List<Player> players, CardPile drawPile, CardPile burnPile, PlayCardPile playPile,
                BoardTurnOrder turnOrder = BoardTurnOrder.RIGHT, BoardPlayOrder playOrder = BoardPlayOrder.UP,
                bool handsAreFlipped = false, int effectMultiplier = 1, int whoStarts = 0,
                bool hasBurnedThisTurn = false, int turnsPlayed = 0, IBoardPrinter boardPrinter = null)
            {
                Players = players;

                foreach (Player player in players)
                {
                    player.Hand.Sort();
                }

                DrawPile = drawPile;
                BurnPile = burnPile;
                PlayPile = playPile;
                TurnOrder = turnOrder;
                PlayOrder = playOrder;
                HandsAreFlipped = handsAreFlipped;
                EffectMultiplier = effectMultiplier;
                WhichPlayerStartedGame = whoStarts;
                CurrentPlayerIndex = whoStarts;
                PlayerIndexWhoStartedTurn = whoStarts;
                HasBurnedThisTurn = hasBurnedThisTurn;
                TurnsPlayed = turnsPlayed;
                NumberOfCardsDrawnThisTurn = 0;

                CardValuesInPlayCounts = CountCardValuesInPlay();
                CardValuesTotalCounts = CountCardValuesTotal(CardValuesInPlayCounts);
                StartingPlayerStartedPlayingFrom = Players[whoStarts].PlayingFrom;

                BoardPrinter = boardPrinter is null ? BoardPrinterDefault : boardPrinter;
                ComboHistory = new List<CardCombo>();
                ComboFactory = new CardComboFactory();
                EventSystem = new BoardEventSystem();
                CurrentLegalActions = new HashSet<BoardPlayerAction>();
                CurrentLegalCombos = new LegalCombos();
                _allActions = new () {new PickupPlayPile(), new PlayCardsCombo()};
                _potentialWinnerIndices = new HashSet<int>();

                DrawUntilFullAllPlayers();

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
                EventSystem.TriggerHandsFlippedEvent(this);
                if (HandsAreFlipped)
                {
                    foreach (Player player in Players)
                    {
                        if (player.Hand.Count == 0) { continue; }
                        player.Hand.Shuffle();
                    }
                }
                else
                {
                    foreach (Player player in Players)
                    {
                        if (player.Hand.Count == 0) { continue; }
                        player.Hand.Sort();
                    }
                }
            }

            public void RotateHands(int numberOfRotations, Deque<Hand> hands)
            {
                if (numberOfRotations == 0) { return; }
                for (int i = 0; i < numberOfRotations; i++)
                {
                    hands.Rotate((int)TurnOrder);
                    EventSystem.TriggerHandsRotatedEventListener((int)TurnOrder, this);
                    for (int j = 0; j < Players.Count; j++)
                    {
                        Players[j].Hand = hands[j];
                    }
                }     
            }

            public void StartGivingAwayPlayPile(PlayPileGiveAwayHandler.InvalidFilter invalidFilter = null)
            {
                CurrentPlayer.PlayPileGiveAwayHandler = new PlayPileGiveAwayHandler(this, CurrentPlayerIndex, invalidFilter);
                EventSystem.TriggerStartedPlayPileGiveAway(CurrentPlayerIndex);
            }

            public void StartGivingAwayCards(int numberOfCards, CardGiveAwayHandler.InvalidFilter invalidFilter = null)
            {
                CurrentPlayer.CardGiveAwayHandler = new CardGiveAwayHandler(numberOfCards, this, CurrentPlayerIndex, invalidFilter);
                CurrentPlayer.CardGiveAwayHandler.RegisterOnCardGiveAwayListener(ReceiveCard);
                CurrentPlayer.CardGiveAwayHandler.RegisterOnFinishCardGiveAwayListener(EventSystem.TriggerOnFinishCardGiveAwayListenersWithTearDown);
                EventSystem.TriggerStartedCardGiveAway(numberOfCards, CurrentPlayerIndex);
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
                
                EventSystem.TriggerOnTurnStartEvents(this);
            }

            public void EndTurn()
            {
                TurnsPlayed++;
                EventSystem.TriggerOnTurnEndEvents(this);
            }

            public void PlayCards(CardsList cards)
            {
                PlayCards(cards, true);
            }

            public void PlayCards(CardsList cards, bool addToPile=true)
            {
                ComboFactory.SetCounts(cards);
                CardCombo cardCombo = ComboFactory.CreateCombo();
                List<bool> comboVisibility = ComboFactory.ComboVisibility(this);
                if (addToPile) { PlayPile.Add(cards, comboVisibility); }
                bool willBurnDueToMinimumRunFour = PlayPile.ContainsMinLengthRun(4);

                DrawUntilFull(CurrentPlayerIndex);

                if (NumberOfCombosPlayedThisTurn > 52) { return; }

                cardCombo.Apply(this);

                ResetEffectMultiplierIfNecessary(ComboFactory.ComboCardValue());
                NumberOfCombosPlayedThisTurn++;
                ComboHistory.Add(cardCombo);

                int jokerCount = PlayPile.CountValue(CardValue.JOKER);
                if (willBurnDueToMinimumRunFour || jokerCount > 0) 
                {
                    // Sometimes jokers can be played from the burn pile, then require re-burning.
                    Burn(jokerCount);
                }

                EventSystem.TriggerOnFinishPlaySuccesfulComboListenersWithTearDown();
                return;
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
                    EventSystem.TriggerBurnEvents(jokerCount);
                    CalculateLegalCombos(CurrentPlayer.PlayableCards);
                    return;
                }
                BurnPile.Add(PlayPile);
                
                CardValuesInPlayCounts.SubtractInPlace(PlayPile.CountAllCardValues());

                PlayPile.Clear();
                EventSystem.TriggerBurnEvents(jokerCount);
                CalculateLegalCombos(CurrentPlayer.PlayableCards);
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
                EventSystem.TriggerPlayerDrawEvents(cardsDrawn.Count, playerIndex);
                return cardsDrawn;
            }

            public void DrawUntilFullAllPlayers()
            {
                for (int playerIndex = 0; playerIndex < Players.Count; playerIndex++)
                {
                    DrawUntilFull(playerIndex);
                }
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
                    _potentialWinnerIndices.Clear();
                    if (DrawPile.Count > 0) { return _potentialWinnerIndices; }

                    for (int i = 0; i < Players.Count; i++)
                    {
                        {
                            Player player = Players[i];
                            if (!player.HasCards)
                            {
                                _potentialWinnerIndices.Add(i);
                            }
                        }
                    }
                    return _potentialWinnerIndices;
                }
            }

            public LegalCombos CurrentLegalCombos { get; protected set; }
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
                CurrentLegalCombos = GetLegalCombos(cards);
            }

            LegalCombos GetLegalCombos(CardsList cards)
            {
                if (cards.Count == 0) { return new LegalCombos(); }
                if (HandsAreFlipped) 
                {
                    return CardComboCalculator.HandFlippedCombos(cards);
                }

                bool jokersHaveAceValues = CardValuesInPlayCounts[CardValue.ACE] == 0;

                if (PlayPile.Count == 0 || PlayPile.VisibleTopCard is null)
                {
                    if (jokersHaveAceValues)
                    {
                        return CardComboCalculator.FillerCombos(cards, CardValue.SIX, 3);
                    }

                    return CardComboCalculator.FillerAndFilterCombos(cards, CardValue.SIX, CardComboCalculator.IsJoker, 3);
                }

                Card topCard = PlayPile.VisibleTopCard as Card;
                CardsList validCards = CardComboCalculator.PlayableCards(PlayOrder, cards, topCard.Value, CardValuesInPlayCounts);
                
                if (topCard.Value == CardValue.ACE || CardComboCalculator.ContainsPlayableJokersAsAceValues(jokersHaveAceValues, cards))
                {
                    if (CardComboCalculator.ContainsUnplayableFiller(PlayOrder, cards, topCard.Value))
                    {
                        return CardComboCalculator.FillerNotExclusiveCombos(validCards, CardValue.SIX, 3);
                    }
                    return CardComboCalculator.FillerCombos(validCards, CardValue.SIX, 3);
                }

                if (CardComboCalculator.ContainsUnplayableFiller(PlayOrder, cards, topCard.Value))
                {
                    return CardComboCalculator.FillerFilterNotExclusiveCombos(validCards, CardValue.SIX, CardComboCalculator.IsJoker, 3);
                }

                return CardComboCalculator.FillerAndFilterCombos(validCards, CardValue.SIX, CardComboCalculator.IsJoker, 3);
            }
        }
    }
}
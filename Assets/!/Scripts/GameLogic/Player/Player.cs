using System;
using System.Collections.Generic;
using DataStructures;
using KarmaLogic.Cards;

namespace KarmaLogic
{
    namespace Players
    {
        public class Player
        {
            public Hand Hand { get; set; }
            public CardsList KarmaDown { get; set; }
            public CardsList KarmaUp { get; set; }
            public CardGiveAwayHandler CardGiveAwayHandler { get; set; }
            public PlayPileGiveAwayHandler PlayPileGiveAwayHandler { get; set; }

            public delegate void OnSwapHandWithKarmaUpListener(int handIndex, int karmaUpIndex);
            event OnSwapHandWithKarmaUpListener OnSwapHandWithKarmaUp;

            public Player(Hand hand, CardsList karmaDown, CardsList karmaUp)
            {
                Hand = hand;
                KarmaDown = karmaDown;
                KarmaUp = karmaUp;
            }

            public Player(List<List<int>> playerMatrix, CardSuit cardSuit = null)
            {
                CardSuit suit = cardSuit;
                if (cardSuit is null)
                {
                    suit = CardSuit.DebugDefault;
                }

                Hand = new Hand(playerMatrix[0], suit);
                KarmaUp = new CardsList(playerMatrix[1], suit);
                KarmaDown = new CardsList(playerMatrix[2], suit);
            }

            public override string ToString()
            {
                return "Player(H" + Hand + ", UK" + KarmaUp + ", DK" + KarmaDown + ")";
            }

            public int Length {get { return Hand.Count + KarmaUp.Count + KarmaDown.Count; } }

            public bool HasCards { get { return Length > 0; } }
            public CardsList PlayableCards 
            { 
                get
                {
                    if (Hand.Count > 0)
                    {
                        return Hand;
                    }
                    if (KarmaUp.Count > 0)
                    {
                        return KarmaUp;
                    }
                    if (KarmaDown.Count > 0)
                    {
                        return KarmaDown;
                    }
                    return new CardsList();
                } 
            }

            public PlayingFrom PlayingFrom
            {
                get
                {
                    if (Hand.Count > 0)
                    {
                        return PlayingFrom.Hand;
                    }
                    if (KarmaUp.Count > 0)
                    {
                        return PlayingFrom.KarmaUp;
                    }
                    if (KarmaDown.Count > 0)
                    {
                        return PlayingFrom.KarmaDown;
                    }
                    return PlayingFrom.Empty;
                }
            }

            public void Pickup(CardsList cards)
            {
                Hand.Add(cards);
                cards.Clear();
            }

            public void ReceiveCard(Card card, Player giver)
            {
                Hand.Add(card);
                giver.PlayableCards.Remove(card);
            }

            public CardsList PopFromPlayable(int[] indices)
            {
                return PlayableCards.PopMultiple(indices);
            }

            public void SwapHandWithKarmaUp(int handIndex, int upKarmaIndex)
            {
                (Hand[handIndex], KarmaUp[upKarmaIndex]) = (KarmaUp[upKarmaIndex], Hand[handIndex]);
                OnSwapHandWithKarmaUp?.Invoke(handIndex, upKarmaIndex);
                Hand.Sort();
            }

            public Card DrawCard(CardsList cards)
            {
                Card cardDrawn = cards.Pop();
                Hand.Add(cardDrawn);
                return cardDrawn;
            }

            public DictionaryDefaultInt<CardValue> CountAllCardValues() {
                DictionaryDefaultInt<CardValue> counts = new();
                counts.UnionInPlace(Hand.CountAllCardValues());
                counts.UnionInPlace(KarmaUp.CountAllCardValues());
                counts.UnionInPlace(KarmaDown.CountAllCardValues());
                return counts;
            }

            public int CountValue(CardValue cardValue)
            { 
                int total = 0;
                if (Hand.Count > 0)
                {
                    total += Hand.CountValue(cardValue);
                }
                if (KarmaUp.Count > 0)
                {
                    total += KarmaUp.CountValue(cardValue);
                }
                if (KarmaDown.Count > 0)
                {
                    total += KarmaDown.CountValue(cardValue);
                }
                return total;
            }

            public void RegisterOnSwapHandWithPlayableEvent(OnSwapHandWithKarmaUpListener listener)
            {
                OnSwapHandWithKarmaUp += listener;
            }
        }

        public enum PlayingFrom: sbyte
        {
            Empty = -1,
            Hand = 0,
            KarmaUp = 1,
            KarmaDown = 2,
        }
    }
}

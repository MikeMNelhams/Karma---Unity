using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Karma.Cards;

namespace Karma
{
    namespace Players
    {
        public class Player
        {
            public Hand Hand { get; set; }
            public CardsList KarmaDown { get; set; }
            public CardsList KarmaUp { get; set; }

            public Player(Hand hand, CardsList karmaDown, CardsList karmaUp)
            {
                Hand = hand;
                this.KarmaDown = karmaDown;
                this.KarmaUp = karmaUp;
            }

            public Player(List<List<int>> playerMatrix)
            {
                CardSuit suit = CardSuit.Hearts;
                Hand = new Hand(playerMatrix[0], suit);
                KarmaUp = new CardsList(playerMatrix[1], suit);
                KarmaDown = new CardsList(playerMatrix[2], suit);
            }

            public override string ToString()
            {
                return "Player(H" + Hand + ", UK" + KarmaUp + ", DK" + KarmaDown;
            }

            public int Length {get { return Hand.Count + KarmaUp.Count + KarmaDown.Count; }
            }

            public bool HasCards { get { return Length > 0; } }
            public CardsList PlayableCards 
            { get
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

            public int PlayingFrom
            {
                get
                {
                    if (Hand.Count > 0)
                    {
                        return 0;
                    }
                    if (KarmaUp.Count > 0)
                    {
                        return 1;
                    }
                    if (KarmaDown.Count > 0)
                    {
                        return 2;
                    }
                    return -1;
                }
            }

            public void Pickup(CardsList cards)
            {
                Hand.Add(cards);
                cards.Clear();
            }

            public void ReceiveCard(Card card, Player player)
            {
                Hand.Add(card);
                player.PlayableCards.Remove(card);
            }

            public CardsList PopFromPlayable(int[] indices)
            {
                return PlayableCards.PopMultiple(indices);
            }

            public void SwapHandWithPlayable(int handIndex, int upKarmaIndex)
            {
                (Hand[handIndex], KarmaUp[upKarmaIndex]) = (KarmaUp[upKarmaIndex], Hand[handIndex]);
                Hand.Sort();
            }

            public Card DrawCard(CardsList cards)
            {
                Card cardDrawn = cards.Pop();
                Hand.Add(cardDrawn);
                return cardDrawn;
            }

            public void ShuffleHand()
            {
                Hand.Shuffle();
            }

            public int NumberOfJokers 
            { 
                get 
                {
                    int total = 0;
                    if (Hand.Count > 0)
                    {
                        total += Hand.CountValue(CardValue.JOKER);
                    }
                    if (KarmaUp.Count > 0)
                    {
                        total += KarmaUp.CountValue(CardValue.JOKER);
                    }
                    if (KarmaDown.Count > 0)
                    {
                        total += KarmaDown.CountValue(CardValue.JOKER);
                    }
                    return total;
                } 
            }
        }
    }
}

using Karma.Board;
using Karma.Cards;
using System.Collections.Generic;

namespace Karma
{
    namespace Controller
    {
        public interface IController
        {
            public int GiveAwayCardIndex(HashSet<int> excludedCardsIndices);
            public int GiveAwayPlayerIndex(HashSet<int> excludedPlayerIndices);
            public int JokerTargetIndex(HashSet<int> excludedPlayerIndices);
            public bool WantsToMulligan();
            public int MulliganHandIndex();
            public int MulliganKarmaUpIndex();
            public BoardTurnOrder ChooseStartDirection();
            public BoardPlayerAction SelectAction(IBoard board);
            public CardsList SelectCardsToPlay();
            public int VoteForWinner(HashSet<int> excludedPlayerIndices);
        }
    }
}

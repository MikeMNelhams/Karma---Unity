using KarmaLogic.BasicBoard;
using KarmaLogic.Board;

namespace KarmaPlayerMode
{
    public abstract class KarmaPlayModeBoardPreset<IBoard>
    {
        public KarmaPlayModeBoardPreset()
        {

        }

        public abstract BasicBoardParams BoardParams { get; }
        public abstract int TurnLimit { get; }
    }
}

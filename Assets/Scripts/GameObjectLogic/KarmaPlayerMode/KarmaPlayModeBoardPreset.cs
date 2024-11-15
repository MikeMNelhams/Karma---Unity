using KarmaLogic.Board;

namespace KarmaPlayerMode
{
    public abstract class KarmaPlayModeBoardPreset<IBoard>
    {
        public KarmaPlayModeBoardPreset()
        {

        }

        public abstract IBoard Board { get; }
        public abstract int TurnLimit { get; }
    }
}

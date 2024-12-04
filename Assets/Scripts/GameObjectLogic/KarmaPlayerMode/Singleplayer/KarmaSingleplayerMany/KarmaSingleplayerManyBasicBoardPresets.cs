using KarmaLogic.BasicBoard;

namespace KarmaPlayerMode.Singleplayer
{
    public class PlayRandomStartFourPlayable : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public PlayRandomStartFourPlayable() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardFactory.RandomStartAllPlayable(4);

        public override int TurnLimit => 1000;
    }
}


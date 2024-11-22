using KarmaLogic.BasicBoard;

namespace KarmaPlayerMode
{
    namespace Singleplayer
    {
        public class TestStartQueenCombo : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestStartQueenCombo() : base()
            {
            }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotQueenCombo();

            public override int TurnLimit => 100 ;
        }

        public class TestStartJokerCombo : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestStartJokerCombo() : base() 
            {
            }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotJokerCombo();

            public override int TurnLimit => 100;
        }

        public class TestStartVoting : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestStartVoting() : base()
            {
            }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotVotingTestBoard();

            public override int TurnLimit => 100;
        }

        public class TestStartVoting2 : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestStartVoting2() : base()
            {
            }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotVotingTestBoard2();

            public override int TurnLimit => 100;
        }

        public class TestScenarioFullHand : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestScenarioFullHand() : base() { }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotTestScenario1();
            public override int TurnLimit => 100;
        }
    }
}


using KarmaLogic.BasicBoard;

namespace KarmaPlayerMode
{
    namespace Singleplayer
    {
        public class RandomStart4Player : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public RandomStart4Player() : base()
            {
            }

            public override BasicBoard Board => BoardFactory.RandomStart(4);

            public override int TurnLimit => 200;
        }

        public class TestStartQueenCombo : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestStartQueenCombo() : base()
            {
            }

            public override BasicBoard Board => BoardTestFactory.BotQueenCombo();

            public override int TurnLimit => 100 ;
        }

        public class TestStartJokerCombo : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestStartJokerCombo() : base() 
            {
            }

            public override BasicBoard Board => BoardTestFactory.BotJokerCombo();

            public override int TurnLimit => 100;
        }

        public class TestStartVoting : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestStartVoting() : base()
            {
            }

            public override BasicBoard Board => BoardTestFactory.BotVotingTestBoard();

            public override int TurnLimit => 100;
        }

        public class TestStartVoting2 : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestStartVoting2() : base()
            {
            }

            public override BasicBoard Board => BoardTestFactory.BotVotingTestBoard2();

            public override int TurnLimit => 100;
        }

        public class TestScenario1 : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestScenario1() : base() { }

            public override BasicBoard Board => BoardTestFactory.BotTestScenario1();
            public override int TurnLimit => 100;
        }
    }
}


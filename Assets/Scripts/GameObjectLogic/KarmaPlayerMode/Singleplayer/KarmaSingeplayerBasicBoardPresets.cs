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

            public override BasicBoardParams BoardParams => BoardTestFactory.BotVotingTestBoard1();

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

            public override BasicBoardParams BoardParams => BoardTestFactory.BotTestFullHand();
            public override int TurnLimit => 100;
        }

        public class TestLeftHandRotate : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestLeftHandRotate() : base() { }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotTestLeftHandRotate();

            public override int TurnLimit => 20;
        }

        public class TestGameWonNoVoting : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestGameWonNoVoting() : base() { }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotTestGameWonNoVote();

            public override int TurnLimit => 100;
        }

        public class TestPotentialWinnerIsSkippedInUnwonGame : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestPotentialWinnerIsSkippedInUnwonGame() : base() { }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotTestPotentialWinnerIsSkippedInUnwonGame();

            public override int TurnLimit => 100;
        }

        public class TestMultipleSeparateCardGiveaways : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestMultipleSeparateCardGiveaways() : base() { }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotTestMultipleSeparateCardGiveaways();

            public override int TurnLimit => 100;
        }

        public class TestQueenComboLastCardToWin : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestQueenComboLastCardToWin() : base() { }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotTestQueenComboLastCardToWin();

            public override int TurnLimit => 100;
        }

        public class TestQueenComboLastCardWithJokerInPlay : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestQueenComboLastCardWithJokerInPlay() : base() { }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotTestQueenComboLastCardWithJokerInPlay();

            public override int TurnLimit => 50;
        }

        public class TestValidJokerAsLastCardToWin : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestValidJokerAsLastCardToWin() : base() { }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotTestValidJokerAsLastCardToWin();

            public override int TurnLimit => 100;
        }

        public class TestGettingJokered : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestGettingJokered() : base() { }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotTestGettingJokered();

            public override int TurnLimit => 100;
        }

        public class TestJokerAsAceLastCardToWin : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestJokerAsAceLastCardToWin() : base() { }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotTestJokerAsAceLastCardToWin();

            public override int TurnLimit => 100;
        }

        public class TestAllPlayersNoActionsGameEnds : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public TestAllPlayersNoActionsGameEnds() : base() { }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotTestAllPlayersNoActionsGameEnds();

            public override int TurnLimit => 100;
        }

        public class TestRandomStart : KarmaPlayModeBoardPreset<BasicBoard>
        {

            public TestRandomStart() : base() { }

            public override BasicBoardParams BoardParams => BoardTestFactory.BotTestRandomStart(4);
            public override int TurnLimit => 100;
        }

        public class PlayRandomStart : KarmaPlayModeBoardPreset<BasicBoard>
        {
            public PlayRandomStart() : base() { }

            public override BasicBoardParams BoardParams => BoardFactory.RandomStart(4);

            public override int TurnLimit => 1000;
        }
    }
}


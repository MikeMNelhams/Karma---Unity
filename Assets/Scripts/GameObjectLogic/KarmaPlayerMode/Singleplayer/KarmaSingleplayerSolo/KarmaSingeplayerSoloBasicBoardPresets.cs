using KarmaLogic.BasicBoard;

namespace KarmaPlayerMode.Singleplayer
{
    public class TestStartQueenCombo : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestStartQueenCombo() : base()
        {
        }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotQueenCombo();

        public override int TurnLimit => 100 ;
    }

    public class TestStartJokerCombo : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestStartJokerCombo() : base() 
        {
        }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotJokerCombo();

        public override int TurnLimit => 100;
    }

    public class TestStartVoting : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestStartVoting() : base()
        {
        }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotVotingTestBoard1();

        public override int TurnLimit => 100;
    }

    public class TestStartVoting2 : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestStartVoting2() : base()
        {
        }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotVotingTestBoard2();

        public override int TurnLimit => 100;
    }

    public class TestScenarioFullHand : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestScenarioFullHand() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestFullHand();
        public override int TurnLimit => 100;
    }

    public class TestLeftHandRotate : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestLeftHandRotate() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestLeftHandRotate();

        public override int TurnLimit => 20;
    }

    public class TestGameWonNoVoting : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestGameWonNoVoting() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestGameWonNoVote();

        public override int TurnLimit => 100;
    }

    public class TestPotentialWinnerIsSkippedInUnwonGame : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestPotentialWinnerIsSkippedInUnwonGame() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestPotentialWinnerIsSkippedInUnwonGame();

        public override int TurnLimit => 100;
    }

    public class TestMultipleSeparateCardGiveaways : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestMultipleSeparateCardGiveaways() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestMultipleSeparateCardGiveaways();

        public override int TurnLimit => 100;
    }

    public class TestQueenComboLastCardToWin : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestQueenComboLastCardToWin() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestQueenComboLastCardToWin();

        public override int TurnLimit => 100;
    }

    public class TestQueenComboLastCardWithJokerInPlay : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestQueenComboLastCardWithJokerInPlay() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestQueenComboLastCardWithJokerInPlay();

        public override int TurnLimit => 50;
    }

    public class TestValidJokerAsLastCardToWin : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestValidJokerAsLastCardToWin() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestValidJokerAsLastCardToWin();

        public override int TurnLimit => 100;
    }

    public class TestGettingJokered : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestGettingJokered() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestGettingJokered();

        public override int TurnLimit => 100;
    }

    public class TestJokerAsAceLastCardToWin : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestJokerAsAceLastCardToWin() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestJokerAsAceLastCardToWin();

        public override int TurnLimit => 100;
    }

    public class TestAllPlayersNoActionsGameEnds : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestAllPlayersNoActionsGameEnds() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestAllPlayersNoActionsGameEnds();

        public override int TurnLimit => 100;
    }

    public class TestAceNoHandDoesNotCrash : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestAceNoHandDoesNotCrash() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestAceNoHandDoesNotCrash();

        public override int TurnLimit => 100;
    }

    public class TestAceAndFive : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestAceAndFive() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestAceAndFive();

        public override int TurnLimit => 100;
    }

    public class TestJackOnNine : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestJackOnNine() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestJackOnNine();

        public override int TurnLimit => 1;
    }

    public class TestJackOnQueen : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public TestJackOnQueen() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestJackOnQueen();

        public override int TurnLimit => 1;
    }

    public class TestRandomStart : KarmaPlayModeBoardPreset<BasicBoard>
    {

        public TestRandomStart() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardTestFactory.BotTestRandomStart(4);
        public override int TurnLimit => 100;
    }

    public class PlayRandomStartDefault : KarmaPlayModeBoardPreset<BasicBoard>
    {
        public PlayRandomStartDefault() : base() { }

        public override BasicBoardParams BoardParams => BasicBoardFactory.RandomStartSingleplayerDefault();

        public override int TurnLimit => 1_000;
    }
}


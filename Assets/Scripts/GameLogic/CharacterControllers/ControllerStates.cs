using KarmaLogic.Board;


namespace KarmaLogic
{
    namespace Controller
    {

        public class WaitForTurn : ControllerState
        {
            public WaitForTurn(IBoard board, BaseCharacterProperties playerProperties) : base(board, playerProperties) { }
            public override void OnEnter()
            {
                _playerProperties.EnterWaitingForTurn();
            }

            public override void OnExit()
            {
                _playerProperties.ExitWaitingForTurn();
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        public class PickingAction : ControllerState
        {
            public PickingAction(IBoard board, BaseCharacterProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                _board.PrintChooseableCards();
                _playerProperties.EnterPickingAction();
            }

            public override void OnExit()
            {
                _playerProperties.ExitPickingAction();
            }

            public override int GetHashCode()
            {
                return 1;
            }
        }

        public class VotingForWinner : ControllerState
        {
            public VotingForWinner(IBoard board, BaseCharacterProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                _playerProperties.EnterVotingForWinner();
            }

            public override void OnExit()
            {
                _playerProperties.ExitVotingForWinner();
            }

            public override int GetHashCode()
            {
                return 2;
            }
        }

        public class SelectingCardGiveAwaySelectionIndex : ControllerState
        {
            public SelectingCardGiveAwaySelectionIndex(IBoard board, BaseCharacterProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                _playerProperties.EnterCardGiveAwaySelection();
            }

            public override void OnExit()
            {
                _playerProperties.ExitCardGiveAwaySelection();
            }

            public override int GetHashCode()
            {
                return 3;
            }
        }

        public class SelectingCardGiveAwayPlayerIndex : ControllerState
        {
            public SelectingCardGiveAwayPlayerIndex(IBoard board, BaseCharacterProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                _playerProperties.EnterCardGiveAwayPlayerIndexSelection();
            }

            public override void OnExit()
            {
                _playerProperties.ExitCardGiveAwayPlayerIndexSelection();
            }

            public override int GetHashCode()
            {
                return 4;
            }
        }

        public class SelectingPlayPileGiveAwayPlayerIndex : ControllerState
        {
            public SelectingPlayPileGiveAwayPlayerIndex(IBoard board, BaseCharacterProperties playerProperties) : base(board, playerProperties) { }

            public override void OnEnter()
            {
                _playerProperties.EnterPlayPileGiveAwayPlayerIndexSelection();
            }

            public override void OnExit()
            {
                _playerProperties.ExitPlayPileGiveAwayPlayerIndexSelection();
            }

            public override int GetHashCode()
            {
                return 5;
            }
        }
    }
}
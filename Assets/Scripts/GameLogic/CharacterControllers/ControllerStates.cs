using KarmaLogic.Board;


namespace KarmaLogic
{
    namespace Controller
    {

        public class NullState : ControllerState
        {
            public NullState(IBoard board, ICharacterProperties characterProperties) : base(board, characterProperties) { }
            public override void OnEnter() { }

            public override void OnExit() { }

            public override int GetHashCode()
            {
                return -1;
            }
        }

        public class WaitForTurn : ControllerState
        {
            public WaitForTurn(IBoard board, ICharacterProperties characterProperties) : base(board, characterProperties) { }
            public override void OnEnter()
            {
                _characterProperties.Controller.EnterWaitingForTurn(_board, _characterProperties);
            }

            public override void OnExit()
            {
                _characterProperties.Controller.ExitWaitingForTurn(_board, _characterProperties);
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        public class PickingAction : ControllerState
        {
            public PickingAction(IBoard board, ICharacterProperties characterProperties) : base(board, characterProperties) { }

            public override void OnEnter()
            {
                _board.PrintChooseableCards();
                _characterProperties.Controller.EnterPickingAction(_board, _characterProperties);
            }

            public override void OnExit()
            {
                _characterProperties.Controller.ExitPickingAction(_board, _characterProperties);
            }

            public override int GetHashCode()
            {
                return 1;
            }
        }

        public class VotingForWinner : ControllerState
        {
            public VotingForWinner(IBoard board, ICharacterProperties characterProperties) : base(board, characterProperties) { }

            public override void OnEnter()
            {
                _characterProperties.Controller.EnterVotingForWinner(_board, _characterProperties);
            }

            public override void OnExit()
            {
                _characterProperties.Controller.ExitVotingForWinner(_board, _characterProperties);
            }

            public override int GetHashCode()
            {
                return 2;
            }
        }

        public class SelectingCardGiveAwaySelectionIndex : ControllerState
        {
            public SelectingCardGiveAwaySelectionIndex(IBoard board, ICharacterProperties characterProperties) : base(board, characterProperties) { }

            public override void OnEnter()
            {
                _characterProperties.Controller.EnterCardGiveAwaySelection(_board, _characterProperties);
            }

            public override void OnExit()
            {
                _characterProperties.Controller.ExitCardGiveAwaySelection(_board, _characterProperties);
            }

            public override int GetHashCode()
            {
                return 3;
            }
        }

        public class SelectingCardGiveAwayPlayerIndex : ControllerState
        {
            public SelectingCardGiveAwayPlayerIndex(IBoard board, ICharacterProperties characterProperties) : base(board, characterProperties) { }

            public override void OnEnter()
            {
                _characterProperties.Controller.EnterCardGiveAwayPlayerIndexSelection(_board, _characterProperties);
            }

            public override void OnExit()
            {
                _characterProperties.Controller.ExitCardGiveAwayPlayerIndexSelection(_board, _characterProperties);
            }

            public override int GetHashCode()
            {
                return 4;
            }
        }

        public class SelectingPlayPileGiveAwayPlayerIndex : ControllerState
        {
            public SelectingPlayPileGiveAwayPlayerIndex(IBoard board, ICharacterProperties characterProperties) : base(board, characterProperties) { }

            public override void OnEnter()
            {
                _characterProperties.Controller.EnterPlayPileGiveAwayPlayerIndexSelection(_board, _characterProperties);
            }

            public override void OnExit()
            {
                _characterProperties.Controller.ExitPlayPileGiveAwayPlayerIndexSelection(_board, _characterProperties);
            }

            public override int GetHashCode()
            {
                return 5;
            }
        }
    }
}
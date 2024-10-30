using KarmaLogic.Board;
using KarmaLogic.Controller;

public class PlayerController : Controller
{
    public override void EnterWaitingForTurn(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.DisableCamera();
        characterProperties.HideUI();
    }

    public override void ExitWaitingForTurn(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.EnableCamera();
    }

    public override void EnterCardGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.EnterCardGiveAwayPlayerIndexSelection();
    }

    public override void EnterCardGiveAwaySelection(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.EnterCardGiveAwaySelection();
    }

    public override void EnterPickingAction(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.EnterPickingActionUpdateUI();
    }

    public override void EnterPlayPileGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.EnterPlayPileGiveAwayPlayerIndexSelection();
    }

    public override void EnterVotingForWinner(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.EnterVotingForWinner();
    }

    public override void ExitVotingForWinner(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.ExitVotingForWinner();
    }

    public override void ExitCardGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.ExitCardGiveAwayPlayerIndexSelection();
    }

    public override void ExitCardGiveAwaySelection(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.ExitCardGiveAwaySelection();
    }

    public override void ExitPickingAction(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.ExitPickingActionUpdateUI();
    }

    public override void ExitPlayPileGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.ExitPlayPileGiveAwayPlayerIndexSelection();
    }
}
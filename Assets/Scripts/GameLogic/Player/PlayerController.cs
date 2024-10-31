using KarmaLogic.Board;
using KarmaLogic.Controller;
using System.Threading.Tasks;

public class PlayerController : Controller
{
    public override Task EnterWaitingForTurn(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.DisableCamera();
        characterProperties.HideUI();
        return Task.CompletedTask;
    }

    public override Task ExitWaitingForTurn(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.EnableCamera();
        return Task.CompletedTask;
    }

    public override async Task EnterCardGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        await characterProperties.EnterCardGiveAwayPlayerIndexSelection();
    }

    public override async Task EnterCardGiveAwaySelection(IBoard board, ICharacterProperties characterProperties)
    {
        await characterProperties.EnterCardGiveAwaySelection();
    }

    public override Task EnterPickingAction(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.EnterPickingActionUpdateUI();
        return Task.CompletedTask;
    }

    public override async Task EnterPlayPileGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        await characterProperties.EnterPlayPileGiveAwayPlayerIndexSelection();
    }

    public override Task EnterVotingForWinner(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.EnterVotingForWinner();
        return Task.CompletedTask;
    }

    public override Task ExitVotingForWinner(IBoard board, ICharacterProperties characterProperties)
    {
        characterProperties.ExitVotingForWinner();
        return Task.CompletedTask;
    }

    public override async Task ExitCardGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        await characterProperties.ExitCardGiveAwayPlayerIndexSelection();
    }

    public override async Task ExitCardGiveAwaySelection(IBoard board, ICharacterProperties characterProperties)
    {
        await characterProperties.ExitCardGiveAwaySelection();
    }

    public override async Task ExitPickingAction(IBoard board, ICharacterProperties characterProperties)
    {
        await characterProperties.ExitPickingActionUpdateUI();
    }

    public override async Task ExitPlayPileGiveAwayPlayerIndexSelection(IBoard board, ICharacterProperties characterProperties)
    {
        await characterProperties.ExitPlayPileGiveAwayPlayerIndexSelection();
    }
}
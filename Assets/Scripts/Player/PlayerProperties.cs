using Karma.Board;
using Karma.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProperties : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject head;
    public GameObject cardHolder;

    public Button confirmSelectionButton;
    public Button pickupPlayPileButton;

    public IController Controller {  get; set; }
    public CardSelector CardSelector { get; protected set; }

    Transform headTransform;

    // Start is called before the first frame update
    void Awake()
    {
        CardSelector = new();
    }

    public void EnableCamera()
    {
        playerCamera.enabled = true;
        playerCamera.tag = "MainCamera";
    }

    public void DisableCamera()
    {
        playerCamera.tag = "Untagged";
        playerCamera.enabled = false;
    }

    public void HideUI()
    {
        confirmSelectionButton.gameObject.SetActive(false);
        pickupPlayPileButton.gameObject.SetActive(false);
    }

    public void EnterPickingActionMode()
    {
        KarmaGameManager gameManager = KarmaGameManager.Instance;
        HashSet<BoardPlayerAction> legalActions = gameManager.Board.CurrentLegalActions;
        if (legalActions.Count == 0) { return; }
        if (gameManager.Board.CurrentLegalActions.Contains(gameManager.PickUpAction)) { pickupPlayPileButton.gameObject.SetActive(true); }
        if (gameManager.Board.CurrentLegalActions.Contains(gameManager.PlayCardsComboAction)) { confirmSelectionButton.gameObject.SetActive(true); }
    }

    public void ExitPickingActionMode()
    {
        pickupPlayPileButton.gameObject.SetActive(false);
    }

    public void EnterVotingForWinnerMode()
    {

    }

    public void ExitVotingForWinnerMode()
    {

    }

    public void SetControllerState(ControllerState newState)
    {
        Controller.SetState(newState);
    }
}

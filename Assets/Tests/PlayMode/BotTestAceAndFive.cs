using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class BotTestAceAndFive : MonoBehaviour
{
    [UnitySetUp]
    public IEnumerator LoadKGM()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/!/Scenes/Main Scene.unity");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestMain()
    {
        MenuUIManager.Instance.MenuCamera.enabled = false;
        KarmaGameManager.Instance.SetPlayerMode(KarmaPlayerMode.PlayerMode.Singleplayer);
        KarmaGameManager.Instance.SetPlayerSubMode(0);
        KarmaGameManager.Instance.SetIsUsingBoardPresets(true);
        KarmaGameManager.Instance.SetSelectedBoardPreset(16);
        KarmaGameManager.Instance.GlobalBotDelayInSeconds = 0.001f;

        KarmaGameManager.Instance.BeginGame();

        yield return new WaitForSeconds(2);

        Assert.IsTrue(KarmaGameManager.Instance.SelectedKarmaPlayerMode.IsGameOver);

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator FinishTearDown()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/!/Scenes/EmptyScene.unity");
        yield return null;
    }
}

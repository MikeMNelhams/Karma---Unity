using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine.SceneManagement;

public class BotJokerComboTest : MonoBehaviour
{
    [UnitySetUp]
    public IEnumerator LoadKGM()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/!/Scenes/Main Scene.unity");
        SceneManager.LoadScene("Main Scene");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestMain()
    {
        MenuUIManager.Instance.MenuCamera.enabled = false;
        KarmaGameManager.Instance.SetPlayerMode(KarmaPlayerMode.PlayerMode.Singleplayer);
        KarmaGameManager.Instance.SetPlayerSubMode(0);
        KarmaGameManager.Instance.SetIsUsingBoardPresets(true);
        KarmaGameManager.Instance.SetSelectedBoardPreset(1);
        KarmaGameManager.Instance.GlobalBotDelayInSeconds = 0.001f;

        KarmaGameManager.Instance.BeginGame();

        yield return new WaitForSeconds(2);

        Assert.IsTrue(KarmaGameManager.Instance.SelectedKarmaPlayerMode.IsGameOver);
        Assert.IsFalse(KarmaGameManager.Instance.SelectedKarmaPlayerMode.IsGameWon);

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator FinishTearDown()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/!/Scenes/EmptyScene.unity");
        yield return null;
    }
}

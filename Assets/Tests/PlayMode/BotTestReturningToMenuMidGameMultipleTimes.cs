using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

public class BotTestReturningToMenuMidGameMultipleTimes : MonoBehaviour
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

        yield return new WaitForSeconds(1.5f);

        KarmaGameManager.Instance.EndCurrentGame();

        yield return new WaitForSeconds(2f);

        KarmaGameManager.Instance.BeginGame();

        yield return new WaitForSeconds(1f);

        KarmaGameManager.Instance.EndCurrentGame();

        Assert.IsTrue(MenuUIManager.Instance.ActivePageCount == 1);

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator FinishTearDown()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/!/Scenes/EmptyScene.unity");
        yield return null;
    }
}


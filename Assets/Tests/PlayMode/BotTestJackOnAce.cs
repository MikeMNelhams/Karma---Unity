using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using KarmaLogic.Cards;

public class BotTestJackOnAce : MonoBehaviour
{
    [UnitySetUp]
    public IEnumerator LoadKGM()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/Scenes/Main Scene.unity");
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
        KarmaGameManager.Instance.SetSelectedBoardPreset(19);
        KarmaGameManager.Instance.GlobalBotDelayInSeconds = 0.001f;

        KarmaGameManager.Instance.BeginGame();
        Dictionary<int, int> gameRanksExpected = new()
        {
            { 2, 0 },
            { 0, 1 },
            { 1, 2 },
            { 3, 2 }
        };

        yield return new WaitForSeconds(1.5f);

        Dictionary<int, int> gameRanks = KarmaGameManager.Instance.SelectedKarmaPlayerMode.GameRanks;
        UnityEngine.Debug.Log("Game ranks: " + gameRanks);

        Assert.IsTrue(KarmaGameManager.Instance.SelectedKarmaPlayerMode.IsGameOver);
        Assert.IsFalse(KarmaGameManager.Instance.SelectedKarmaPlayerMode.IsGameWon);
        Assert.IsTrue(gameRanks.Count != 0);

        CollectionAssert.AreEqual(gameRanksExpected.Keys, gameRanks.Keys);
        Dictionary<int, int> difference = gameRanksExpected.Where(x => gameRanks[x.Key] != x.Value).ToDictionary(x => x.Key, x => x.Value);
        Assert.IsTrue(difference.Count == 0);

        Assert.IsTrue(KarmaGameManager.Instance.Board.HandsAreFlipped);

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator FinishTearDown()
    {
        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Assets/Scenes/Main Scene.unity");
        yield return null;
    }
}

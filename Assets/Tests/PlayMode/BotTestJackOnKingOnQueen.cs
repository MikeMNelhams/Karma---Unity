using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using KarmaLogic.Cards;

public class BotTestJackOnKingOnQueen : MonoBehaviour
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
        KarmaGameManager.Instance.SetSelectedBoardPreset(20);
        KarmaGameManager.Instance.GlobalBotDelayInSeconds = 0.001f;

        KarmaGameManager.Instance.BeginGame();
        Dictionary<int, int> gameRanksExpected = new()
        {
            { 0, 0 },
            { 2, 0 },
            { 3, 1 },
            { 1, 2 }
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

        Assert.IsNull(KarmaGameManager.Instance.Board.Players[0].CardGiveAwayHandler);

        List<CardValue> values = KarmaGameManager.Instance.Board.ComboHistory[^1].Cards.CardValues;
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(values.First(), CardValue.QUEEN);

        values = KarmaGameManager.Instance.Board.ComboHistory[^2].Cards.CardValues;
        Assert.AreEqual(values.Count, 1);
        Assert.AreEqual(values.First(), CardValue.KING);

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator FinishTearDown()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Assets/!/Scenes/EmptyScene.unity");
        yield return null;
    }
}

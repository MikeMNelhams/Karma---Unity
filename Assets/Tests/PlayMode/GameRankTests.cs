using System.Collections;
using NUnit.Framework;
using KarmaLogic.BasicBoard;
using KarmaLogic.Board;
using KarmaLogic.Cards;
using UnityEngine;
using UnityEngine.TestTools;

using UnityEngine.SceneManagement;

public class GameRankTests 
{

    [UnityTest]
    public IEnumerator GameLaunches()
    {
        // TODO allow board starts through a public variable to KarmaGameManager(), then plug in the board state u want.
        yield return new WaitForSeconds(0.1f);
    }
}

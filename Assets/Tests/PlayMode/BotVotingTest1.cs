using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using KarmaLogic.GameExceptions;

public class BotVotingTest1 : MonoBehaviour
{
    [UnitySetUp]
    public IEnumerator Setup()
    {
        KarmaGameManager.Instance.SetSelectedBoardPreset(3); 

        yield return null;
    }


    [UnityTest]
    public IEnumerator TestVotesCorrectly()
    {
        // TODO 
        // Assert.Throws<GameWonException>()
        yield return null;
    }
}

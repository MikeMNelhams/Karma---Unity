using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using DataStructures;

public class LWCCTests
{
    [Test]
    public void StartsEmpty()
    {
        ListWithConstantContainsCheck<int> testNumbers = new();
        Assert.AreEqual(0, testNumbers.Count);
    }

    [Test]
    public void EmptyNotContains()
    {
        ListWithConstantContainsCheck<int> testNumbers = new();
        Assert.False(testNumbers.Contains(0));
    }

    [Test]
    public void AddContains()
    {
        ListWithConstantContainsCheck<int> testNumbers = new() { 1 };
        Assert.True(testNumbers.Contains(1));
        Assert.False(testNumbers.Contains(0));
    }

    [Test]
    public void AddGetRemoveIsEmpty()
    {
        ListWithConstantContainsCheck<int> testNumbers = new()
        {
            1
        };

        Assert.AreEqual(1, testNumbers[0]);

        testNumbers.Remove(1);
        Assert.AreEqual(0, testNumbers.Count);
    }

    [Test]
    public void MultiAddRemoveRemoveAddCorrect()
    {
        ListWithConstantContainsCheck<int> testNumbers = new() { 1, 2, 3, 4 };

        testNumbers.Remove(2);
        testNumbers.Remove(1);
        testNumbers.Remove(4);

        testNumbers.Add(5);
        Assert.AreEqual(new ListWithConstantContainsCheck<int>() { 3, 5 }, testNumbers);
    }
}

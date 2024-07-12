using DataStructures;
using Karma.Board;
using Karma.Cards;
using Karma.Controller;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : IController
{
    public bool IsAwaitingInput {  get; set; }
    public BoardPlayerAction SelectedAction { get; set; }

    public PlayerController() 
    { 
        IsAwaitingInput = false;
    }
}
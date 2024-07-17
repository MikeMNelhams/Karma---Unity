using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Karma
{
    namespace Board
    {
        namespace BoardPrinters
        {
            public class BoardPrinterDebug : IBoardPrinter
            {
                public void PrintBoard(IBoard board)
                {
                    string gameState = "Draw Pile: " + board.DrawPile + "\n";
                    gameState += "Play Pile: " + board.PlayPile + "\n";
                    gameState += "Burn Pile: " + board.PlayPile + "\n";
                    gameState += BoardPrintingLibrary.ComboHistoryRepresentation(board);
                    UnityEngine.Debug.Log(PlayersStateRepresentation(board) + "\n" + gameState);
                }

                public void PrintChoosableCards(IBoard board)
                {
                    string message = "Legal moves from: " + board.CurrentPlayer.PlayableCards + ":\n";
                    message += BoardPrintingLibrary.CombosRepresentation(board.CurrentLegalCombos);
                    UnityEngine.Debug.Log(message);
                }

                string PlayersStateRepresentation(IBoard board)
                {
                    List<string> playersRepresentation = BoardPrintingLibrary.PlayersRepresentationDebug(board);
                    return string.Join('\n', playersRepresentation);
                }
            }
        }
    }
}
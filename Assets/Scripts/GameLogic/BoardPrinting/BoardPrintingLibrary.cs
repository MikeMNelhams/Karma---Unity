using DataStructures;
using KarmaLogic.Board;
using KarmaLogic.CardCombos;
using KarmaLogic.Cards;
using KarmaLogic.Players;
using System.Collections.Generic;

namespace KarmaLogic
{
    namespace Board
    {
        public class BoardPrintingLibrary
        {
            public static List<string> PlayersRepresentationDebug(IBoard board)
            {
                List<string> representations = new();
                foreach (Player player in board.Players)
                {
                    representations.Add(PlayerRepresentationDebug(player));
                }
                return representations;
            }

            public static string PlayerRepresentationDebug(Player player)
            {
                return "Player(" + player.Hand + ", " + player.KarmaUp + ", " + player.KarmaDown + ")";
            }

            public static string ComboHistoryRepresentation(IBoard board)
            {
                string comboHistory = "Combo History: [";
                foreach (var combo in board.ComboHistory)
                {
                    comboHistory += combo + ", ";
                }
                return comboHistory + "]";
            }

            public static string CombosRepresentation(LegalCombos combos)
            {
                string combosMessage = "Combos[";
                foreach (var combo in combos)
                {
                    combosMessage += combo + ", ";
                }
                return combosMessage + "]";
            }
        }
    }
}
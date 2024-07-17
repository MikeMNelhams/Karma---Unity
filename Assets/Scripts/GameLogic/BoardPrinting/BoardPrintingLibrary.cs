using DataStructures;
using Karma.Board;
using Karma.Cards;
using Karma.Players;
using System.Collections.Generic;

namespace Karma
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

            public static string CombosRepresentation(HashSet<FrozenMultiSet<CardValue>> combos)
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
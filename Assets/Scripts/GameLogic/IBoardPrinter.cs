using Karma.Board;
using System.Collections;
using System.Collections.Generic;

namespace Karma
{
    namespace Board
    {
        namespace BoardPrinters
        {
            public interface IBoardPrinter
            {
                public void PrintBoard(IBoard board);
                public void PrintChoosableCards(IBoard board);
            }
        }
    }
}
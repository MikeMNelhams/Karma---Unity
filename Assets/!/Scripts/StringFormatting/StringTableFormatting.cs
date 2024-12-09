namespace StringFormatting 
{
    public class StringTableFormatting
    {
        public const char DEFAULT_ROW_CHAR = '-';
        public const int DEFAULT_NUMBER_OF_ROW_CHARS = 50;

        public static string AddRowAbove(string message, int numberOfCharacters = DEFAULT_NUMBER_OF_ROW_CHARS, char character = DEFAULT_ROW_CHAR)
        {
            return Row(numberOfCharacters, character) + "\n" + message;
        }

        public static string AddRowBelow(string message, int numberOfCharacters = DEFAULT_NUMBER_OF_ROW_CHARS, char character = DEFAULT_ROW_CHAR)
        {
            return message + "\n" + Row(numberOfCharacters, character);
        }

        public static string AddTopAndBottomRows(string message, int numberOfCharacters = DEFAULT_NUMBER_OF_ROW_CHARS, char character = DEFAULT_ROW_CHAR)
        {
            return AddRowAbove(AddRowBelow(message, numberOfCharacters, character), numberOfCharacters, character);
        }

        public static string Row(int numberOfCharacters = DEFAULT_NUMBER_OF_ROW_CHARS, char character = DEFAULT_ROW_CHAR)
        {
            return new string(character, numberOfCharacters);
        }
    }
}

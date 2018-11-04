using System;

namespace ExtractMetadataAndParse
{
    public static class ProgressBar
    {
        public static void DrawTextProgressBar(int progress, int total)
        {
            Console.CursorLeft = 0;
            Console.Write("[");
            Console.CursorLeft = 32;
            Console.Write("]");
            Console.CursorLeft = 1;
            var onechunk = 30.0f / total;

            var position = 1;
            for (var i = 0; i < onechunk * progress; i++)
            {
                Console.CursorLeft = position++;
                Console.Write("#");
            }

            for (var i = position; i <= 31; i++)
            {
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            Console.CursorLeft = 35;
            Console.Write(progress + " of " + total);
        }
    }
}
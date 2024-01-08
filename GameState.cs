using System;

namespace PhatTan_Nguyen_Wordle
{
    public static class GameState
    {
        public static string CurrentWord { get; set; }
        public static int MaxGuesses { get; set; }
        public static int GuessesMade { get; set; }
        public static TimeSpan ElapsedTime { get; set; }
        public static bool IsTimerRunning { get; set; }
    }
}

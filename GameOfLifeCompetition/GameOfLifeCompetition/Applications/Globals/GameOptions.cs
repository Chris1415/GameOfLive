namespace GameOfLiveCompetition.Applications.Globals
{
    /// <summary>
    /// Global Game Options Class
    /// </summary>
    /// <author>Christian Hahn</author>
    public static class GameOptions
    {
        /// <summary>
        /// The Board Size
        /// </summary>
        public static int BoardSize { get; set; }

        /// <summary>
        /// The Maximum Number of generations
        /// </summary>
        public static int MaxNumberOfGenerations { get; set; }

        /// <summary>
        /// Determines if the Board is circular, so that at the end of the Board n the next neighbor is 0 
        /// </summary>
        public static bool CircularBoard { get; set; }

        /// <summary>
        /// The Degree of Living Cells decimal from 0.0 to 1.0
        /// </summary>
        public static float DegreeOfLivingCells { get; set; }

        /// <summary>
        /// Determines if the Input should be random
        /// </summary>
        public static bool RandomInput { get; set; }

        /// <summary>
        /// Sets the Game Speed in mSecs
        /// </summary>
        public static int GameSpeed { get; set; }
    }
}


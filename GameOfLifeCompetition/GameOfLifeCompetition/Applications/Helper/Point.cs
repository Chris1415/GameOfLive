namespace GameOfLiveCompetition.Applications.Helper
{
    /// <summary>
    /// Point class for basic operations on geometrical points
    /// </summary>
    /// <author>Christian Hahn</author>
    public class Point
    {
        /// <summary>
        /// Basic Constructor
        /// </summary>
        public Point()
        {
            X = 0;
            Y = 0;
        }

        /// <summary>
        /// Parametrized Constructor with coordinates
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// X Coordinate
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y Coordinate
        /// </summary>
        public int Y { get; set; }
    }
}


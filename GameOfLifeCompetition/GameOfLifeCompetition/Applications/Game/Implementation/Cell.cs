using GameOfLiveCompetition.Applications.Helper;

namespace GameOfLiveCompetition.Applications.Game.Implementation
{
    /// <summary>
    /// Basic Cell Class, which represents a single Field on the Board
    /// </summary>
    /// <author>Christian Hahn</author>
    public class Cell
    {
        /// <summary>
        /// Basic Contrcutor
        /// </summary>
        public Cell()
        {
            Point = new Point();
            LivingStatus = false;
        }

        /// <summary>
        /// Parametrized Constructor with geometric x and y coordinates
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public Cell(int x, int y)
        {
            Point = new Point(x, y);
        }

        /// <summary>
        /// Parametrized contrcutor
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="isAlive">IsAlive Flag</param>
        public Cell(int x, int y, bool isAlive)
        {
            Point = new Point(x, y);
            LivingStatus = isAlive;
        }

        /// <summary>
        /// Gets the living status of the cell
        /// </summary>
        /// <returns>true if the cell is alive, otherwise false</returns>
        public bool LivingStatus { get; set; }

        /// <summary>
        /// Point reference to access the geometrical Position
        /// </summary>
        public Point Point { get; set; }
    }
}


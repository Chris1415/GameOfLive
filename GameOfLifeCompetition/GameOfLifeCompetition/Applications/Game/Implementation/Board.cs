using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GameOfLiveCompetition.Applications.Globals;
using GameOfLiveCompetition.Applications.Helper;
using GameOfLiveCompetition.Applications.UI;

namespace GameOfLiveCompetition.Applications.Game.Implementation
{
    /// <summary>
    /// The Board Class
    /// Holds all information about the current game
    /// </summary>
    /// <author>Christian Hahn</author>
    public class Board
    {
        /// <summary>
        /// Basic Board constructor
        /// </summary>
        public Board(MainWindow view)
        {
            InitializeBoard(GameOptions.BoardSize);
            CurrentNumberOfGeneration = 0;
            View = view;
            Suspenevent = new ManualResetEvent(true);
        }

        /// <summary>
        /// Event fired, when a round has been Complet
        /// </summary>
        public event Action<Board> RoundCompleted;

        /// <summary>
        /// Event to determine that the game has finished
        /// </summary>
        public event Action GameFinished;

        /// <summary>
        /// Reference to the current View
        /// </summary>
        public MainWindow View { get; set; }

        /// <summary>
        /// Determines the current number of generations
        /// </summary>
        public int CurrentNumberOfGeneration { get; set; }

        /// <summary>
        /// The Cells of the Board
        /// </summary>
        public IList<Cell> Cells { get; set; }

        /// <summary>
        /// Give the size of one dimension
        /// </summary>
        public int CurrentBoardSize { get; set; }

        /// <summary>
        /// Flag for Thread Suspension
        /// </summary>
        public static ManualResetEvent Suspenevent { get; set; }

        /// <summary>
        /// Initializes the board with spcific cells
        /// </summary>
        /// <param name="boardSize">The board size</param>
        private void InitializeBoard(int boardSize)
        {
            CurrentBoardSize = boardSize;
            Cells = new List<Cell>(CurrentBoardSize * CurrentBoardSize);

            for (int columnCounter = 0; columnCounter < CurrentBoardSize; columnCounter++)
            {
                for (int rowCounter = 0; rowCounter < CurrentBoardSize; rowCounter++)
                {
                    Cell cell = new Cell(columnCounter, rowCounter, SetInitialLivingStatus());
                    Cells.Add(cell);
                }
            }
        }

        /// <summary>
        /// Gets the input from an extenr Source
        /// </summary>
        /// <param name="externSource">the extenr source</param>
        public void GetInputFromExtern(List<bool> externSource)
        {
            for (int columnCounter = 0; columnCounter < CurrentBoardSize; columnCounter++)
            {
                for (int rowCounter = 0; rowCounter < CurrentBoardSize; rowCounter++)
                {
                    Cells[columnCounter * CurrentBoardSize + rowCounter].LivingStatus =
                        externSource.ElementAt(columnCounter * CurrentBoardSize + rowCounter);
                }
            }
        }

        /// <summary>
        /// Initial Determination of Living Cells in the Board
        /// Uses a random distribution
        /// </summary>
        /// <returns>true if the cell is living, otherwise false</returns>
        private static bool SetInitialLivingStatus()
        {
            if (!GameOptions.RandomInput)
            {
                return false;
            }

            // Use a random number generator for determination of the initial LivingStatus
            Random randomGenerator = new Random(Guid.NewGuid().GetHashCode());
            // Use the degreeOfLiving cells to determine if a cell is living or not
            int r = randomGenerator.Next(0, Int32.MaxValue);
            return (r < (Int32.MaxValue * GameOptions.DegreeOfLivingCells));
        }

        /// <summary>
        /// Entrypoint of the Board Class to start the Game
        /// </summary>
        public void StartGame()
        {
            // Fire Event for Printing the Board
            RoundCompleted(this);
            // Main Game Loop until the maximun number of Generations are reached or the populattion is extinguished
            while (CurrentNumberOfGeneration < GameOptions.MaxNumberOfGenerations && !PopulationExtinguished())
            {
                NextGeneration();
                CurrentNumberOfGeneration++;
                RoundCompleted(this);
                Thread.Sleep(GameOptions.GameSpeed);
                // Event for waiting (pausing), if the event is set by the UI
                Suspenevent.WaitOne();
            }

            if (CurrentNumberOfGeneration < GameOptions.MaxNumberOfGenerations)
            {
                // The population is extinguished
            }

            GameFinished();
        }

        /// <summary>
        /// Make a Deep Copy of the current Board to apply changes individually
        /// </summary>
        /// <param name="src">source Board</param>
        /// <returns>Deep Copy of the Board</returns>
        private IList<Cell> DeepCopyOfBoard(IList<Cell> src)
        {
            IList<Cell> temporaryBoard = new List<Cell>();

            for (int columnCounter = 0; columnCounter < CurrentBoardSize; columnCounter++)
            {
                for (int rowCounter = 0; rowCounter < CurrentBoardSize; rowCounter++)
                {
                    Cell cell = src[columnCounter * CurrentBoardSize + rowCounter];
                    temporaryBoard.Add(new Cell(cell.Point.X, cell.Point.Y, cell.LivingStatus));
                }
            }

            return temporaryBoard;
        }

        /// <summary>
        /// Computes the next generation of cells in the game
        /// </summary>
        public void NextGeneration()
        {
            // Make a Deep Copy of the Board to work with in the Round
            IList<Cell> temporaryBoard = DeepCopyOfBoard(Cells);

            // Run through the whole board in parallel
            Parallel.For(0, CurrentBoardSize,
                col => Parallel.For(0, CurrentBoardSize,
                    row =>
                    {
                        Cell cell = Cells[col * CurrentBoardSize + row];
                        // Use the current status of the cell and its neighbors to determine the new living status 
                        bool newLivingStatus = DetermineNewCellLivingStatus(cell);
                        temporaryBoard[col * CurrentBoardSize + row].LivingStatus = newLivingStatus;
                    }));

            // Copy Back the Results to the origin Board
            Cells = DeepCopyOfBoard(temporaryBoard);
        }

        /// <summary>
        /// Get the Number of living neighbors
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="circularBoard">Determines if neighbors around the Borad from n to 0 are taken</param>
        /// <returns>The number of Living neighbors</returns>
        private int GetNumberOfLivingNeighbors(Cell cell, bool circularBoard)
        {
            Point currentPosition = cell.Point;
            int numberOfLivingCells = 0;

            // Run through all neighbors (8er)
            // Here Usage of Parallel.For could produce an overhead, so no paralelism is used
            for (int col = currentPosition.X - 1; col <= currentPosition.X + 1; col++)
            {
                for (int row = currentPosition.Y - 1; row <= currentPosition.Y + 1; row++)
                {
                    int realCol = col;
                    int realRow = row;
                    bool isAroundTheBoard = false;

                    // If the neighor-cell is the origin -> skip
                    if (col == currentPosition.X && row == currentPosition.Y)
                    {
                        continue;
                    }

                    // If the Edge is reached at the top, go around the boarder
                    if (realRow < 0)
                    {
                        realRow = CurrentBoardSize - 1;
                        isAroundTheBoard = true;
                    }

                    // If the Edge is reached at the left, go around the boarder
                    if (realCol < 0)
                    {
                        realCol = CurrentBoardSize - 1;
                        isAroundTheBoard = true;
                    }

                    // If the Edge is reached at the bottom, go around the boarder
                    if (realRow >= CurrentBoardSize)
                    {
                        realRow = 0;
                        isAroundTheBoard = true;
                    }

                    // If the Edge is reached at the right, go around the Boarder
                    if (realCol >= CurrentBoardSize)
                    {
                        realCol = 0;
                        isAroundTheBoard = true;
                    }

                    // Check if a circular behaviour of the board is wished
                    if (!circularBoard && isAroundTheBoard)
                    {
                        // If no circular behaviour is wished and the current neighbor would be outside -> skip
                        continue;
                    }

                    // Determine the Living Status of the neighbor-cell and increment the neighbor counter
                    if (Cells[realCol * CurrentBoardSize + realRow].LivingStatus)
                    {
                        numberOfLivingCells++;
                    }
                }
            }

            return numberOfLivingCells;
        }

        /// <summary>
        /// Gets the new Living Status in the next generation 
        /// </summary>
        /// <param name="cell"></param>
        /// <returns>true if the Cell is alive in the next genreation, otherwise false</returns>
        private bool DetermineNewCellLivingStatus(Cell cell)
        {
            bool oldLivingStatus = cell.LivingStatus;
            bool newLivingStatus;
            int numberOfLivingNeighbors = GetNumberOfLivingNeighbors(cell, GameOptions.CircularBoard);

            // Apply the Rules of the Game
            if (oldLivingStatus)
            {
                // If a Cell is living
                if (numberOfLivingNeighbors < 2 || numberOfLivingNeighbors > 3)
                {
                    // The Cell dies if less than 2 or more than 3 Cells are living in the neighborhood
                    newLivingStatus = false;
                }
                else
                {
                    // The Cell survives, if 2 or 3 living cells are in neighborhood
                    newLivingStatus = true;
                }
            }
            else
            {
                // If the Cell is dead, it will get alive if exactly 3 cells are living in the neighborhood
                newLivingStatus = numberOfLivingNeighbors == 3;
            }

            return newLivingStatus;
        }

        /// <summary>
        /// Determine if the Population is extinguished on the board
        /// </summary>
        /// <returns>true if the population is extinguished, otherwise false</returns>
        public bool PopulationExtinguished()
        {
            return !Cells.Any(cell => cell.LivingStatus);
        }
    }
}

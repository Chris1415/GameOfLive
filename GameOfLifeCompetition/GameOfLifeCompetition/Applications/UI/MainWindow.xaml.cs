using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using GameOfLiveCompetition.Applications.Game.Implementation;
using GameOfLiveCompetition.Applications.Globals;
using GameOfLiveCompetition.Applications.Helper;

namespace GameOfLiveCompetition.Applications.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <author>Christian Hahn</author>
    public partial class MainWindow
    {
        /// <summary>
        /// Reference the the GameHandler
        /// </summary>
        public GameHandler GameHandler { get; set; }

        /// <summary>
        /// Constructor for the Main Window
        /// </summary>
        public MainWindow()
        {
            GameHandler = new GameHandler(this);
            GameHandler.RoundCompletedEventHandler += PrintBoardAfterRoundCompleted;
            InitializeComponent();
        }

        /// <summary>
        /// Inserts the initial Cells
        /// White Cells with the right callbacks
        /// </summary>
        private void InitializeGrid()
        {
            LifeBoard.ColumnDefinitions.Clear();
            LifeBoard.RowDefinitions.Clear();
            LifeBoard.Children.Clear();
            for (int i = 0; i < GameOptions.BoardSize; i++)
            {
                for (int j = 0; j < GameOptions.BoardSize; j++)
                {
                    DataGridCell cell = new DataGridCell();
                    if (!GameOptions.RandomInput)
                    {
                        cell.MouseDoubleClick += CellClicked;     
                    }
                    cell.SetColorOfCell(Strings.Colors.Black);
                    cell.BorderBrush = Brushes.Black;
                    Grid.SetColumn(cell, j);
                    Grid.SetRow(cell, i);
                    LifeBoard.Children.Add(cell);
                }

                LifeBoard.ColumnDefinitions.Add(new ColumnDefinition());
                LifeBoard.RowDefinitions.Add(new RowDefinition());
            }
        }

        /// <summary>
        /// Callback if a cell is clicked
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="args">args</param>
        public void CellClicked(object sender, MouseEventArgs args)
        {
            UIElement senderCell = sender as UIElement;
            string currentColor = senderCell.GetColorOfCell();
            if (currentColor.Equals(Strings.Colors.White))
            {
                senderCell.SetColorOfCell(Strings.Colors.Black);
                return;
            }

            senderCell.SetColorOfCell(Strings.Colors.White);
        }

        /// <summary>
        /// Simple Validator to remove non number characters
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args</param>
        private void TextBoxValidation(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            int textboxValue;

            if (textBox == null)
            {
                return;
            }

            // Check if the inserted Text is valid (Int32)
            if (Int32.TryParse(textBox.Text, out textboxValue))
            {
                return;
            }

            // Get information about changed Text and revert the changes
            TextChange textChange = e.Changes.ElementAt(0);
            int iAddedLength = textChange.AddedLength;
            int iOffset = textChange.Offset;
            textBox.Text = textBox.Text.Remove(iOffset, iAddedLength);
        }

        /// <summary>
        /// Event for the Continue/Pause Button
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">Click Args</param>
        private void ToggleContinuePause(object sender, RoutedEventArgs e)
        {
            GameHandler.ToggleContinuePause();
        }

        /// <summary>
        /// Event for the Start/Stop Button Click
        /// </summary>
        /// <param name="sender">The Button</param>
        /// <param name="e">Click Args</param>
        private void ToggleStartStop(object sender, RoutedEventArgs e)
        {
            GameHandler.ToggleStartStop();
        }

        /// <summary>
        /// The click event for the Apply Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyClicked(object sender, RoutedEventArgs e)
        {
            string erroLog = string.Empty;
            // Validate the input in the options sidebar
            if (!InputIsValid(ref erroLog))
            {
                // Give Feedback of wrong input
                ErrorLog.Foreground = Brushes.Red;
                ErrorLog.Text = erroLog;
                return;
            }

            // Apply the Options from the sidebar
            GameOptions.BoardSize = Int32.Parse(BoardSizeOption.Text);
            GameOptions.DegreeOfLivingCells = (Int32.Parse(DegreeOfLivingCellsOptions.Text) / (float)100);
            GameOptions.MaxNumberOfGenerations = Int32.Parse(NumberOfGenerationsOption.Text);
            GameOptions.RandomInput = RandomInputCheckboxOption.IsChecked != null && RandomInputCheckboxOption.IsChecked.Value;
            GameOptions.CircularBoard = CircularBoardCheckboxOption.IsChecked != null && CircularBoardCheckboxOption.IsChecked.Value;
            GameOptions.GameSpeed = Int32.Parse(GamespeedOpton.Text);
            
            StartStopButton.IsEnabled = true;
            ErrorLog.Text = string.Empty;
            InitializeGrid();
        }

        /// <summary>
        /// Validates the Input
        /// </summary>
        /// <returns>true if the input from the sidebar is valid</returns>
        private bool InputIsValid(ref string errorLog)
        {
            if (errorLog == null)
            {
                throw new ArgumentNullException("errorLog");
            }

            StringBuilder errorBuilder = new StringBuilder();
            bool isValid = true;

            if(BoardSizeOption.Text.Equals(string.Empty) || Int32.Parse(BoardSizeOption.Text) > 1000)
            {
                errorBuilder.AppendLine("Enter a board size between 0 and 1000");
                isValid = false;
            }

            if (DegreeOfLivingCellsOptions.Text.Equals(string.Empty) ||
                Int32.Parse(DegreeOfLivingCellsOptions.Text) < 0 || 
                Int32.Parse(DegreeOfLivingCellsOptions.Text) > 100)
            {
                errorBuilder.AppendLine("Enter a degree between 0 and 100");
                isValid = false;
            }

            if (NumberOfGenerationsOption.Text.Equals(string.Empty))
            {
                errorBuilder.AppendLine("Enter a value for the number of generations");
                isValid = false;
            }

            if (GamespeedOpton.Text.Equals(string.Empty))
            {
                errorBuilder.AppendLine("Enter a value for the pause between Rounds");
                isValid = false;
            }
            
            errorLog = errorBuilder.ToString();
            return isValid;

        }

        /// <summary>
        /// Event for Application Close to savely shut down the environment
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">args</param>
        private void ApplicationCloseEvent(object sender, CancelEventArgs e)
        {
            GameHandler.ExitGame();
        }

        /// <summary>
        /// Event to Call after a Round has been completed
        /// </summary>
        /// <param name="board">reference to the board</param>
        private void PrintBoardAfterRoundCompleted(Board board)
        {
            PrintBoardInGrid(board);
            PrintNumberOfGeneration(board);
        }

        /// <summary>
        /// Print the number of generation
        /// </summary>
        /// <param name="board">reference to the board</param>
        private void PrintNumberOfGeneration(Board board)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                GenerationLabel.Content = string.Format("Generation {0}/{1}", board.CurrentNumberOfGeneration,
                    GameOptions.MaxNumberOfGenerations);
            });
        }
        
        /// <summary>
        /// Get the input of the Board
        /// </summary>
        /// <returns>the input of the Board </returns>
        public List<bool> GetCurrentInputFromUi()
        {
            List<bool> uiInput = new List<bool>();
            for (int col = 0; col < GameHandler.Board.CurrentBoardSize; col++)
            {
                for (int row = 0; row < GameHandler.Board.CurrentBoardSize; row++)
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart) delegate
                    {

                        UIElement element = LifeBoard.Children
                            .Cast<UIElement>()
                            .First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);

                        string color = element.GetColorOfCell();
                        uiInput.Add(color.Equals(Strings.Colors.White));
                    });
                }
            }
               
            return uiInput;
        }


        /// <summary>
        /// Prints the current Board in a Grid
        /// </summary>
        private void PrintBoardInGrid(Board board)
        {
            Parallel.For(0, GameHandler.Board.CurrentBoardSize,
                col => Parallel.For(0, GameHandler.Board.CurrentBoardSize,
                    row => Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        UIElement element = LifeBoard.Children
                            .Cast<UIElement>()
                            .First(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col);

                        element.SetColorOfCell(board.Cells[col * board.CurrentBoardSize + row].LivingStatus
                                    ? Strings.Colors.White
                                    : Strings.Colors.Black);
                    })));
        }
    }
}

using System;using System.Threading;
using System.Windows.Threading;
using GameOfLiveCompetition.Applications.Globals;
using GameOfLiveCompetition.Applications.UI;

namespace GameOfLiveCompetition.Applications.Game.Implementation
{
    /// <summary>
    /// Main Class for handling the current game
    /// </summary>
    /// <author>Christian Hahn</author>
    public class GameHandler
    {
        /// <summary>
        /// Reference to the current Model
        /// </summary>
        public Board Board { get; set; }

        /// <summary>
        /// Reference to the current Game Thread
        /// </summary>
        Thread CurrentGameThread { get; set; }

        /// <summary>
        /// Reference to the User Inteface
        /// </summary>
        private MainWindow CurrentWindow { get; set; }

        /// <summary>
        /// The ROund Completed Event Handler
        /// </summary>
        public event Action<Board> RoundCompletedEventHandler;

        /// <summary>
        /// Constructor with the Main Window as Reference
        /// </summary>
        /// <param name="mainWindow">Reference to the UI</param>
        public GameHandler(MainWindow mainWindow)
        {
            CurrentWindow = mainWindow;
        }

        /// <summary>
        /// Exits the Application safely
        /// </summary>
        public void ExitGame()
        {
            //// Mechanism to savely shut down the Application
            if (CurrentGameThread != null)
            {
                CurrentGameThread.Abort();
            }
        }

        /// <summary>
        /// Entry Point for the GUI Interaction
        /// </summary>
        public void ToggleStartStop()
        {
            if (CurrentWindow.StartStopButton.Content.Equals(Strings.ButtonTexts.StopButtonText))
            {
                // If The Button has the Text Stop -> Stop the Game, disable the running Thread and change the Text to Start
                CurrentWindow.StartStopButton.Content = Strings.ButtonTexts.StartButtonText;
                CurrentWindow.ContinuePauseButton.IsEnabled = false;
                CurrentWindow.ApplyButton.IsEnabled = true;
                if (CurrentGameThread == null)
                {
                    return;
                }

                CurrentGameThread.Abort();
                CurrentGameThread = null;
            }
            else if (CurrentWindow.StartStopButton.Content.Equals(Strings.ButtonTexts.StartButtonText))
            {
                // If The Button has the Text Start -> Start the Game by starting the Thread and change the Text to Stop
                if (CurrentGameThread == null)
                {
                    CurrentWindow.StartStopButton.Content = Strings.ButtonTexts.StopButtonText;
                    CurrentWindow.ContinuePauseButton.IsEnabled = true;
                    CurrentWindow.ApplyButton.IsEnabled = false;
                    CurrentGameThread = new Thread(StartThread);
                    CurrentGameThread.Start();
                }

                CurrentWindow.StartStopButton.Content = Strings.ButtonTexts.StopButtonText;
            }

        }

        /// <summary>
        /// Entry Point for the GUI Interaction
        /// </summary>
        public void ToggleContinuePause()
        {
           if (CurrentWindow.ContinuePauseButton.Content.Equals(Strings.ButtonTexts.PauseButtonText))
            {
                // If The Button has the Text Pause -> Pause the Game and change the Text to Continue
                CurrentWindow.ContinuePauseButton.Content = Strings.ButtonTexts.ContinueButtonText;
                Board.Suspenevent.Reset();
            }
            else if (CurrentWindow.ContinuePauseButton.Content.Equals(Strings.ButtonTexts.ContinueButtonText))
            {
                // If The Button has the Text Continue -> Contine the Game and change the Text to Pause
                CurrentWindow.ContinuePauseButton.Content = Strings.ButtonTexts.PauseButtonText;
                Board.Suspenevent.Set();
            }
        }

        /// <summary>
        /// Thread Start Function
        /// </summary>
        public void StartThread()
        {
            Board = new Board(CurrentWindow);
            if (!GameOptions.RandomInput)
            {
               Board.GetInputFromExtern(CurrentWindow.GetCurrentInputFromUi());
            }

            Board.RoundCompleted += CallbackForRoundCompleted;
            Board.GameFinished += GameFinishedCallback;
            Board.StartGame();
        }

       
        /// <summary>
        /// Callback to handle the Game Finished Event
        /// </summary>
        private void GameFinishedCallback()
        {
            CurrentWindow.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart) ToggleStartStop);
        }

        /// <summary>
        /// Event for Printing the Board
        /// </summary>
        /// <param name="board">Reference to the current Board</param>
        private void CallbackForRoundCompleted(Board board)
        {
            RoundCompletedEventHandler(board);
        }
    }
}

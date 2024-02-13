using Reversi.Persistance;
using ReversiWPF.ViewModel;
using System;
using System.Windows;
using System.Windows.Threading;
using Reversi;
using Reversi.Model;
using System.ComponentModel;
using Microsoft.Win32;
using System.Windows.Media;

namespace ReversiWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Reversi.Model.ReversiGameModel model = null!;
        private ReversiViewModel reversiViewModel = null!;
        private MainWindow view = null!;
        private DispatcherTimer timer = null!;
        
        public App()
        {
            Startup += App_Startup;
        }
        private void App_Startup(object? sender, StartupEventArgs e)
        {
            model = new Reversi.Model.ReversiGameModel(new ReversiFileDataAccess());
            model.GameOver += new EventHandler<ReversiEventArgs>(MGameOver);
            model.Advance += new EventHandler<ReversiEventArgs>(MGameAdvance);

            reversiViewModel = new ReversiViewModel(model);
            reversiViewModel.NewGame += new EventHandler(VMNewGame);
            reversiViewModel.PauseGame += new EventHandler(VMPauseGame);
            reversiViewModel.LoadGame += new EventHandler(VMLoadGame);
            reversiViewModel.SaveGame += new EventHandler(VMSaveGame);
            reversiViewModel.QuitGame += new EventHandler(VMQuitGame);
            reversiViewModel.ChangeMapSize += new EventHandler(VMChangeMapSize);
            reversiViewModel.Pass += new EventHandler(VMPass); 

            view = new MainWindow();
            view.DataContext = reversiViewModel;
            view.Closing+= new System.ComponentModel.CancelEventHandler(CloseWindow);
            view.Show();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Tick;

        }
        private void Tick(object? sender, EventArgs e)
        {
            model.Tick();
        }

        private void CloseWindow(object? sender, CancelEventArgs e)
        {
            bool timerOn = timer.IsEnabled;

            timer.Stop();

            if (MessageBox.Show("Are you sure you want to exit the game?", "Reversi Game", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;

                if (timerOn)
                    timer.Start();
            }
        }

        private void VMNewGame(object? sender, EventArgs e)
        {
            reversiViewModel.MenuVisibility = Visibility.Hidden;
            reversiViewModel.SaveEnabled = true;
            if(reversiViewModel.Buttons!=null)
            {
                reversiViewModel.Buttons.Clear();
            }
            model.NewGame();
            model.CheckValidPlaces();
            reversiViewModel.GenerateButtons();
            timer.Start();
        }
        private void VMPauseGame(object? sender, EventArgs e)
        {
            bool timerOn = timer.IsEnabled;
            reversiViewModel.MenuVisibility = Visibility.Hidden;
            if (timerOn)
            {
                timer.Stop();
            }
            else
            {
                timer.Start();
            }
        }
        private async void VMLoadGame(object? sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Reversi save";
                openFileDialog.Filter = "Reversi game|*.json";
                if (openFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        await model.LoadGameAsync(openFileDialog.FileName);
                    }
                    catch (ReversiDataException)
                    {
                        MessageBox.Show("Couldn't load the game!.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Couldn't load the game!", "Reversi game", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            reversiViewModel.MenuVisibility = Visibility.Hidden;
            reversiViewModel.SaveEnabled = true;
            if (reversiViewModel.Buttons != null)
            {
                reversiViewModel.Buttons.Clear();
            }
            model.CheckValidPlaces();
            reversiViewModel.GenerateButtons();
            timer.Start();
        }
        private async void VMSaveGame(object? sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Reversi save";
                saveFileDialog.Filter = "Reversi game|*.json";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        await model.SaveGameAsync(saveFileDialog.FileName);
                    }
                    catch (ReversiDataException)
                    {
                        MessageBox.Show("Couldn't save the game!.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch 
            {
                MessageBox.Show("Couldn't save the game!", "Reversi game", MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        private void VMQuitGame(object? sender, EventArgs e)
        {
            view.Close();
        }
        private void VMChangeMapSize(object? sender, EventArgs e)
        {
            int size;
            if(sender is not null){
                if (int.TryParse(sender!.ToString()!.Split(',')[0], out size))
                reversiViewModel.vmMapSizeChange(size);
            }
        }
        private void VMPass(object? sender, EventArgs e)
        {
            model.ChangePlayer();
        }

        private void MGameOver(object? sender, ReversiEventArgs e)
        {
            timer.Stop();
            if (e.Player1Won)
            {
                MessageBox.Show("Player 1 won the game!",
                                "Reversi",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            }
            else if (e.Player2Won)
            {
                MessageBox.Show("Player 2 won the game!",
                                "Reversi",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            }
            else
            {
                MessageBox.Show("It's a tie!",
                                "Reversi",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            }
        }
        private void MGameAdvance(object? sender, ReversiEventArgs e)
        {
            if (!model.IsGameOver)
            {
                if (e.Player1Pass)
                {
                    reversiViewModel.P1PassEnabled = true;
                }
                else
                {
                    reversiViewModel.P1PassEnabled = false;
                }
                if (e.Player2Pass)
                {
                    reversiViewModel.P2PassEnabled = true;
                }
                else
                {
                    reversiViewModel.P2PassEnabled = false;
                }
                if (model.GetCurrentPlayerStatus)
                {
                    reversiViewModel.P2Background=Brushes.LightGreen;
                    reversiViewModel.P1Background = Brushes.LightGray;
                }
                else
                {
                    reversiViewModel.P1Background = Brushes.LightGreen;
                    reversiViewModel.P2Background = Brushes.LightGray;
                }
            }
        }
    }
}

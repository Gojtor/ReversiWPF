using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Reversi.Model;

namespace ReversiWPF.ViewModel
{
    public class ReversiViewModel : ViewModelBase
    {
        private Reversi.Model.ReversiGameModel _model;
        private bool saveBtnActive=false;
        private Visibility visibility = Visibility.Visible;
        private Brush p1BackGround = Brushes.LightGray;
        private Brush p2BackGround = Brushes.LightGray;
        private bool p1PassEna=false;
        private bool p2PassEna=false;
        
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }
        public DelegateCommand QuitGameCommand { get; private set; }
        public DelegateCommand PauseGameCommand { get; private set; }
        public DelegateCommand ChangeMapSizeRadioCommand { get; private set; }
        public DelegateCommand PassCommand { get; private set; }
        public ObservableCollection<ReversiButtons>? Buttons { get; set; }

        public event EventHandler? NewGame;
        public event EventHandler? LoadGame;
        public event EventHandler? SaveGame;
        public event EventHandler? QuitGame;
        public event EventHandler? PauseGame;
        public event EventHandler? ChangeMapSize;
        public event EventHandler? Pass;
        public int Player1Pieces { get { return _model.Player1PieceCount; } }
        public int Player2Pieces { get { return _model.Player2PieceCount; } }
        public int Player1Time { get { return _model.Player1Time; } }
        public int Player2Time { get { return _model.Player2Time; } }

        public Brush P1Background
        {
            get{ return p1BackGround; }
            set
            {
                p1BackGround = value;
                OnPropertyChanged(nameof(P1Background));
            }
        }
        public Brush P2Background
        {
            get { return p2BackGround; }
            set
            {
                p2BackGround = value;
                OnPropertyChanged(nameof(P2Background));
            }
        }
        public bool P1PassEnabled { 
            get { return p1PassEna; } 
            set
            {
                p1PassEna = value;
                OnPropertyChanged(nameof(P1PassEnabled));
                _model.CheckValidPlaces();
                SetTable();
                OnPropertyChanged(nameof(Buttons));
            }
        }
        public bool P2PassEnabled { 
            get { return p2PassEna; }
            set
            {
                p2PassEna = value;
                OnPropertyChanged(nameof(P2PassEnabled));
                _model.CheckValidPlaces();
                SetTable();
                OnPropertyChanged(nameof(Buttons));
            }
        }

        public bool SaveEnabled {
            get { return saveBtnActive; }
            set 
            {
               saveBtnActive = value;
               OnPropertyChanged(nameof(SaveEnabled));
            }
        }

        public Visibility MenuVisibility
        {
            get { return visibility; }
            set {
                if (visibility == Visibility.Hidden)
                {
                    visibility = Visibility.Visible;
                }
                else
                {
                    visibility = value;
                }
                OnPropertyChanged(nameof(MenuVisibility)); 
            }
        }
        public int RowCount { 
            get { return _model.MapSize; }
            set
            {
                value=_model.MapSize;
                OnPropertyChanged(nameof(RowCount));
            }
        }
        public int ColumnCount { 
            get { return _model.MapSize; } 
            set { 
                value= _model.MapSize;
                OnPropertyChanged(nameof(ColumnCount));
            }
        }

       
        public ReversiViewModel(Reversi.Model.ReversiGameModel model) 
        {
            _model = model;
            NewGameCommand = new DelegateCommand(param=> WhenNewGame());
            LoadGameCommand = new DelegateCommand(param=> WhenLoadGame());
            SaveGameCommand = new DelegateCommand(param=> WhenSaveGame());
            QuitGameCommand = new DelegateCommand(param => WhenQuitGame());
            PauseGameCommand = new DelegateCommand(param=> WhenPauseGame());
            ChangeMapSizeRadioCommand = new DelegateCommand(param => WhenChangeMapSize(param!));
            PassCommand = new DelegateCommand(param => WhenPass());

            _model.ButtonChange += new EventHandler<ReversiButtonChangeEventArgs>(Model_ButtonChange);
            _model.Advance += new EventHandler<ReversiEventArgs>(Model_GameAdvance);
            _model.GameOver += new EventHandler<ReversiEventArgs>(Model_GameOver);

        }

        

        private void PutPiece(int x, int y)
        {
            _model.Update(x, y);
        }

        private void SetTable()
        {
            foreach (ReversiButtons button in Buttons!)
            {
                int modelValue = _model.Table.GetValue(button.X, button.Y);
                button.Text = !(modelValue == 0) ? modelValue.ToString() : String.Empty;
                if (modelValue == 2)
                {
                    button.IsValid= true;
                }
            }
            OnPropertyChanged(nameof(Player1Time));
            OnPropertyChanged(nameof(Player2Time));
        }

        private void Model_ButtonChange(object? sender, ReversiButtonChangeEventArgs changed)
        {
            ReversiButtons _button = Buttons!.Single(button => button.X == changed.X && button.Y == changed.Y);
            int modelValue = _model.Table.GetValue(_button.X, _button.Y);
            if (modelValue == 0)
            {
                _button.Text= String.Empty;
            }
            else
            {
                _button.Text=modelValue.ToString();
            }
            _model.CheckValidPlaces();
            SetTable();

        }
        private void Model_GameAdvance(object? sender, ReversiEventArgs e) 
        {
            OnPropertyChanged(nameof(Player1Time));
            OnPropertyChanged(nameof(Player2Time));
            OnPropertyChanged(nameof(Player1Pieces));
            OnPropertyChanged(nameof(Player2Pieces));
        }
        private void Model_GameOver(object? sender, ReversiEventArgs e) 
        {
            foreach (ReversiButtons field in Buttons!)
            {
                field.IsValid = false;
            }
        }
        private void Model_GameStart(object? sender, ReversiEventArgs e) { SetTable(); }

        private void WhenNewGame() { NewGame?.Invoke(this, EventArgs.Empty); }
        private void WhenLoadGame() { LoadGame?.Invoke(this, EventArgs.Empty); }
        private void WhenSaveGame() { SaveGame?.Invoke(this, EventArgs.Empty); }
        private void WhenQuitGame() { QuitGame?.Invoke(this, EventArgs.Empty); }
        private void WhenPauseGame() { PauseGame?.Invoke(this, EventArgs.Empty); }
        private void WhenChangeMapSize(object param) { ChangeMapSize?.Invoke(param, EventArgs.Empty); }
        private void WhenPass() { Pass?.Invoke(this, EventArgs.Empty); }


        public void GenerateButtons()
        {
            RowCount = _model.MapSize;
            ColumnCount = _model.MapSize;
            OnPropertyChanged(nameof(Buttons));
            Buttons = new ObservableCollection<ReversiButtons>();
            for (int i = 0; i < _model.MapSize; i++)
            {
                for (int j = 0; j < _model.MapSize; j++)
                {
                    Buttons.Add(new ReversiButtons
                    {
                        IsValid = false,
                        Text = "0",
                        X = i,
                        Y = j,
                        PutPieceCommand = new DelegateCommand(param =>
                        {
                            if (param is Tuple<int, int> position)
                                PutPiece(position.Item1, position.Item2);
                        })
                    });
                }
            }
            SetTable();
            OnPropertyChanged(nameof(Buttons));
        }
        public void vmMapSizeChange(int size)
        {
            _model.MapSize = size;     
        }
    }
}

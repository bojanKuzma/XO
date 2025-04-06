using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using XO.Commands;
using XO.Stores;

namespace XO.ViewModels;

public class GameViewModel : BaseViewModel
{
    private readonly NavigationStore _navigationStore;
    private readonly Random _random = new();
    private string[] _board = new string[9];
    private bool _isNameInputVisible;
    private bool _isPlayer1Turn;
    private bool _isProcessingMove;
    private string _player1Name;
    private string _player2Name;
    private bool _isSinglePlayer;

    public GameViewModel(bool isSinglePlayer, NavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
        _isSinglePlayer = isSinglePlayer;
        Player1Name = string.Empty;
        Player2Name = isSinglePlayer ? "AI" : string.Empty;
        IsPlayer1Turn = true;
        IsNameInputVisible = true;
        StartGameCommand = new ActionCommand<object>(_ => StartGame(), _ => CanStartGame());
        CellClickCommand = new ActionCommand<string>(OnCellClick, _ => !IsProcessingMove);
        GiveUpCommand = new ActionCommand<object>(_ => GiveUp());
        ResetCommand = new ActionCommand<object>(_ => Reset());
    }
    
    private bool _isCelebrating;
    private string _winnerMessage;

    public bool IsCelebrating
    {
        get => _isCelebrating;
        set
        {
            _isCelebrating = value;
            OnPropertyChanged(nameof(IsCelebrating));
        }
    }

    public string WinnerMessage
    {
        get => _winnerMessage;
        set
        {
            _winnerMessage = value;
            OnPropertyChanged(nameof(WinnerMessage));
        }
    }

    public bool IsProcessingMove
    {
        get => _isProcessingMove;
        set
        {
            _isProcessingMove = value;
            OnPropertyChanged(nameof(IsProcessingMove));
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public string[] Board
    {
        get => _board;
        set
        {
            _board = value;
            OnPropertyChanged(nameof(Board));
        }
    }

    public bool IsPlayer1Turn
    {
        get => _isPlayer1Turn;
        set
        {
            _isPlayer1Turn = value;
            OnPropertyChanged(nameof(IsPlayer1Turn));
        }
    }

    public bool IsSinglePlayer
    {
        get => _isSinglePlayer;
        private set
        {
            _isSinglePlayer = value;
            OnPropertyChanged(nameof(IsSinglePlayer));
        }
    }

    public string Player1Name
    {
        get => _player1Name;
        set
        {
            if (value != null && value.Length > 15)
                value = value.Substring(0, 15);
            _player1Name = value;
            OnPropertyChanged(nameof(Player1Name));
        }
    }

    public string Player2Name
    {
        get => _player2Name;
        set
        {
            if (value != null && value.Length > 15)
                value = value.Substring(0, 15);
            
            _player2Name = value;
            OnPropertyChanged(nameof(Player2Name));
        }
    }

    public bool IsNameInputVisible
    {
        get => _isNameInputVisible;
        set
        {
            _isNameInputVisible = value;
            OnPropertyChanged(nameof(IsNameInputVisible));
        }
    }

    public ICommand StartGameCommand { get; }
    public ICommand CellClickCommand { get; }
    public ICommand GiveUpCommand { get; }
    public ICommand ResetCommand { get; }


    private void StartGame()
    {
        IsNameInputVisible = false;
        InitializeGame();
    }

    private bool CanStartGame()
    {
        // Only allow starting the game if Player1Name is provided
        // For single-player mode, Player2Name is "AI" by default
        return !string.IsNullOrWhiteSpace(Player1Name) &&
               (IsSinglePlayer || !string.IsNullOrWhiteSpace(Player2Name));
    }

    private void InitializeGame()
    {
        Console.WriteLine($"Starting game: {Player1Name} vs {Player2Name}");
    }

    private async void OnCellClick(string cellIndex)
    {
        if (IsProcessingMove) return;
        IsProcessingMove = true;

        try
        {
            if (!int.TryParse(cellIndex, out var index) || index < 0 || index >= 9 ||
                !string.IsNullOrEmpty(Board[index])) return;
            UpdateBoardCell(index, IsPlayer1Turn ? "X" : "O");
            IsPlayer1Turn = !IsPlayer1Turn;

            // Check for a winner or a draw
            CheckForWinner();

            // If single player mode and it's AI's turn, make AI move
            if (IsSinglePlayer && !IsPlayer1Turn) 
                await MakeAIMove();
        }
        finally
        {
            IsProcessingMove = false;
        }
    }

    private async Task MakeAIMove()
    {
        var availableMoves = new List<int>();
        for (var i = 0; i < 9; i++)
            if (string.IsNullOrEmpty(Board[i]))
                availableMoves.Add(i);

        if (availableMoves.Count > 0)
        {
            // Small delay for better UX (optional)
            await Task.Delay(500);

            var randomIndex = _random.Next(availableMoves.Count);
            var aiMove = availableMoves[randomIndex];

            UpdateBoardCell(aiMove, "O");
            IsPlayer1Turn = !IsPlayer1Turn;

            CheckForWinner();
        }
    }

    private void CheckForWinner()
    {
        // Check rows
        for (var i = 0; i < 3; i++)
            if (!string.IsNullOrEmpty(Board[i * 3]) && Board[i * 3] == Board[i * 3 + 1] &&
                Board[i * 3 + 1] == Board[i * 3 + 2])
            {
                ShowWinner(Board[i * 3]);
                return;
            }

        // Check columns
        for (var j = 0; j < 3; j++)
            if (!string.IsNullOrEmpty(Board[j]) && Board[j] == Board[j + 3] && Board[j + 3] == Board[j + 6])
            {
                ShowWinner(Board[j]);
                return;
            }

        // Check diagonals
        if (!string.IsNullOrEmpty(Board[0]) && Board[0] == Board[4] && Board[4] == Board[8])
        {
            ShowWinner(Board[0]);
            return;
        }

        if (!string.IsNullOrEmpty(Board[2]) && Board[2] == Board[4] && Board[4] == Board[6])
        {
            ShowWinner(Board[2]);
            return;
        }

        // Check for a draw
        if (IsBoardFull()) ShowWinner("Draw");
    }

    private bool IsBoardFull()
    {
        foreach (var cell in Board)
            if (string.IsNullOrEmpty(cell))
                return false;

        return true;
    }

    private async void ShowWinner(string winner)
    {
        if (DialogHost.IsDialogOpen("RootDialog"))
        {
            return;
        }
        var message = winner == "Draw" ? "It's a draw!" : $"Player {winner} wins!";
        WinnerMessage = message;
            
        // Start celebration
        IsCelebrating = true;
        var dialogContent = new StackPanel
        {
            Margin = new Thickness(20),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        dialogContent.Children.Add(new TextBlock
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        });

        var closeButton = new Button
        {
            Content = "Close",
            Margin = new Thickness(0, 20, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Center
        };
        closeButton.Click += (s, e) => DialogHost.CloseDialogCommand.Execute(null, null);

        dialogContent.Children.Add(closeButton);

        await DialogHost.Show(dialogContent, "RootDialog");
        IsCelebrating = false;

        // Reset the board
        Board = new string[9];
        IsPlayer1Turn = true;
    }

    private void UpdateBoardCell(int index, string value)
    {
        if (index >= 0 && index < 9)
        {
            Board[index] = value;
            OnPropertyChanged(nameof(Board));
        }
    }

    private void GiveUp()
    {
        _navigationStore.CurrentViewModel = new MainMenuViewModel(_navigationStore);
    }

    private void Reset()
    {
        Board = new string[9];
    }
}
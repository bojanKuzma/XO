using System.Windows;
using System.Windows.Input;
using XO.Commands;
using XO.Stores;

namespace XO.ViewModels;

public class GameViewModel : BaseViewModel
{
    private string[] _board = new string[9];
    private bool _isNameInputVisible;
    private bool _isPlayer1Turn;
    private string _player1Name;
    private string _player2Name;
    private readonly NavigationStore _navigationStore;

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

    private bool IsSinglePlayer { get; }

    public string Player1Name
    {
        get => _player1Name;
        set
        {
            _player1Name = value;
            OnPropertyChanged(nameof(Player1Name));
        }
    }

    public string Player2Name
    {
        get => _player2Name;
        set
        {
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
    
    public GameViewModel(bool isSinglePlayer, NavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
        IsSinglePlayer = isSinglePlayer;
        Player1Name = string.Empty;
        Player2Name = isSinglePlayer ? "AI" : string.Empty; 
        IsPlayer1Turn = true;
        IsNameInputVisible = true;
        StartGameCommand = new ActionCommand<object>(_ => StartGame(), _ => CanStartGame());
        CellClickCommand = new ActionCommand<string>(OnCellClick);
        GiveUpCommand = new ActionCommand<object>(_ => GiveUp());
        ResetCommand = new ActionCommand<object>(_ => Reset());
    }

  


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

    private void OnCellClick(string cellIndex)
    {
        if (!int.TryParse(cellIndex, out int index) || index < 0 || index >= 9 ||
            !string.IsNullOrEmpty(Board[index])) return;
        UpdateBoardCell(index, IsPlayer1Turn ? "X" : "O");
        IsPlayer1Turn = !IsPlayer1Turn;

        // Check for a winner or a draw
        CheckForWinner();
    }

    private void CheckForWinner()
    {
        // Check rows
        for (int i = 0; i < 3; i++)
        {
            if (!string.IsNullOrEmpty(Board[i * 3]) && Board[i * 3] == Board[i * 3 + 1] && Board[i * 3 + 1] == Board[i * 3 + 2])
            {
                ShowWinner(Board[i * 3]);
                return;
            }
        }

        // Check columns
        for (int j = 0; j < 3; j++)
        {
            if (!string.IsNullOrEmpty(Board[j]) && Board[j] == Board[j + 3] && Board[j + 3] == Board[j + 6])
            {
                ShowWinner(Board[j]);
                return;
            }
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
        if (IsBoardFull())
        {
            ShowWinner("Draw");
        }
    }

    private bool IsBoardFull()
    {
        foreach (var cell in Board)
        {
            if (string.IsNullOrEmpty(cell))
            {
                return false;
            }
        }
        return true;
    }

    private void ShowWinner(string winner)
    {
        string message = winner == "Draw" ? "It's a draw!" : $"Player {winner} wins!";
        MessageBox.Show(message, "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);

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
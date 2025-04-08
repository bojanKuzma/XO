using System.Windows;
using System.Windows.Input;
using XO.Commands;
using XO.Stores;

namespace XO.ViewModels;

public class MainMenuViewModel : BaseViewModel
{
    private NavigationStore _navigationStore;
    private LanguageStore _languageStore;

    public MainMenuViewModel(NavigationStore navigationStore, LanguageStore languageStore)
    {
        _navigationStore = navigationStore;
        _languageStore = languageStore;
        StartOnePlayerGameCommand = new ActionCommand<object>(_ => StartOnePlayerGame());
        StartTwoPlayerGameCommand = new ActionCommand<object>(_ => StartTwoPlayerGame());
        ShowScoresCommand = new ActionCommand<object>(_ => ShowScores());
    }
    

    public ICommand StartOnePlayerGameCommand { get; }
    public ICommand StartTwoPlayerGameCommand { get; }
    public ICommand ShowScoresCommand { get; }
    
    
    private void StartOnePlayerGame()
    {
        _navigationStore.CurrentViewModel = new GameViewModel(true, _navigationStore, _languageStore);
    }

    private void StartTwoPlayerGame()
    {
        _navigationStore.CurrentViewModel = new GameViewModel(false, _navigationStore, _languageStore);
    }

    private void ShowScores()
    {
        _navigationStore.CurrentViewModel?.Dispose();
        _navigationStore.CurrentViewModel = new ScoresViewModel(_navigationStore, _languageStore);
    }
}
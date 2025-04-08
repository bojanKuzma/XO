using System.Windows.Input;
using XO.Commands;
using XO.Models;
using XO.Stores;

namespace XO.ViewModels;

public class ScoresViewModel : BaseViewModel
{
    
    public ICommand GoBackCommand { get; }
    public IEnumerable<PlayerStats> PlayerStats => 
        StatisticsService.GetAllStats().Values.OrderByDescending(s => s.Wins);

    public ScoresViewModel(NavigationStore navigationStore, LanguageStore languageStore)
    {
        GoBackCommand = new ActionCommand<object>(_ => GoBack(navigationStore, languageStore));
    }

    private void GoBack(NavigationStore navigationStore, LanguageStore languageStore)
    {
        navigationStore.CurrentViewModel = new MainMenuViewModel(navigationStore, languageStore);
    }
}
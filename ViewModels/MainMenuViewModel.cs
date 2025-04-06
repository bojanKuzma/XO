using System.Windows;
using System.Windows.Input;
using XO.Commands;
using XO.Stores;

namespace XO.ViewModels;

public class MainMenuViewModel : BaseViewModel
{
    private NavigationStore _navigationStore;

    public MainMenuViewModel(NavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
        StartOnePlayerGameCommand = new ActionCommand<object>(_ => StartOnePlayerGame());
        StartTwoPlayerGameCommand = new ActionCommand<object>(_ => StartTwoPlayerGame());
        ShowScoresCommand = new ActionCommand<object>(_ => ShowScores());
        ChangeLanguageCommand = new ActionCommand<string>(ChangeLanguage);
    }
    

    public ICommand StartOnePlayerGameCommand { get; }
    public ICommand StartTwoPlayerGameCommand { get; }
    public ICommand ShowScoresCommand { get; }
    public ICommand ChangeLanguageCommand { get; }
    
    
    private void UpdateResourceDictionary(string resourcePath, Func<ResourceDictionary, bool> condition)
    {
        var mergedDictionaries = Application.Current.Resources.MergedDictionaries;

        // Remove existing dictionaries that match the condition
        for (int i = mergedDictionaries.Count - 1; i >= 0; i--)
        {
            var dict = mergedDictionaries[i];
            if (condition(dict))
            {
                mergedDictionaries.RemoveAt(i);
            }
        }

        // Add the new resource dictionary
        var resourceUri = new Uri(resourcePath, UriKind.Relative);
        var resourceDict = new ResourceDictionary { Source = resourceUri };
        mergedDictionaries.Add(resourceDict);
    }

    private void ChangeLanguage(string languageCode)
    {
       
        bool IsLanguageDictionary(ResourceDictionary dict) => 
            dict.Source != null && dict.Source.ToString().Contains("Resources/");
        UpdateResourceDictionary($"Resources/locale.{languageCode}.xaml", IsLanguageDictionary);
    }
    
    
    private void StartOnePlayerGame()
    {
        _navigationStore.CurrentViewModel = new GameViewModel(true, _navigationStore);
    }

    private void StartTwoPlayerGame()
    {
        _navigationStore.CurrentViewModel = new GameViewModel(false, _navigationStore);
    }

    private void ShowScores()
    {
        _navigationStore.CurrentViewModel?.Dispose();
        _navigationStore.CurrentViewModel = new ScoresViewModel();
    }
}